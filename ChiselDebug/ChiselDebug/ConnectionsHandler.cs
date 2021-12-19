using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Routing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebug
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
                Dictionary<FIRRTLNode, Point> nodePoses = new Dictionary<FIRRTLNode, Point>();
                foreach (var nodePos in placements.NodePositions)
                {
                    nodePoses.Add(nodePos.Value, nodePos.Position);
                }
                nodePoses.Add(Mod, Point.Zero);

                List<(IOInfo start, IOInfo end)> lines = new List<(IOInfo start, IOInfo end)>();

                Dictionary<Output, HashSet<Input>> disallowedConnections = new Dictionary<Output, HashSet<Input>>();
                HashSet<AggregateIO> alreadyMade = new HashSet<AggregateIO>();
                List<Output> outputIOs = IOInfos.Keys.OfType<Output>().ToList();
                foreach (var output in outputIOs)
                {
                    if (!output.IsPartOfAggregateIO)
                    {
                        continue;
                    }



                    var outputParent = output.ParentIO;
                    if (!IOInfos.ContainsKey(outputParent))
                    {
                        continue;
                    }

                    if (alreadyMade.Contains(outputParent))
                    {
                        continue;
                    }

                    ScalarIO[] outputParentScalarIO = outputParent.Flatten().ToArray();
                    if (!outputParentScalarIO.All(x => IOInfos.ContainsKey(x)))
                    {
                        continue;
                    }

                    foreach (var input in output.GetConnectedInputs())
                    {
                        if (!input.IsPartOfAggregateIO)
                        {
                            continue;
                        }

                        var inputParent = input.ParentIO;
                        if (!IOInfos.ContainsKey(inputParent))
                        {
                            continue;
                        }

                        ScalarIO[] inputParentScalarIO = inputParent.Flatten().ToArray();
                        if (!inputParentScalarIO.All(x => IOInfos.ContainsKey(x)))
                        {
                            continue;
                        }

                        if (outputParentScalarIO.Length != inputParentScalarIO.Length)
                        {
                            continue;
                        }

                        bool isFaliure = false;
                        List<(Output from, Input to)> connections = new List<(Output from, Input to)>();
                        for (int i = 0; i < outputParentScalarIO.Length; i++)
                        {
                            ScalarIO fromOutputIO = outputParentScalarIO[i];
                            ScalarIO fromInputIO = inputParentScalarIO[i];
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
                                isFaliure = true;
                                break;
                            }

                            if (!outputIO.GetConnectedInputs().Any(x => x == inputIO))
                            {
                                isFaliure = true;
                                break;
                            }

                            connections.Add((outputIO, inputIO));
                        }

                        if (isFaliure)
                        {
                            break;
                        }

                        foreach (var connection in connections)
                        {
                            disallowedConnections.TryAdd(connection.from, new HashSet<Input>());
                            disallowedConnections[connection.from].Add(connection.to);
                        }

                        IOInfo outputInfo = IOInfos[outputParent];
                        IOInfo inputInfo = IOInfos[inputParent];

                        Point startOffset = nodePoses[outputInfo.Node];
                        Point endOffset = nodePoses[inputInfo.Node];

                        IOInfo offsetStartIO = new IOInfo(outputInfo.Node, outputInfo.DirIO.WithOffsetPosition(startOffset));
                        IOInfo offsetEndIO = new IOInfo(inputInfo.Node, inputInfo.DirIO.WithOffsetPosition(endOffset));

                        lines.Add((offsetStartIO, offsetEndIO));
                        alreadyMade.Add(outputParent);
                        break;
                    }
                }

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
                                Point startOffset = nodePoses[outputInfo.Node];
                                Point endOffset = nodePoses[inputInfo.Node];

                                IOInfo offsetStartIO = new IOInfo(outputInfo.Node, outputInfo.DirIO.WithOffsetPosition(startOffset));
                                IOInfo offsetEndIO = new IOInfo(inputInfo.Node, inputInfo.DirIO.WithOffsetPosition(endOffset));

                                lines.Add((offsetStartIO, offsetEndIO));
                            }
                        }
                    }
                }

                return lines;
            }
        }
    }
}
