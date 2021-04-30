﻿using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ChiselDebug.CombGraph
{
    public class CombComputeGraph
    {
        private readonly List<CombComputeNode> Nodes = new List<CombComputeNode>();
        private readonly List<CombComputeNode> ConstComputeNodes = new List<CombComputeNode>();
        private readonly List<CombComputeNode> RootNodes = new List<CombComputeNode>();
        private readonly List<Output> RootSources;

        public CombComputeGraph(List<Output> rootSources)
        {
            this.RootSources = rootSources;
        }

        public void AddValueChangingNode(CombComputeNode combNode)
        {
            Nodes.Add(combNode);
        }

        public void AddConstNode(CombComputeNode constCombNode)
        {
            ConstComputeNodes.Add(constCombNode);
        }

        public void ComputeRoots()
        {
            foreach (var node in Nodes)
            {
                if (!node.HasComputeDependencies())
                {
                    RootNodes.Add(node);
                }
            }
            foreach (var node in ConstComputeNodes)
            {
                if (!node.HasComputeDependencies())
                {
                    RootNodes.Add(node);
                }
            }
        }

        public void ComputeConsts()
        {
            foreach (var node in ConstComputeNodes)
            {
                node.Compute();
            }
        }

        public List<Output> Compute()
        {
            Reset();

            int remainingNodes = Nodes.Count + ConstComputeNodes.Count;

            List<Output> updatedCons = new List<Output>();
            Queue<CombComputeNode> nodesReady = new Queue<CombComputeNode>();
            foreach (var root in RootNodes)
            {
                nodesReady.Enqueue(root);
            }

            while (nodesReady.Count > 0)
            {
                CombComputeNode node = nodesReady.Dequeue();
                remainingNodes--;

                updatedCons.AddRange(node.Compute());

                foreach (var maybeReady in node.GetEdges())
                {
                    if (!maybeReady.IsWaitingForDependencies())
                    {
                        nodesReady.Enqueue(maybeReady);
                    }
                }
            }

            Debug.Assert(remainingNodes == 0);

            return updatedCons;
        }

        public void InferTypes()
        {
            Reset();

            int remainingNodes = Nodes.Count + ConstComputeNodes.Count;

            Queue<CombComputeNode> nodesReady = new Queue<CombComputeNode>();
            foreach (var root in RootNodes)
            {
                nodesReady.Enqueue(root);
            }

            while (nodesReady.Count > 0)
            {
                CombComputeNode node = nodesReady.Dequeue();
                remainingNodes--;

                node.inferTypes();

                foreach (var maybeReady in node.GetEdges())
                {
                    if (!maybeReady.IsWaitingForDependencies())
                    {
                        nodesReady.Enqueue(maybeReady);
                    }
                }
            }

            Debug.Assert(remainingNodes == 0);
        }

        public void Reset()
        {
            foreach (var node in Nodes)
            {
                node.ResetRemainingDependencies();
            }
        }

        public CombComputeNode[] GetValueChangingNodes()
        {
            return Nodes.ToArray();
        }

        public CombComputeNode[] GetConstNodes()
        {
            return ConstComputeNodes.ToArray();
        }

        public CombComputeNode[] GetRootNodes()
        {
            return RootNodes.ToArray();
        }

        public CombComputeNode[] GetAllNodes()
        {
            List<CombComputeNode> allNodes = new List<CombComputeNode>();
            allNodes.AddRange(Nodes);
            allNodes.AddRange(ConstComputeNodes);

            return allNodes.ToArray();
        }

        public Output[] GetAllRootSources()
        {
            return RootSources.ToArray();
        }

        public static CombComputeGraph MakeGraph(Module module)
        {
            List<CombComputeNode> computeNodes = new List<CombComputeNode>();
            HashSet<Output> alreadyNode = new HashSet<Output>();
            Queue<Output[]> toMake = new Queue<Output[]>();

            foreach (var reg in module.GetAllNestedNodesOfType<Register>())
            {
                toMake.Enqueue(reg.GetOutputs());
            }
            foreach (var mem in module.GetAllNestedNodesOfType<Memory>())
            {
                toMake.Enqueue(mem.GetOutputs());
            }

            Output[] rootModuleIncommingPorts = module.GetInternalOutputs().ToArray();
            if (rootModuleIncommingPorts.Length > 0)
            {
                toMake.Enqueue(rootModuleIncommingPorts);
            }

            List<Output> rootSources = toMake.SelectMany(x => x).ToList();
            rootSources.AddRange(module.GetAllNestedNodesOfType<ConstValue>().Select(x => x.Result));


            var constNodesAndCons = GetAllCombNodeFromConstValue(module);

            Dictionary<CombComputeNode, HashSet<Output>> extraDeps = new Dictionary<CombComputeNode, HashSet<Output>>();


            HashSet<Output> seenOuts = new HashSet<Output>();
            foreach (var adw in constNodesAndCons.consFromConsts)
            {
                seenOuts.Add(adw);
            }
            while (true)
            {
                while (toMake.Count > 0)
                {
                    var make = toMake.Dequeue();
                    var compInfo = MakeCombComputeNode(make, constNodesAndCons.consFromConsts, false);
                    extraDeps.Add(compInfo.node, compInfo.depOnCons);

                    computeNodes.Add(compInfo.node);

                    foreach (var depNode in compInfo.depTo)
                    {
                        if (alreadyNode.Add(depNode.First()))
                        {
                            toMake.Enqueue(depNode);
                        }
                    }

                    foreach (var fgr in compInfo.node.GetResponsibleConnections())
                    {
                        seenOuts.Add(fgr);
                    }
                }

                bool addedAnything = false;
                foreach (var wfe in constNodesAndCons.endPoints)
                {
                    if (wfe.All(x => seenOuts.Add(x)))
                    {
                        toMake.Enqueue(wfe);
                        addedAnything = true;
                    }
                }

                if (!addedAnything)
                {
                    break;
                }
            }

            CombComputeGraph graph = new CombComputeGraph(rootSources);
            foreach (var node in computeNodes)
            {
                graph.AddValueChangingNode(node);
            }
            foreach (var constNode in constNodesAndCons.constNodes)
            {
                graph.AddConstNode(constNode);
            }

            CombComputeNode[] allNodes = graph.GetAllNodes();
            Dictionary<Input, List<CombComputeNode>> depToInputs = new Dictionary<Input, List<CombComputeNode>>();
            foreach (var node in allNodes)
            {
                foreach (var nodeStop in node.GetStopInputs())
                {
                    if (depToInputs.TryGetValue(nodeStop, out var deps))
                    {
                        deps.Add(node);
                    }
                    else
                    {
                        List<CombComputeNode> deps1 = new List<CombComputeNode>();
                        deps1.Add(node);
                        depToInputs.Add(nodeStop, deps1);
                    }
                }
            }

            Dictionary<Output, CombComputeNode> conToDep = new Dictionary<Output, CombComputeNode>();
            foreach (var node in allNodes)
            {
                foreach (var con in node.GetResponsibleConnections())
                {
                    conToDep.Add(con, node);
                }
            }

            Dictionary<CombComputeNode, HashSet<CombComputeNode>> nodeEdges = new Dictionary<CombComputeNode, HashSet<CombComputeNode>>();
            foreach (var node in allNodes)
            {
                nodeEdges.Add(node, new HashSet<CombComputeNode>());
            }
            foreach (var node in allNodes)
            {
                Output firstNodeStart = node.GetStartOutputs()[0];
                Input[] inputDeps;
                if (firstNodeStart.Node is Module mod)
                {
                    inputDeps = new Input[] { (Input)mod.GetPairedIO(firstNodeStart) };
                }
                else
                {
                    inputDeps = firstNodeStart.Node.GetInputs();
                }

                foreach (var inputDep in inputDeps)
                {
                    if (depToInputs.TryGetValue(inputDep, out var deps))
                    {
                        foreach (var dep in deps)
                        {
                            //Not allowed to have a dependency on itself
                            if (dep == node)
                            {
                                continue;
                            }
                            nodeEdges[dep].Add(node);
                        }
                    }
                }

                if (extraDeps.TryGetValue(node, out var extraConDeps))
                {
                    foreach (var con in extraConDeps)
                    {
                        CombComputeNode depNode = conToDep[con];
                        //Not allowed to have a dependency on itself
                        if (depNode == node)
                        {
                            continue;
                        }
                        nodeEdges[depNode].Add(node);
                    }
                }
            }
            foreach (var keyValue in nodeEdges)
            {
                keyValue.Key.AddEdges(keyValue.Value.ToList());
            }

            //graph.ComputeConsts();

            //Find graph roots so it doesn't have to be done in the future
            graph.ComputeRoots();

            return graph;
        }

        private static (List<CombComputeNode> constNodes, HashSet<Output> consFromConsts, List<Output[]> endPoints) GetAllCombNodeFromConstValue(Module module)
        {
            List<CombComputeNode> constNodes = new List<CombComputeNode>();
            HashSet<Output> consFromConsts = new HashSet<Output>();
            List<Output[]> endPoints = new List<Output[]>();

            foreach (var constValue in module.GetAllNestedNodesOfType<ConstValue>())
            {
                var compInfo = MakeCombComputeNode(new[] { constValue.Result }, consFromConsts, true);
                constNodes.Add(compInfo.node);

                foreach (var adwf in compInfo.depTo)
                {
                    endPoints.Add(adwf);
                }

                foreach (var con in compInfo.node.GetResponsibleConnections())
                {
                    consFromConsts.Add(con);
                }
            }

            return (constNodes, consFromConsts, endPoints);
        }

        public static CombComputeGraph MakeMonoGraph(Module module)
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
            foreach (var constVal in module.GetAllNestedNodesOfType<ConstValue>())
            {
                startingPoints.AddRange(constVal.GetOutputs());
            }

            var monoComb = MakeCombComputeNode(startingPoints.ToArray(), new HashSet<Output>(), false, false);
            if (monoComb.depTo.Count != 0)
            {
                throw new Exception();
            }
            if (monoComb.depOnCons.Count != 0)
            {
                throw new Exception();
            }

            CombComputeGraph graph = new CombComputeGraph(startingPoints);
            graph.AddValueChangingNode(monoComb.node);

            //Find graph roots so it doesn't have to be done in the future
            graph.ComputeRoots();

            return graph;
        }

        private static (CombComputeNode node, List<Output[]> depTo, HashSet<Output> depOnCons) MakeCombComputeNode(Output[] outputs, HashSet<Output> consFromConsts, bool ignoreConCondBorders, bool skipStatePre = true)
        {
            HashSet<Output> depOnCons = new HashSet<Output>();
            HashSet<Output> seenCons = new HashSet<Output>();
            List<Computable> computeOrder = new List<Computable>();

            bool HasUnSeenConCond(Output output)
            {
                if (output.IsConditional() && !seenCons.Contains(output.GetConditional()))
                {
                    return true;
                }

                return false;
            }

            void AddConnections(Queue<(Output con, Input input)> toTraverse, Output output)
            {
                Debug.Assert(!seenCons.Contains(output));
                if (seenCons.Add(output))
                {
                    computeOrder.Add(new Computable(output));
                }
                foreach (var input in output.GetConnectedInputs())
                {
                    toTraverse.Enqueue((output, input));
                }
                if (output.IsConditional())
                {
                    depOnCons.Add(output.GetConditional());
                }
            }

            void AddMissingCons(HashSet<Output> missingCons, Input input)
            {
                foreach (var con in input.GetAllConnections())
                {
                    //Connections from constant will never change value
                    //and they will be always be computed before everyting
                    //else. Therefore they will not be a dependency which
                    //is why they can be skipped here.
                    if (consFromConsts.Contains(con))
                    {
                        //Still need to mark as dependency 
                        depOnCons.Add(con);
                        continue;
                    }
                    missingCons.Add(con);
                }
            }

            Dictionary<FIRRTLNode, HashSet<Output>> seenButMissingFirNodeInputs = new Dictionary<FIRRTLNode, HashSet<Output>>();
            Dictionary<Input, HashSet<Output>> seenButMissingModInputCons = new Dictionary<Input, HashSet<Output>>();
            HashSet<FIRRTLNode> finishedNodes = new HashSet<FIRRTLNode>();
            Dictionary<Output, List<Output>> blockedOutputs = new Dictionary<Output, List<Output>>();

            foreach (var computeFirst in outputs.Where(x => x.Node is not Module && (x.Node is not IStatePreserving || !skipStatePre)).Select(x => x.Node).Distinct())
            {
                computeOrder.Add(new Computable(computeFirst));
            }

            Queue<(Output con, Input input)> toTraverse = new Queue<(Output con, Input input)>();
            foreach (var output in outputs)
            {
                AddConnections(toTraverse, output);

                while (toTraverse.Count > 0)
                {
                    var conInput = toTraverse.Dequeue();
                    if (blockedOutputs.TryGetValue(conInput.con, out var blockedOuts))
                    {
                        Debug.Assert(blockedOuts.Distinct().Count() == blockedOuts.Count);
                        foreach (var blocked in blockedOuts)
                        {
                            AddConnections(toTraverse, blocked);
                        }
                        blockedOutputs.Remove(conInput.con);
                    }

                    //Punch through module border to continue search on the other side
                    if (conInput.input.Node is Module mod)
                    {
                        HashSet<Output> missingCons;
                        if (!seenButMissingModInputCons.TryGetValue(conInput.input, out missingCons))
                        {
                            missingCons = new HashSet<Output>();
                            seenButMissingModInputCons.Add(conInput.input, missingCons);

                            AddMissingCons(missingCons, conInput.input);
                        }

                        missingCons.Remove(conInput.con);

                        if (missingCons.Count == 0)
                        {
                            Output inPairedOut = (Output)mod.GetPairedIO(conInput.input);
                            if (ignoreConCondBorders || !HasUnSeenConCond(inPairedOut))
                            {
                                AddConnections(toTraverse, inPairedOut);
                            }
                            else
                            {
                                List<Output> blocked;
                                if (!blockedOutputs.TryGetValue(inPairedOut.GetConditional(), out blocked))
                                {
                                    blocked = new List<Output>();
                                    blockedOutputs.Add(inPairedOut.GetConditional(), blocked);

                                }

                                blocked.Add(inPairedOut);
                            }
                            seenButMissingModInputCons.Remove(conInput.input);
                        }
                    }
                    //Ignore state preserving components as a combinatorial graph
                    //shouldn't cross those
                    else if (conInput.input.Node is IStatePreserving)
                    {
                        if (skipStatePre)
                        {
                            continue;
                        }

                        //Only add outputs once to search
                        if (!finishedNodes.Add(conInput.input.Node))
                        {
                            continue;
                        }

                        var outs = conInput.input.Node.GetOutputs();
                        foreach (Output nodeOutput in outs)
                        {
                            AddConnections(toTraverse, nodeOutput);
                        }
                    }
                    else if (conInput.input.Node is DummySink)
                    {
                        continue;
                    }
                    else
                    {
                        if (finishedNodes.Contains(conInput.input.Node))
                        {
                            continue;
                        }
                        Debug.Assert(conInput.input.Node.GetIO().SelectMany(x => x.Flatten()).Select(x => x.GetConditional()).Where(x => x != null).Distinct().Count() <= 1);
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
                        }

                        missingCons.Remove(conInput.con);

                        //If this graph has provided all inputs to this component then
                        //it can finally compute the component and continue the graph
                        //with the components output
                        if (missingCons.Count == 0)
                        {
                            if (conInput.input.Node is not IStatePreserving)
                            {
                                computeOrder.Add(new Computable(conInput.input.Node));
                            }

                            foreach (var nodeOutput in conInput.input.Node.GetOutputs())
                            {
                                if (ignoreConCondBorders || !HasUnSeenConCond(nodeOutput))
                                {
                                    AddConnections(toTraverse, nodeOutput);
                                }
                                else
                                {
                                    List<Output> blocked;
                                    if (!blockedOutputs.TryGetValue(nodeOutput.GetConditional(), out blocked))
                                    {
                                        blocked = new List<Output>();
                                        blockedOutputs.Add(nodeOutput.GetConditional(), blocked);

                                    }

                                    blocked.Add(nodeOutput);
                                }
                            }



                            finishedNodes.Add(conInput.input.Node);
                            seenButMissingFirNodeInputs.Remove(conInput.input.Node);
                        }
                    }
                }
            }

            List<Output[]> depForOutputs = new List<Output[]>();
            foreach (var node in seenButMissingFirNodeInputs)
            {
                depForOutputs.Add(node.Key.GetOutputs());
            }
            foreach (var input in seenButMissingModInputCons)
            {
                Module mod = (Module)input.Key.Node;
                depForOutputs.Add(new Output[] { (Output)mod.GetPairedIO(input.Key) });
            }
            foreach (var blocked in blockedOutputs)
            {
                foreach (var output in blocked.Value)
                {
                    depForOutputs.Add(new Output[] { output });
                }
            }

            List<Input> endInputs = new List<Input>();
            endInputs.AddRange(seenButMissingModInputCons.Keys);
            endInputs.AddRange(seenButMissingFirNodeInputs.Keys.SelectMany(x => x.GetInputs()));

            foreach (var seenCon in seenCons)
            {
                depOnCons.Remove(seenCon);
            }

            return (new CombComputeNode(outputs, endInputs.ToArray(), computeOrder.ToArray(), seenCons.ToArray()), depForOutputs, depOnCons);
        }
    }
}
