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
            HashSet<AggregateIO> alreadyMade = new HashSet<AggregateIO>();
            List<Output> outputIOs = IOInfos.Keys.OfType<Output>().ToList();
            foreach (var output in outputIOs)
            {
                ScalarIO[] outputParentScalarIO;
                IOInfo outputInfo;
                if (!TryCanIOMakeAggregateConnection(alreadyMade, output, out outputParentScalarIO, out outputInfo))
                {
                    continue;
                }

                foreach (var input in output.GetConnectedInputs())
                {
                    ScalarIO[] inputParentScalarIO;
                    IOInfo inputInfo;
                    if (!TryCanIOMakeAggregateConnection(alreadyMade, input, out inputParentScalarIO, out inputInfo))
                    {
                        continue;
                    }

                    if (outputParentScalarIO.Length != inputParentScalarIO.Length)
                    {
                        continue;
                    }

                    if (TryGetAllAggregateConnections(outputParentScalarIO, inputParentScalarIO, out var connections))
                    {
                        // The connections between inputs/outputs are replaced with this
                        // single aggregate connection and therefore we need to mark the
                        // scalar connections as not allowed so they won't later be made
                        foreach (var connection in connections)
                        {
                            disallowedConnections.TryAdd(connection.from, new HashSet<Input>());
                            disallowedConnections[connection.from].Add(connection.to);
                        }

                        lines.Add(MakeLine(nodePoses, outputInfo, inputInfo));
                        break;
                    }
                }
            }

            return lines;
        }

        private bool TryCanIOMakeAggregateConnection(HashSet<AggregateIO> alreadyMade, ScalarIO scalar, out ScalarIO[] aggScalarIO, out IOInfo aggIOInfo)
        {
            if (!scalar.IsPartOfAggregateIO)
            {
                aggScalarIO = null;
                aggIOInfo = null;
                return false;
            }

            var scalarParent = scalar.ParentIO;
            if (!IOInfos.TryGetValue(scalarParent, out aggIOInfo))
            {
                aggScalarIO = null;
                aggIOInfo = null;
                return false;
            }

            if (!alreadyMade.Add(scalarParent))
            {
                aggScalarIO = null;
                aggIOInfo = null;
                return false;
            }

            aggScalarIO = scalarParent.Flatten().ToArray();
            if (!aggScalarIO.All(x => IOInfos.ContainsKey(x)))
            {
                aggScalarIO = null;
                aggIOInfo = null;
                return false;
            }

            return true;
        }

        private bool TryGetAllAggregateConnections(ScalarIO[] aggOutputIO, ScalarIO[] aggInputIO, out List<(Output from, Input to)> connections)
        {
            connections = new List<(Output from, Input to)>();
            for (int i = 0; i < aggOutputIO.Length; i++)
            {
                ScalarIO fromOutputIO = aggOutputIO[i];
                ScalarIO fromInputIO = aggInputIO[i];

                Output outputIO;
                Input inputIO;
                if (fromOutputIO is Output && fromInputIO is Input)
                {
                    outputIO = (Output)fromOutputIO;
                    inputIO = (Input)fromInputIO;
                }
                else if (fromOutputIO is Input && fromInputIO is Output)
                {
                    outputIO = (Output)fromInputIO;
                    inputIO = (Input)fromOutputIO;
                }
                else
                {
                    return false;
                }

                if (!outputIO.GetConnectedInputs().Any(x => x == inputIO))
                {
                    return false;
                }

                connections.Add((outputIO, inputIO));
            }

            return true;
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
