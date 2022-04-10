using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Placing;
using ChiselDebug.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.Routing
{
    public sealed class ConnectionsHandler
    {

        private readonly Module Mod;
        private readonly List<Source> UsedModuleConnections;
        private readonly Dictionary<FIRIO, IOInfo> IOInfos = new Dictionary<FIRIO, IOInfo>();

        public ConnectionsHandler(Module mod)
        {
            this.Mod = mod;
            this.UsedModuleConnections = Mod.GetAllModuleConnections();
            UsedModuleConnections.RemoveAll(x => x.Node is INoPlaceAndRoute);
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

        internal List<LineInfo> GetAllConnectionLines(PlacementInfo placements)
        {
            lock (this)
            {
                Dictionary<FIRRTLNode, Point> nodePoses = placements.NodePositions.ToDictionary(x => x.Value, x => x.Position);
                nodePoses.Add(Mod, Point.Zero);

                List<LineInfo> lines = new List<LineInfo>();
                lines.AddRange(MakeAggregateLines(nodePoses, out var disallowedConnections));
                lines.AddRange(MakeScalarLines(nodePoses, disallowedConnections));

                return lines;
            }
        }

        private List<LineInfo> MakeAggregateLines(Dictionary<FIRRTLNode, Point> nodePoses, out Dictionary<Source, HashSet<Sink>> disallowedConnections)
        {
            disallowedConnections = new Dictionary<Source, HashSet<Sink>>();
            List<LineInfo> lines = new List<LineInfo>();
            Dictionary<AggregateIO, HashSet<AggregateIO>> alreadyMade = new Dictionary<AggregateIO, HashSet<AggregateIO>>();
            foreach (var aggregateOutput in IOInfos.Keys.OfType<AggregateIO>().Where(x => x.IsPassiveOfType<Source>()))
            {
                foreach (var endPoint in aggregateOutput.GetConnections().Select(x => x.To))
                {
                    alreadyMade.TryAdd(aggregateOutput, new HashSet<AggregateIO>());
                    if (!alreadyMade[aggregateOutput].Add(endPoint))
                    {
                        continue;
                    }

                    var connections = aggregateOutput.Flatten().Zip(endPoint.Flatten()).Select(x => IOHelper.OrderIO(x.First, x.Second));
                    foreach (var connection in connections)
                    {
                        disallowedConnections.TryAdd(connection.output, new HashSet<Sink>());
                        disallowedConnections[connection.output].Add(connection.input);
                    }

                    IOInfo outputInfo = IOInfos[aggregateOutput];
                    IOInfo inputInfo = IOInfos[endPoint];
                    lines.Add(MakeLine(nodePoses, outputInfo, inputInfo));
                }
            }

            return lines;
        }

        private List<LineInfo> MakeScalarLines(Dictionary<FIRRTLNode, Point> nodePoses, Dictionary<Source, HashSet<Sink>> disallowedConnections)
        {
            List<LineInfo> lines = new List<LineInfo>();
            HashSet<Sink> allAllowed = new HashSet<Sink>();
            foreach (var connection in UsedModuleConnections)
            {
                HashSet<Sink> notAllowedInputs = disallowedConnections.GetValueOrDefault(connection) ?? allAllowed;
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

        private LineInfo MakeLine(Dictionary<FIRRTLNode, Point> nodePoses, IOInfo outputInfo, IOInfo inputInfo)
        {
            Point startOffset = nodePoses[outputInfo.Node];
            Point endOffset = nodePoses[inputInfo.Node];

            IOInfo offsetStartIO = new IOInfo(outputInfo.Node, outputInfo.DirIO.WithOffsetPosition(startOffset));
            IOInfo offsetEndIO = new IOInfo(inputInfo.Node, inputInfo.DirIO.WithOffsetPosition(endOffset));

            return new LineInfo(offsetStartIO, offsetEndIO);
        }
    }
}
