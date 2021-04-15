using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VCDReader;

namespace ChiselDebug
{
    public class ComputeGraph
    {

    }

    public class CombComputeNode
    {
        private readonly List<FIRRTLNode> ComputeOrder;
        private readonly List<CombComputeNode> OutgoingEdges = new List<CombComputeNode>();
        private int TotalComputeDependencies = 0;
        private int RemainingComputeDependencies = 0;

        public CombComputeNode(List<FIRRTLNode> computeOrder)
        {
            this.ComputeOrder = computeOrder;
        }

        public void AddEdgeTo(CombComputeNode edgeTo)
        {
            OutgoingEdges.Add(edgeTo);
            edgeTo.AddComputeDependency();
        }

        private void AddComputeDependency()
        {
            TotalComputeDependencies++;
        }

        public void ResetRemainingDependencies()
        {
            RemainingComputeDependencies = TotalComputeDependencies;
        }
    }

    public class CircuitGraph
    {
        public readonly string Name;
        public readonly Module MainModule;

        public CircuitGraph(string name, Module mainModule)
        {
            this.Name = name;
            this.MainModule = mainModule;
        }

        public void InferTypes()
        {
            MainModule.InferType();
        }

        public List<Connection> SetState(CircuitState state)
        {
            List<Connection> consWithChanges = new List<Connection>();
            foreach (BinaryVarValue varValue in state.VariableValues.Values)
            {
                //VCD adds wires that contain the previous clock value.
                //These wires are not part of the circuit and therefore
                //there is no circuit state for them to update.
                if (varValue.Variable.Reference.EndsWith("/prev"))
                {
                    continue;
                }

                Scope scope = varValue.Variable.Scopes[0];
                if (scope.Type == ScopeType.Module)
                {
                    string[] modulePath = varValue.Variable.Scopes.Skip(1).Select(x => x.Name).ToArray();
                    IContainerIO moduleIO = ((IContainerIO)MainModule).GetIO(modulePath, true);

                    IContainerIO ioLink = null;
                    bool foundIO = moduleIO.TryGetIO(varValue.Variable.Reference, false, out ioLink);

                    //Wierd wires are added to memory ports that are not part of the
                    //firrtl code. Just ignore those wires.
                    if (!foundIO && moduleIO is MemPort)
                    {
                        continue;
                    }

                    //Because a register is Duplex, its io is contained
                    //within a bundle so it's necessary to do this extra
                    //step to get the correct io out.
                    if (ioLink is DuplexIO regIO)
                    {
                        ioLink = regIO.GetIO(varValue.Variable.Reference);
                    }

                    //Apparently if a module contains an instance of another module
                    //then it will also have a wire with the instance name in the vcd
                    //file. Ends up with a bundle when this happens so just ignore the
                    //change.
                    if (ioLink is IOBundle)
                    {
                        continue;
                    }
                    Connection con = ((ScalarIO)ioLink).Con;

                    if (con != null && con.Value.UpdateValue(varValue))
                    {
                        consWithChanges.Add(con);
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return consWithChanges;
        }

        private void feis()
        {
            HashSet<Connection> mustBeSet = new HashSet<Connection>();
        }

        private void MakeCombComputeGraph()
        {
            HashSet<Connection> hasValue = new HashSet<Connection>();
            Dictionary<Connection, HashSet<Connection>>
        }

        private (CombComputeNode node, List<FIRRTLNode> depTo) MakeCombComputeNode(Output[] outputs)
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
                            Input[] nodeInputs = (Input[])conInput.input.Node.GetInputs();
                            missingCons = new HashSet<Connection>(nodeInputs.SelectMany(x => x.GetAllConnections()));
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

            return (new CombComputeNode(computeOrder), seenButMissingInputs.Keys.ToList());
        }
    }
}
