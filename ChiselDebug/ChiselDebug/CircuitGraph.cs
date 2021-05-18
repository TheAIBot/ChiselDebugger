using ChiselDebug.CombGraph;
using ChiselDebug.CombGraph.CombGraphOptimizations;
using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using VCDReader;

namespace ChiselDebug
{

    public class CircuitGraph
    {
        public readonly string Name;
        public readonly Module MainModule;
        public readonly CombComputeGraph ComputeGraph;
        public readonly CombComputeGraph OptimizedComputegraph;
        private readonly Dictionary<VarDef, ScalarIO> VarDefToCon = new Dictionary<VarDef, ScalarIO>();
        private readonly HashSet<Output> ComputeAllowsUpdate = new HashSet<Output>();
        private readonly Dictionary<string, List<Output>> VarDefIDToCon = new Dictionary<string, List<Output>>();

        public CircuitGraph(string name, Module mainModule)
        {
            this.Name = name;
            this.MainModule = mainModule;
            this.ComputeGraph = CombComputeGraph.MakeMonoGraph(MainModule);
            ComputeGraph.InferTypes();
            this.OptimizedComputegraph = ComputeOptimizer.Optimize(ComputeGraph);

            foreach (var rootStart in ComputeGraph.GetAllRootSources())
            {
                if (rootStart.Node is not ConstValue)
                {
                    ComputeAllowsUpdate.Add(rootStart);
                }
            }
        }

        private bool TryFindVerilogMemPort(string ioName, MemoryIO memory, out IContainerIO foundIO)
        {
            foreach (MemPort port in memory.GetIOInOrder())
            {
                if (ioName.Contains(port.Name))
                {
                    int portNameStart = ioName.IndexOf(port.Name);
                    string remaining = ioName.Substring(0, portNameStart);
                    string portPath = ioName.Substring(portNameStart);

                    if (TryFindIO(portPath, memory, false, out foundIO))
                    {
                        if (foundIO is ScalarIO)
                        {
                            return true;
                        }

                        if (TryFindIO(remaining, foundIO, false, out var fullFoundIO))
                        {
                            if (fullFoundIO is not ScalarIO)
                            {
                                foundIO = null;
                                return false;
                            }

                            foundIO = fullFoundIO;
                            return true;
                        }
                    }
                }
            }

            foundIO = null;
            return false;
        }

        private bool TryFindIO(string ioName, IContainerIO container, bool isVerilogVCD, out IContainerIO foundIO)
        {
            string remainingName = ioName;
            string searchName = remainingName;
            while (true)
            {
                if (container.TryGetIO(searchName, true, out foundIO))
                {
                    container = foundIO;

                    //Remove found name from what still needs to be found
                    remainingName = remainingName.Substring(searchName.Length);

                    //Also remove _ from the name as firrtl uses it as an io name
                    //separator
                    if (remainingName.Length > 0)
                    {
                        remainingName = remainingName.Substring(1);
                    }
                    if (remainingName.Length == 0)
                    {
                        return true;
                    }

                    //Verilog represents memport names in a wierd way in the vcd
                    //which is why that case has to be specially handled
                    if (isVerilogVCD && container is MemoryIO memory && memory.GetDataType() is AggregateIO)
                    {
                        if (TryFindVerilogMemPort(remainingName, memory, out foundIO))
                        {
                            return true;
                        }

                        foundIO = null;
                        return false;
                    }

                    if (container is ScalarIO)
                    {
                        foundIO = null;
                        return false;
                    }

                    if (container is DuplexIO duplex)
                    {
                        if (remainingName.EndsWith("/in"))
                        {
                            container = duplex.GetInput();
                            remainingName = remainingName.Substring(0, remainingName.Length - "/in".Length);
                        }
                        else
                        {
                            container = duplex.GetOutput();
                        }
                    }

                    searchName = remainingName;
                }
                else
                {
                    int _index = searchName.LastIndexOf('_');
                    if (_index == -1)
                    {
                        foundIO = null;
                        return false;
                    }

                    searchName = searchName.Substring(0, _index);
                }
            }
        }

