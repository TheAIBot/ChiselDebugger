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
    }
}
