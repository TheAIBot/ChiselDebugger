using ChiselDebug.GraphFIR;
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

            CombComputeGraph graph = new CombComputeGraph();
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

        private static (CombComputeNode node, List<Output[]> depTo, HashSet<Output> depOnCons) MakeCombComputeNode(Output[] outputs, HashSet<Output> consFromConsts, bool ignoreConCondBorders)
        {
            HashSet<Output> depOnCons = new HashSet<Output>();
            HashSet<Output> seenCons = new HashSet<Output>();

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

            List<Computable> computeOrder = new List<Computable>();
            Dictionary<FIRRTLNode, HashSet<Output>> seenButMissingFirNodeInputs = new Dictionary<FIRRTLNode, HashSet<Output>>();
            Dictionary<Input, HashSet<Output>> seenButMissingModInputCons = new Dictionary<Input, HashSet<Output>>();

            foreach (var computeFirst in outputs.Where(x => x.Node is not Module && x.Node is not IStatePreserving).Select(x => x.Node).Distinct())
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
                    if (seenCons.Add(conInput.con))
                    {
                        computeOrder.Add(new Computable(conInput.con));
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
                                seenButMissingModInputCons.Remove(conInput.input);
                            }
                        }
                    }
                    //Ignore state preserving components as a combinatorial graph
                    //shouldn't cross those
                    else if (conInput.input.Node is Memory ||
                             conInput.input.Node is Register)
                    {
                        continue;
                    }
                    else if (conInput.input.Node is DummySink)
                    {
                        continue;
                    }
                    else
                    {
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
                            Output[] outfewpduts = conInput.input.Node.GetOutputs();
                            if (ignoreConCondBorders || outfewpduts.All(x => !HasUnSeenConCond(x)))
                            {
                                seenButMissingFirNodeInputs.Remove(conInput.input.Node);
                                computeOrder.Add(new Computable(conInput.input.Node));

                                foreach (Output nodeOutput in outfewpduts)
                                {
                                    AddConnections(toTraverse, nodeOutput);
                                }
                            }

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

            List<Input> endInputs = new List<Input>();
            endInputs.AddRange(seenButMissingModInputCons.Keys);
            endInputs.AddRange(seenButMissingFirNodeInputs.Keys.SelectMany(x => x.GetInputs()));

            return (new CombComputeNode(outputs, endInputs.ToArray(), computeOrder.ToArray(), seenCons.ToArray()), depForOutputs, depOnCons);
        }
    }
}