        public ScalarIO GetConnection(VarDef variable, bool isVerilogVCD)
        {
            if (VarDefToCon.TryGetValue(variable, out ScalarIO con))
            {
                return con;
            }

            //VCD adds wires that contain the previous clock value.
            //These wires are not part of the circuit and therefore
            //there is no circuit state for them to update.
            if (variable.Reference.EndsWith("/prev"))
            {
                VarDefToCon.Add(variable, null);
                return null;
            }

            //Verilog creatred vcd adds a top level module to the module path which has to be
            //ignore as it's not part of the firrtl code. Apart from that, the first module
            //should always be skipped as it's the root module that is being searched from.
            int modulesSkipped = isVerilogVCD ? 2 : 1;
            string[] modulePath = variable.Scopes.Skip(modulesSkipped).Select(x => x.Name).ToArray();
            IContainerIO moduleIO = ((IContainerIO)MainModule).GetIO(modulePath, true);

            IContainerIO ioLink = null;
            bool foundIO = TryFindIO(variable.Reference, moduleIO, isVerilogVCD, out ioLink);

            if (!foundIO)
            {
                VarDefToCon.Add(variable, null);
                return null;
            }

            //Because a register is Duplex, its io is contained
            //within a bundle so it's necessary to do this extra
            //step to get the correct io out.
            if (ioLink is DuplexIO regIO)
            {
                ioLink = regIO.GetIO(variable.Reference);
            }

            //Apparently if a module contains an instance of another module
            //then it will also have a wire with the instance name in the vcd
            //file. Ends up with a bundle when this happens so just ignore the
            //change.
            if (ioLink is Module)
            {
                VarDefToCon.Add(variable, null);
                return null;
            }
            if (ioLink is MemoryIO)
            {
                VarDefToCon.Add(variable, null);
                return null;
            }
            if (ioLink is MemPort)
            {
                VarDefToCon.Add(variable, null);
                return null;
            }

            if (ioLink is ScalarIO scalar)
            {
                VarDefToCon.Add(variable, scalar);
                return scalar;
            }

            throw new Exception("No");
        }

        public void SetState(CircuitState state, bool isVerilogVCD)
        {
            if (VarDefToCon.Count == 0)
            {
                foreach (BinaryVarValue varValue in state.VariableValues.Values)
                {
                    foreach (var variable in varValue.Variables)
                    {
                        Output con = GetConnection(variable, isVerilogVCD) as Output;
                        if (con == null)
                        {
                            continue;
                        }

                        if (!ComputeAllowsUpdate.Contains(con))
                        {
                            continue;
                        }

                        if (!con.Value.IsInitialized())
                        {
                            continue;
                        }

                        var varCopy = varValue;
                        con.Value.UpdateValue(ref varCopy);

                        List<Output> idToScalars;
                        if (!VarDefIDToCon.TryGetValue(variable.ID, out idToScalars))
                        {
                            idToScalars = new List<Output>();
                            VarDefIDToCon.Add(variable.ID, idToScalars);
                        }

                        idToScalars.Add(con);
                    }
                }
            }
            else
            {
                foreach (var idOutputs in VarDefIDToCon)
                {
                    BinaryVarValue binValue = state.VariableValues[idOutputs.Key];
                    foreach (var output in idOutputs.Value)
                    {
                        output.Value.UpdateValue(ref binValue);
                    }
                }
            }
        }

        public List<Output> ComputeRemainingGraph()
        {
            return ComputeGraph.ComputeAndGetChanged();
        }

        public void ComputeRemainingGraphFast()
        {
            OptimizedComputegraph.ComputeFast();
        }

        public string StateToString()
        {
            StringBuilder builder = new StringBuilder();
            ModuleStateToString(builder, MainModule, string.Empty);

            return builder.ToString();
        }

        private void ModuleStateToString(StringBuilder builder, Module mod, string indentation)
        {
            if (mod.Name == "Queue_5")
            {

            }
            builder.Append(indentation);
            builder.Append(' ');
            builder.AppendLine(mod.Name);

            foreach (var io in mod.GetInternalIO())
            {
                foreach (var scalar in io.Flatten())
                {
                    if (!scalar.IsAnonymous && scalar.Value.IsInitialized())
                    {
                        builder.Append(indentation + '\t');
                        builder.Append(scalar.GetFullName());
                        builder.Append(" = ");
                        builder.AppendLine(scalar.Value.ToBinaryString());
                    }
                }
            }

            NodesStateToString(builder, mod.GetAllNodes(), indentation);
        }

        private void NodesStateToString(StringBuilder builder, FIRRTLNode[] nodes, string indentation)
        {
            indentation = indentation + '\t';

            foreach (var node in nodes)
            {
                if (node is Module childMod)
                {
                    ModuleStateToString(builder, childMod, indentation);
                }
                else if (node is Conditional cond)
                {
                    foreach (var condMod in cond.CondMods)
                    {
                        builder.Append(indentation);
                        builder.AppendLine("when");
                        NodesStateToString(builder, condMod.Mod.GetAllNodes(), indentation);
                    }
                }
                else
                {
                    foreach (ScalarIO scalar in node.GetIO().SelectMany(x => x.Flatten()))
                    {
                        if (!scalar.IsAnonymous)
                        {
                            string ioName = scalar.GetFullName();
                            if (node is Memory mem)
                            {
                                ioName = mem.Name + "." + ioName;
                            }
                            if (node is Register reg)
                            {
                                ioName = reg.Name + "." + ioName;
                            }

                            builder.Append(indentation);
                            builder.Append(ioName);
                            builder.Append(" = ");
                            if (scalar.Value.IsInitialized())
                            {
                                builder.AppendLine(scalar.Value.ToBinaryString());
                            }
                            else
                            {
                                builder.AppendLine("???");
                            }
                        }
                    }
                }
            }
        }
    }
}
