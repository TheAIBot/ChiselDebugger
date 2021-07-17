﻿using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ChiselDebug.CombGraph
{
    public class CombComputeOrder
    {
        private readonly Output[] StartOutputs;
        private Computable[] ComputeOrder;

        public CombComputeOrder(Output[] startOutputs, Computable[] computeOrder)
        {
            this.StartOutputs = startOutputs;
            this.ComputeOrder = computeOrder;
        }

        public List<Output> ComputeAndGetChanged()
        {
            List<Output> updatedConnections = new List<Output>();
            for (int i = 0; i < ComputeOrder.Length; i++)
            {
                Output updated = ComputeOrder[i].ComputeGetIfChanged();
                if (updated != null)
                {
                    updatedConnections.Add(updated);
                }
            }

            return updatedConnections;
        }

        public void ComputeFast()
        {
            for (int i = 0; i < ComputeOrder.Length; i++)
            {
                ComputeOrder[i].ComputeFast();
            }
        }

        public void InferTypes()
        {
            for (int i = 0; i < ComputeOrder.Length; i++)
            {
                ComputeOrder[i].InferType();
            }
        }

        public ReadOnlySpan<Output> GetAllRootSources()
        {
            return StartOutputs;
        }

        public ReadOnlySpan<Computable> GetComputeOrder()
        {
            return ComputeOrder;
        }

        public void SetComputeOrder(Computable[] order)
        {
            ComputeOrder = order;
        }

        public CombComputeOrder Copy()
        {
            return new CombComputeOrder(StartOutputs.ToArray(), ComputeOrder.ToArray());
        }

        public static CombComputeOrder MakeMonoGraph(Module module)
        {
            List<Output> startingPoints = new List<Output>();
            foreach (var childMod in module.GetAllNestedNodesOfType<Module>())
            {
                foreach (var output in childMod.GetInternalOutputs())
                {
                    Input paired = (Input)output.GetPaired();
                    if (output.IsConnectedToAnything() && !paired.IsConnectedToAnything())
                    {
                        startingPoints.Add(output);
                    }
                }
                foreach (var input in childMod.GetInternalInputs())
                {
                    Output paired = (Output)input.GetPaired();
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
                    Input paired = (Input)output.GetPaired();
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

        private readonly struct SourceSinkCon
        {
            private readonly Output Source;
            private readonly Input Sink;

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

        private static CombComputeOrder MakeCombComputeNode(Output[] outputs)
        {
            HashSet<Output> seenCons = new HashSet<Output>();
            HashSet<SourceSinkCon> seenSourceSinkCons = new HashSet<SourceSinkCon>();
            List<Computable> computeOrder = new List<Computable>();

            void AddMissingCons(HashSet<Output> missingCons, Input input)
            {
                foreach (var con in input.GetConnections())
                {
                    if (!seenCons.Contains(con.From))
                    {
                        missingCons.Add(con.From);
                    }
                }
            }

            void AddMaybeBlockedCon(Stack<(Output con, Input input)> toTraverse, Dictionary<Output, HashSet<Output>> blockedOutputs, Output output)
            {
                Input[] conInputs = output.GetConnectedInputs().ToArray();
                if (conInputs.Length == 0)
                {
                    if (seenCons.Add(output))
                    {
                        computeOrder.Add(new Computable(output));
                    }
                    return;
                }

                foreach (var input in conInputs)
                {
                    foreach (var connection in input.GetConnections())
                    {
                        if (connection.From !=  output)
                        {
                            continue;
                        }

                        if (connection.Condition != null && !seenCons.Contains(connection.Condition) && connection.Condition != output)
                        {
                            HashSet<Output> blocked;
                            if (!blockedOutputs.TryGetValue(connection.Condition, out blocked))
                            {
                                blocked = new HashSet<Output>();
                                blockedOutputs.Add(connection.Condition, blocked);
                            }

                            blocked.Add(output);
                        }
                        else
                        {
                            if (seenCons.Add(output))
                            {
                                computeOrder.Add(new Computable(output));
                            }

                            SourceSinkCon con = new SourceSinkCon(output, input);
                            if (seenSourceSinkCons.Add(con))
                            {
                                toTraverse.Push((output, input));
                            }
                        }
                    }

                }
            }



            Dictionary<FIRRTLNode, HashSet<Output>> seenButMissingFirNodeInputs = new Dictionary<FIRRTLNode, HashSet<Output>>();
            Dictionary<Input, HashSet<Output>> seenButMissingBorderInputCons = new Dictionary<Input, HashSet<Output>>();
            HashSet<FIRRTLNode> finishedNodes = new HashSet<FIRRTLNode>();
            Dictionary<Output, HashSet<Output>> blockedOutputs = new Dictionary<Output, HashSet<Output>>();
            Dictionary<Output, List<FIRRTLNode>> nodeInputBlocker = new Dictionary<Output, List<FIRRTLNode>>();
            Dictionary<Output, List<Input>> modInputBlocker = new Dictionary<Output, List<Input>>();

            foreach (var computeFirst in outputs.Where(x => x.Node is not Module && (x.Node is not IStatePreserving)).Select(x => x.Node).Distinct())
            {
                computeOrder.Add(new Computable(computeFirst));
            }

            Stack<(Output con, Input input)> toTraverse = new Stack<(Output con, Input input)>();


            void FoundNodeDep(Output con, FIRRTLNode node, HashSet<Output> missingCons)
            {
                missingCons.Remove(con);

                //If this graph has provided all inputs to this component then
                //it can finally compute the component and continue the graph
                //with the components output
                if (missingCons.Count == 0)
                {
                    if (node is not IStatePreserving)
                    {
                        computeOrder.Add(new Computable(node));
                    }

                    foreach (var nodeOutput in node.GetOutputs())
                    {
                        AddMaybeBlockedCon(toTraverse, blockedOutputs, nodeOutput);
                    }

                    finishedNodes.Add(node);
                    seenButMissingFirNodeInputs.Remove(node);
                }
            }

            void FoundBorderInputDep(Output con, Input modInput, HashSet<Output> missingCons)
            {
                missingCons.Remove(con);

                if (missingCons.Count == 0)
                {
                    Output inPairedOut = (Output)modInput.GetPaired();
                    AddMaybeBlockedCon(toTraverse, blockedOutputs, inPairedOut);

                    seenButMissingBorderInputCons.Remove(modInput);
                }
            }


            foreach (var output in outputs)
            {
                AddMaybeBlockedCon(toTraverse, blockedOutputs, output);

                while (toTraverse.Count > 0)
                {
                    var conInput = toTraverse.Pop();
                    if (blockedOutputs.TryGetValue(conInput.con, out var blockedOuts))
                    {
                        foreach (var blocked in blockedOuts)
                        {
                            AddMaybeBlockedCon(toTraverse, blockedOutputs, blocked);
                        }
                        blockedOutputs.Remove(conInput.con);
                    }
                    if (nodeInputBlocker.TryGetValue(conInput.con, out var blockedNodeInputs))
                    {
                        foreach (var blockedNode in blockedNodeInputs)
                        {
                            FoundNodeDep(conInput.con, blockedNode, seenButMissingFirNodeInputs[blockedNode]);
                        }
                        nodeInputBlocker.Remove(conInput.con);
                    }
                    if (modInputBlocker.TryGetValue(conInput.con, out var blockedModInputs))
                    {
                        foreach (var blockedInput in blockedModInputs)
                        {
                            FoundBorderInputDep(conInput.con, blockedInput, seenButMissingBorderInputCons[blockedInput]);
                        }
                        modInputBlocker.Remove(conInput.con);
                    }

                    //Punch through module border to continue search on the other side
                    if (conInput.input.Node is Module || conInput.input.Node is Wire)
                    {
                        HashSet<Output> missingCons;
                        if (!seenButMissingBorderInputCons.TryGetValue(conInput.input, out missingCons))
                        {
                            missingCons = new HashSet<Output>();
                            seenButMissingBorderInputCons.Add(conInput.input, missingCons);

                            AddMissingCons(missingCons, conInput.input);

                            foreach (var condCon in conInput.input.GetConnections())
                            {
                                if (condCon.Condition != null && !seenCons.Contains(condCon.Condition))
                                {
                                    missingCons.Add(condCon.Condition);
                                    modInputBlocker.TryAdd(condCon.Condition, new List<Input>());
                                    modInputBlocker[condCon.Condition].Add(conInput.input);
                                }
                            }
                        }

                        FoundBorderInputDep(conInput.con, conInput.input, missingCons);
                    }
                    //Ignore state preserving components as a combinatorial graph
                    //shouldn't cross those
                    else if (conInput.input.Node is IStatePreserving)
                    {
                        continue;
                    }
                    else
                    {
                        if (finishedNodes.Contains(conInput.input.Node))
                        {
                            Debug.Assert(conInput.input.Node.GetInputs().SelectMany(x => x.GetConnections()).All(x => seenCons.Contains(x.From)));
                            continue;
                        }
                        HashSet<Output> missingCons;
                        if (!seenButMissingFirNodeInputs.TryGetValue(conInput.input.Node, out missingCons))
                        {
                            missingCons = new HashSet<Output>();
                            seenButMissingFirNodeInputs.Add(conInput.input.Node, missingCons);

                            ScalarIO[] nodeInputs = conInput.input.Node.GetInputs();
                            foreach (Input input in nodeInputs)
                            {
                                AddMissingCons(missingCons, input);
                            }
                            foreach (Input input in nodeInputs)
                            {
                                foreach (var condCon in input.GetConnections())
                                {
                                    if (condCon.Condition != null && !seenCons.Contains(condCon.Condition))
                                    {
                                        missingCons.Add(condCon.Condition);
                                        nodeInputBlocker.TryAdd(condCon.Condition, new List<FIRRTLNode>());
                                        nodeInputBlocker[condCon.Condition].Add(conInput.input.Node);
                                    }
                                }
                            }
                        }

                        FoundNodeDep(conInput.con, conInput.input.Node, missingCons);
                    }
                }
            }

            return new CombComputeOrder(outputs, computeOrder.ToArray());
        }
    }
}
