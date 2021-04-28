using ChiselDebug.CombGraph;
using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VCDReader;

namespace ChiselDebug
{

    public class CircuitGraph
    {
        public readonly string Name;
        public readonly Module MainModule;
        public readonly CombComputeGraph ComputeGraph;
        public readonly Dictionary<VarDef, Output> VarDefToCon = new Dictionary<VarDef, Output>();
        public readonly HashSet<Output> ComputeAllowsUpdate = new HashSet<Output>();

        public CircuitGraph(string name, Module mainModule)
        {
            this.Name = name;
            this.MainModule = mainModule;
            this.ComputeGraph = CombComputeGraph.MakeMonoGraph(MainModule);
            ComputeGraph.InferTypes();

            foreach (var root in ComputeGraph.GetRootNodes())
            {
                foreach (var rootStart in root.GetStartOutputs())
                {
                    ComputeAllowsUpdate.Add(rootStart);
                }
            }
        }

        private bool TryFindIO(string ioName, IContainerIO container, out IContainerIO foundIO)
        {
            string remainingName = ioName;
            string searchName = remainingName;
            while (true)
            {
                if (container.TryGetIO(searchName, false, out foundIO))
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

        public Output GetConnection(VarDef variable, bool isVerilogVCD)
        {
            if (VarDefToCon.TryGetValue(variable, out Output con))
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
            bool foundIO = TryFindIO(variable.Reference, moduleIO, out ioLink);

            if (!foundIO)
            {
                return null;
                ////Wierd wires are added to memory ports that are not part of the
                ////firrtl code. Just ignore those wires.
                //if (moduleIO is MemPort)
                //{
                //    VarDefToCon.Add(variable, null);
                //    return null;
                //}

                ////FIRRTL lowering creates temporaries that are not part of high
                ////level FIRRTL code which is why they can't be found
                //if (variable.Reference.StartsWith("_GEN"))
                //{
                //    VarDefToCon.Add(variable, null);
                //    return null;
                //}

                //throw new Exception($"Failed to find vcd io in circuit. IO name: {variable.Reference}");
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
            if (ioLink is IOBundle)
            {
                VarDefToCon.Add(variable, null);
                return null;
            }

            if (ioLink is Output output)
            {
                VarDefToCon.Add(variable, output);
                return output;
            }

            return null;
        }

        public List<Output> SetState(CircuitState state, bool isVerilogVCD)
        {

            foreach (BinaryVarValue varValue in state.VariableValues.Values)
            {
                foreach (var variable in varValue.Variables)
                {
                    Output con = GetConnection(variable, isVerilogVCD);
                    if (con == null)
                    {
                        continue;
                    }

                    if (!ComputeAllowsUpdate.Contains(con))
                    {
                        continue;
                    }

                    if (con.Value.GetValue() == null)
                    {
                        continue;
                    }
                    con.Value.UpdateValue(varValue);
                }
            }

            return ComputeGraph.Compute();
        }
    }
}
