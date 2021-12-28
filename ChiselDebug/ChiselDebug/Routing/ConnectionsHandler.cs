using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.Routing
{
    public class ConnectionsHandler
    {

        private readonly Module Mod;
        private readonly List<Output> UsedModuleConnections;
        private readonly Dictionary<FIRIO, IOInfo> IOInfos = new Dictionary<FIRIO, IOInfo>();

        public ConnectionsHandler(Module mod)
        {
            this.Mod = mod;
            this.UsedModuleConnections = Mod.GetAllModuleConnections();
            UsedModuleConnections.RemoveAll(x => x is INoPlaceAndRoute);
        }

        public void UpdateIOFromNode(FIRRTLNode node, DirectedIO[] inputOffsets, DirectedIO[] outputOffsets)
        {
            lock (this)
            {
                foreach (var inputPos in inputOffsets)
                {
                    IOInfos[inputPos.IO] = new IOInfo(node, inputPos);
                }
                foreach (var inputPos in outputOffsets)
                {
                    IOInfos[inputPos.IO] = new IOInfo(node, inputPos);
                }
            }
        }

        internal List<(IOInfo start, IOInfo end)> GetAllConnectionLines(PlacementInfo placements)
        {
            lock (this)
            {
                Dictionary<FIRRTLNode, Point> nodePoses = placements.NodePositions.ToDictionary(x => x.Value, x => x.Position);
                nodePoses.Add(Mod, Point.Zero);

                List<(IOInfo start, IOInfo end)> lines = new List<(IOInfo start, IOInfo end)>();
                lines.AddRange(MakeAggregateLines(nodePoses, out var disallowedConnections));
                lines.AddRange(MakeScalarLines(nodePoses, disallowedConnections));

                return lines;
            }
        }

        private List<(IOInfo start, IOInfo end)> MakeAggregateLines(Dictionary<FIRRTLNode, Point> nodePoses, out Dictionary<Output, HashSet<Input>> disallowedConnections)
        {
            disallowedConnections = new Dictionary<Output, HashSet<Input>>();
            List<(IOInfo start, IOInfo end)> lines = new List<(IOInfo start, IOInfo end)>();
            Dictionary<AggregateIO, HashSet<AggregateIO>> alreadyMade = new Dictionary<AggregateIO, HashSet<AggregateIO>>();
            List<Output> outputIOs = IOInfos.Keys.OfType<Output>().ToList();
            foreach (var output in outputIOs)
            {
                if (!output.IsPartOfAggregateIO)
                {
                    continue;
                }

                AggregateIO outputParent = output.ParentIO;
                if (!outputParent.Flatten().All(x => IOInfos.ContainsKey(x)))
                {
                    continue;
                }

                foreach (var endPoint in outputParent.GetConnections())
                {
                    if (!endPoint.Flatten().All(x => IOInfos.ContainsKey(x)))
                    {
                        continue;
                    }

                    alreadyMade.TryAdd(outputParent, new HashSet<AggregateIO>());
                    if (!alreadyMade[outputParent].Add(endPoint))
                    {
                        continue;
                    }

                    var connections = outputParent.Flatten().Zip(endPoint.Flatten()).Select(x => IOHelper.OrderIO(x.First, x.Second));
                    foreach (var connection in connections)
                    {
                        disallowedConnections.TryAdd(connection.output, new HashSet<Input>());
                        disallowedConnections[connection.output].Add(connection.input);
                    }

                    IOInfo outputInfo = IOInfos[outputParent];
                    IOInfo inputInfo = IOInfos[endPoint];
                    lines.Add(MakeLine(nodePoses, outputInfo, inputInfo));
                    break;
                }
            }

            return lines;
        }

        private List<(IOInfo start, IOInfo end)> MakeScalarLines(Dictionary<FIRRTLNode, Point> nodePoses, Dictionary<Output, HashSet<Input>> disallowedConnections)
        {
            List<(IOInfo start, IOInfo end)> lines = new List<(IOInfo start, IOInfo end)>();
            HashSet<Input> allAllowed = new HashSet<Input>();
            foreach (var connection in UsedModuleConnections)
            {
                HashSet<Input> notAllowedInputs = disallowedConnections.GetValueOrDefault(connection) ?? allAllowed;
                if (IOInfos.TryGetValue(connection, out IOInfo outputInfo))
                {
                    foreach (var input in connection.GetConnectedInputs())
                    {
                        if (notAllowedInputs.Contains(input))
                        {
                            continue;
                        }

                        if (IOInfos.TryGetValue(input, out IOInfo inputInfo))
                        {
                            lines.Add(MakeLine(nodePoses, outputInfo, inputInfo));
                        }
                    }
                }
            }

            return lines;
        }

        private (IOInfo start, IOInfo end) MakeLine(Dictionary<FIRRTLNode, Point> nodePoses, IOInfo outputInfo, IOInfo inputInfo)
        {
            Point startOffset = nodePoses[outputInfo.Node];
            Point endOffset = nodePoses[inputInfo.Node];

            IOInfo offsetStartIO = new IOInfo(outputInfo.Node, outputInfo.DirIO.WithOffsetPosition(startOffset));
            IOInfo offsetEndIO = new IOInfo(inputInfo.Node, inputInfo.DirIO.WithOffsetPosition(endOffset));

            return (offsetStartIO, offsetEndIO);
        }
    }
}
