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
        }

        public void ComputeConsts()
        {
            foreach (var node in ConstComputeNodes)
            {
                node.Compute();
            }
        }

        public List<Connection> ComputeGraph()
        {
            foreach (var node in Nodes)
            {
                node.ResetRemainingDependencies();
            }

            List<Connection> updatedCons = new List<Connection>();
            Queue<CombComputeNode> nodesReady = new Queue<CombComputeNode>();
            foreach (var root in RootNodes)
            {
                nodesReady.Enqueue(root);
            }

            while (nodesReady.Count > 0)
            {
                CombComputeNode node = nodesReady.Dequeue();

                updatedCons.AddRange(node.Compute());

                foreach (var maybeReady in node.GetEdges())
                {
                    if (!maybeReady.IsWaitingForDependencies())
                    {
                        nodesReady.Enqueue(maybeReady);
                    }
                }
            }

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
                toMake.Enqueue((Output[])reg.GetOutputs());
            }
            foreach (var mem in module.GetAllNestedNodesOfType<Memory>())
            {
                toMake.Enqueue((Output[])mem.GetOutputs());
            }

            ScalarIO[] rootModuleIncommingPorts = module.GetInternalOutputs();
            if (rootModuleIncommingPorts.Length > 0)
            {
                toMake.Enqueue(rootModuleIncommingPorts.Cast<Output>().ToArray());
            }


            var constNodesAndCons = GetAllCombNodeFromConstValue(module);

            while (toMake.Count > 0)
            {
                var make = toMake.Dequeue();
                var compInfo = MakeCombComputeNode(make, constNodesAndCons.consFromConsts);

                computeNodes.Add(compInfo.node);

                foreach (var depNode in compInfo.depTo)
                {
                    if (alreadyNode.Add(depNode.First()))
                    {
                        toMake.Enqueue(depNode);
                    }
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

            Dictionary<CombComputeNode, List<CombComputeNode>> nodeEdges = new Dictionary<CombComputeNode, List<CombComputeNode>>();
            foreach (var node in allNodes)
            {
                nodeEdges.Add(node, new List<CombComputeNode>());
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
                    inputDeps = firstNodeStart.Node.GetInputs().Cast<Input>().ToArray();
                }

                foreach (var inputDep in inputDeps)
                {
                    if (depToInputs.TryGetValue(inputDep, out var deps))
                    {
                        foreach (var dep in deps)
                        {
                            nodeEdges[dep].Add(node);
                        }
                    }
                }
            }
            foreach (var keyValue in nodeEdges)
            {
                keyValue.Key.AddEdges(keyValue.Value);
            }

            graph.ComputeConsts();

            //Find graph roots so it doesn't have to be done in the future
            graph.ComputeRoots();

            return graph;
        }

        private static (List<CombComputeNode> constNodes, HashSet<Connection> consFromConsts) GetAllCombNodeFromConstValue(Module module)
        {
            List<CombComputeNode> constNodes = new List<CombComputeNode>();
            HashSet<Connection> consFromConsts = new HashSet<Connection>();
            foreach (var constValue in module.GetAllNestedNodesOfType<ConstValue>())
            {
                var compInfo = MakeCombComputeNode(new[] { constValue.Result }, consFromConsts);
                constNodes.Add(compInfo.node);

                foreach (var con in compInfo.node.GetResponsibleConnections())
                {
                    consFromConsts.Add(con);
                }
            }

            return (constNodes, consFromConsts);
        }

        private static (CombComputeNode node, List<Output[]> depTo) MakeCombComputeNode(Output[] outputs, HashSet<Connection> consFromConsts)
        {
            void AddConnections(Queue<(Connection con, Input input)> toTraverse, Output output)
            {
                foreach (var input in output.Con.To)
                {
                    toTraverse.Enqueue((output.Con, input));
                }
            }

            void AddMissingCons(HashSet<Connection> missingCons, Connection[] inputCons)
            {
                foreach (var con in inputCons)
                {
                    //Connections from constant will never change value
                    //and they will be always be computed before everyting
                    //else. Therefore they will not be a dependency which
                    //is why they can be skipped here.
                    if (consFromConsts.Contains(con))
                    {
                        continue;
                    }
                    missingCons.Add(con);
                }
            }

            List<Computable> computeOrder = new List<Computable>();
            HashSet<Connection> seenCons = new HashSet<Connection>();
            Dictionary<FIRRTLNode, HashSet<Connection>> seenButMissingFirNodeInputs = new Dictionary<FIRRTLNode, HashSet<Connection>>();
            Dictionary<Input, HashSet<Connection>> seenButMissingModInputCons = new Dictionary<Input, HashSet<Connection>>();

            foreach (var computeFirst in outputs.Where(x => x.Node is not Module && x.Node is not IStatePreserving).Select(x => x.Node).Distinct())
            {
                computeOrder.Add(new Computable(computeFirst));
            }

            Queue<(Connection con, Input input)> toTraverse = new Queue<(Connection con, Input input)>();
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
                        Connection[] inputCons = conInput.input.GetAllConnections();
                        if (inputCons.Length > 1)
                        {
                            HashSet<Connection> missingCons;
                            if (!seenButMissingModInputCons.TryGetValue(conInput.input, out missingCons))
                            {
                                missingCons = new HashSet<Connection>();
                                seenButMissingModInputCons.Add(conInput.input, missingCons);

                                AddMissingCons(missingCons, inputCons);
                            }

                            missingCons.Remove(conInput.con);

                            if (missingCons.Count == 0)
                            {
                                seenButMissingModInputCons.Remove(conInput.input);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        Output inPairedOut = (Output)mod.GetPairedIO(conInput.input);
                        AddConnections(toTraverse, inPairedOut);
                        continue;
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
                        HashSet<Connection> missingCons;
                        if (!seenButMissingFirNodeInputs.TryGetValue(conInput.input.Node, out missingCons))
                        {
                            missingCons = new HashSet<Connection>();
                            seenButMissingFirNodeInputs.Add(conInput.input.Node, missingCons);

                            ScalarIO[] nodeInputs = conInput.input.Node.GetInputs();
                            foreach (Input input in nodeInputs)
                            {
                                AddMissingCons(missingCons, input.GetAllConnections());
                            }
                        }

                        missingCons.Remove(conInput.con);

                        //If this graph has provided all inputs to this component then
                        //it can finally compute the component and continue the graph
                        //with the components output
                        if (missingCons.Count == 0)
                        {
                            seenButMissingFirNodeInputs.Remove(conInput.input.Node);
                            computeOrder.Add(new Computable(conInput.input.Node));

                            foreach (Output nodeOutput in conInput.input.Node.GetOutputs())
                            {
                                AddConnections(toTraverse, nodeOutput);
                            }
                        }
                    }
                }
            }

            List<Output[]> depForOutputs = new List<Output[]>();
            foreach (var node in seenButMissingFirNodeInputs.Keys)
            {
                depForOutputs.Add(node.GetOutputs().Cast<Output>().ToArray());
            }
            foreach (var input in seenButMissingModInputCons.Keys)
            {
                Module mod = (Module)input.Node;
                depForOutputs.Add(new Output[] { (Output)mod.GetPairedIO(input) });
            }

            return (new CombComputeNode(outputs, seenButMissingModInputCons.Keys.ToArray(), computeOrder.ToArray(), seenCons.ToArray()), depForOutputs);
        }
    }
}
