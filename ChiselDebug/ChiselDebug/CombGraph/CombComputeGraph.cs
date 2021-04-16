using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ChiselDebug.CombGraph
{
    public class CombComputeGraph
    {
        private readonly Dictionary<FIRRTLNode, CombComputeNode> Nodes = new Dictionary<FIRRTLNode, CombComputeNode>();

        public void AddNode(FIRRTLNode firNode, CombComputeNode combNode)
        {
            Nodes.Add(firNode, combNode);
        }

        public void AddEdge(FIRRTLNode from, FIRRTLNode to)
        {
            Nodes[from].AddEdgeTo(Nodes[to]);
        }

        public void ComputeRoots()
        {

        }

        public void ComputeGraph()
        {

        }

        public void Reset()
        {
            foreach (var node in Nodes.Values)
            {
                node.ResetRemainingDependencies();
            }
        }

        public CombComputeNode[] GetAllNodes()
        {
            return Nodes.Values.ToArray();
        }

        public static CombComputeGraph MakeGraph(Module module)
        {
            List<(FIRRTLNode root, CombComputeNode node, List<FIRRTLNode> depTo)> computeNodes = new List<(FIRRTLNode root, CombComputeNode node, List<FIRRTLNode> depTo)>();
            HashSet<FIRRTLNode> alreadyNode = new HashSet<FIRRTLNode>();
            Queue<(FIRRTLNode root, Output[] outputs)> toMake = new Queue<(FIRRTLNode root, Output[] outputs)>();

            foreach (var reg in module.GetAllNestedNodesOfType<Register>())
            {
                toMake.Enqueue((reg, (Output[])reg.GetOutputs()));
            }
            foreach (var mem in module.GetAllNestedNodesOfType<Memory>())
            {
                toMake.Enqueue((mem, (Output[])mem.GetOutputs()));
            }

            HashSet<Connection> consFromConsts = GetAllCombNodeFromConstValue(module);

            while (toMake.Count > 0)
            {
                var make = toMake.Dequeue();
                var compInfo = MakeCombComputeNode(make.outputs, consFromConsts);

                computeNodes.Add((make.root, compInfo.node, compInfo.depTo));

                foreach (var depNode in compInfo.depTo)
                {
                    if (alreadyNode.Add(depNode))
                    {
                        toMake.Enqueue((depNode, depNode.GetOutputs().Cast<Output>().ToArray()));
                    }
                }
            }

            CombComputeGraph graph = new CombComputeGraph();
            foreach (var node in computeNodes)
            {
                graph.AddNode(node.root, node.node);
            }

            foreach (var node in computeNodes)
            {
                foreach (var depTo in node.depTo)
                {
                    graph.AddEdge(node.root, depTo);
                }
            }

            //Find graph roots so it doesn't have to be done in the future
            graph.ComputeRoots();

            return graph;
        }

        private static HashSet<Connection> GetAllCombNodeFromConstValue(Module module)
        {
            HashSet<Connection> consFromConsts = new HashSet<Connection>();
            foreach (var constValue in module.GetAllNestedNodesOfType<ConstValue>())
            {
                var compInfo = MakeCombComputeNode(new[] { constValue.Result }, consFromConsts);

                foreach (var con in compInfo.node.GetResponsibleConnections())
                {
                    consFromConsts.Add(con);
                }
            }

            foreach (var con in consFromConsts)
            {
                con.Value.SetValueString("const??");
            }

            return consFromConsts;
        }

        private static (CombComputeNode node, List<FIRRTLNode> depTo) MakeCombComputeNode(Output[] outputs, HashSet<Connection> consFromConsts)
        {
            void AddConnections(Queue<(Connection con, Input input)> toTraverse, Output output)
            {
                foreach (var input in output.Con.To)
                {
                    toTraverse.Enqueue((output.Con, input));
                }
            }

            List<FIRRTLNode> computeOrder = new List<FIRRTLNode>();
            Dictionary<FIRRTLNode, HashSet<Connection>> seenButMissingInputs = new Dictionary<FIRRTLNode, HashSet<Connection>>();
            HashSet<Connection> seenCons = new HashSet<Connection>();

            Queue<(Connection con, Input input)> toTraverse = new Queue<(Connection con, Input input)>();
            foreach (var output in outputs)
            {
                AddConnections(toTraverse, output);

                while (toTraverse.Count > 0)
                {
                    var conInput = toTraverse.Dequeue();
                    seenCons.Add(conInput.con);

                    //Punch through module border to continue search on the other side
                    if (conInput.input.Node is Module mod)
                    {
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
                    else
                    {
                        HashSet<Connection> missingCons;
                        if (!seenButMissingInputs.TryGetValue(conInput.input.Node, out missingCons))
                        {
                            missingCons = new HashSet<Connection>();

                            ScalarIO[] nodeInputs = conInput.input.Node.GetInputs();
                            foreach (Input input in nodeInputs)
                            {
                                foreach (var con in input.GetAllConnections())
                                {
                                    if (consFromConsts.Contains(con))
                                    {
                                        continue;
                                    }
                                    missingCons.Add(con);
                                }
                            }
                            // nodeInputs.SelectMany(x => ((Input)x).GetAllConnections()));

                            seenButMissingInputs.Add(conInput.input.Node, missingCons);
                        }

                        missingCons.Remove(conInput.con);

                        //If this graph has provided all inputs to this component then
                        //it can finally compute the component and continue the graph
                        //with the components output
                        if (missingCons.Count == 0)
                        {
                            seenButMissingInputs.Remove(conInput.input.Node);
                            computeOrder.Add(conInput.input.Node);

                            foreach (Output nodeOutput in conInput.input.Node.GetOutputs())
                            {
                                AddConnections(toTraverse, nodeOutput);
                            }
                        }
                    }
                }
            }

            return (new CombComputeNode(computeOrder, seenCons.ToList()), seenButMissingInputs.Keys.ToList());
        }
    }
}
