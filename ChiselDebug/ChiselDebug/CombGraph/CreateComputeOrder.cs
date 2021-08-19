﻿using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.CombGraph
{
    internal static class CreateComputeOrder
    {
        public static CombComputeOrder<Computable> MakeMonoGraph(Module module)
        {
            List<Output> startingPoints = new List<Output>();
            foreach (var childMod in module.GetAllNestedNodesOfType<Module>())
            {
                foreach (var output in childMod.GetInternalOutputs())
                {
                    Input paired = output.GetPaired();
                    if (output.IsConnectedToAnything() && !paired.IsConnectedToAnything())
                    {
                        startingPoints.Add(output);
                    }
                }
                foreach (var input in childMod.GetInternalInputs())
                {
                    Output paired = input.GetPaired();
                    if (paired.IsConnectedToAnything() && !input.IsConnectedToAnything())
                    {
                        startingPoints.Add(paired);
                    }
                }
            }
            foreach (var wire in module.GetAllNestedNodesOfType<Wire>())
            {
                foreach (var output in wire.GetOutputs())
                {
                    Input paired = output.GetPaired();
                    if (!paired.IsConnectedToAnything())
                    {
                        startingPoints.Add(output);
                    }
                }
            }
            foreach (var output in module.GetInternalOutputs())
            {
                if (!startingPoints.Contains(output))
                {
                    startingPoints.Add(output);
                }
            }
            foreach (var constVal in module.GetAllNestedNodesOfType<ConstValue>())
            {
                startingPoints.AddRange(constVal.GetOutputs());
            }
            foreach (var statePres in module.GetAllNestedNodesOfType<IStatePreserving>())
            {
                startingPoints.AddRange(((FIRRTLNode)(statePres)).GetOutputs());
            }

            return MakeCombComputeNode(startingPoints.ToArray());
        }

        private static CombComputeOrder<Computable> MakeCombComputeNode(Output[] outputs)
        {
            HashSet<Output> seenCons = new HashSet<Output>();
            HashSet<SourceSinkCon> seenSourceSinkCons = new HashSet<SourceSinkCon>();
            List<Computable> computeOrder = new List<Computable>();

            void AddMissingCons<T>(HashSet<Output> missingCons, Input input, Dictionary<Output, List<T>> blocker, T blocked)
            {
                foreach (var con in input.GetConnections())
                {
                    if (!seenCons.Contains(con.From))
                    {
                        missingCons.Add(con.From);
                    }
                    if (con.Condition != null && !seenCons.Contains(con.Condition))
                    {
                        missingCons.Add(con.Condition);
                        blocker.TryAdd(con.Condition, new List<T>());
                        blocker[con.Condition].Add(blocked);
                    }
                }
            }

            void AddSinkToSearch(Stack<SourceSinkCon> toTraverse, Output output)
            {
                if (seenCons.Add(output))
                {
                    computeOrder.Add(new Computable(output));
                }

                foreach (var input in output.GetConnectedInputs())
                {
                    SourceSinkCon con = new SourceSinkCon(output, input);
                    if (seenSourceSinkCons.Add(con))
                    {
                        toTraverse.Push(con);
                    }
                }
            }

            Dictionary<FIRRTLNode, HashSet<Output>> seenButMissingFirNodeInputs = new Dictionary<FIRRTLNode, HashSet<Output>>();
            Dictionary<Input, HashSet<Output>> seenButMissingBorderInputCons = new Dictionary<Input, HashSet<Output>>();
            Dictionary<Output, List<FIRRTLNode>> nodeInputBlocker = new Dictionary<Output, List<FIRRTLNode>>();
            Dictionary<Output, List<Input>> modInputBlocker = new Dictionary<Output, List<Input>>();

            foreach (var computeFirst in outputs.Where(x => x.Node is not Module && (x.Node is not IStatePreserving)).Select(x => x.Node).Distinct())
            {
                computeOrder.Add(new Computable(computeFirst));
            }

            Stack<SourceSinkCon> toTraverse = new Stack<SourceSinkCon>();


            void FoundNodeDep(Output con, FIRRTLNode node, HashSet<Output> missingCons)
            {
                missingCons.Remove(con);

                //If this graph has provided all inputs to this component then
                //it can finally compute the component and continue the graph
                //with the components output
                if (missingCons.Count == 0)
                {
                    computeOrder.Add(new Computable(node));

                    foreach (var nodeOutput in node.GetOutputs())
                    {
                        AddSinkToSearch(toTraverse, nodeOutput);
                    }

                    seenButMissingFirNodeInputs.Remove(node);
                }
            }

            void FoundBorderInputDep(Output con, Input modInput, HashSet<Output> missingCons)
            {
                missingCons.Remove(con);

                if (missingCons.Count == 0)
                {
                    Output inPairedOut = modInput.GetPaired();
                    AddSinkToSearch(toTraverse, inPairedOut);

                    seenButMissingBorderInputCons.Remove(modInput);
                }
            }


            foreach (var output in outputs)
            {
                AddSinkToSearch(toTraverse, output);

                while (toTraverse.Count > 0)
                {
                    var conInput = toTraverse.Pop();
                    if (nodeInputBlocker.TryGetValue(conInput.Source, out var blockedNodeInputs))
                    {
                        foreach (var blockedNode in blockedNodeInputs)
                        {
                            FoundNodeDep(conInput.Source, blockedNode, seenButMissingFirNodeInputs[blockedNode]);
                        }
                        nodeInputBlocker.Remove(conInput.Source);
                    }
                    if (modInputBlocker.TryGetValue(conInput.Source, out var blockedModInputs))
                    {
                        foreach (var blockedInput in blockedModInputs)
                        {
                            FoundBorderInputDep(conInput.Source, blockedInput, seenButMissingBorderInputCons[blockedInput]);
                        }
                        modInputBlocker.Remove(conInput.Source);
                    }

                    //Punch through module border to continue search on the other side
                    if (conInput.Sink.Node is Module || conInput.Sink.Node is Wire)
                    {
                        HashSet<Output> missingCons;
                        if (!seenButMissingBorderInputCons.TryGetValue(conInput.Sink, out missingCons))
                        {
                            missingCons = new HashSet<Output>();
                            seenButMissingBorderInputCons.Add(conInput.Sink, missingCons);

                            AddMissingCons(missingCons, conInput.Sink, modInputBlocker, conInput.Sink);
                        }

                        FoundBorderInputDep(conInput.Source, conInput.Sink, missingCons);
                    }
                    //Ignore state preserving components as a combinatorial graph
                    //shouldn't cross those
                    else if (conInput.Sink.Node is IStatePreserving)
                    {
                        continue;
                    }
                    else
                    {
                        HashSet<Output> missingCons;
                        if (!seenButMissingFirNodeInputs.TryGetValue(conInput.Sink.Node, out missingCons))
                        {
                            missingCons = new HashSet<Output>();
                            seenButMissingFirNodeInputs.Add(conInput.Sink.Node, missingCons);

                            ScalarIO[] nodeInputs = conInput.Sink.Node.GetInputs();
                            foreach (Input input in nodeInputs)
                            {
                                AddMissingCons(missingCons, input, nodeInputBlocker, conInput.Sink.Node);
                            }
                        }

                        FoundNodeDep(conInput.Source, conInput.Sink.Node, missingCons);
                    }
                }
            }

            return new CombComputeOrder<Computable>(outputs, computeOrder.ToArray());
        }
    }

    internal readonly struct SourceSinkCon
    {
        public readonly Output Source;
        public readonly Input Sink;

        public SourceSinkCon(Output source, Input sink)
        {
            this.Source = source;
            this.Sink = sink;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Source, Sink);
        }
    }
}
