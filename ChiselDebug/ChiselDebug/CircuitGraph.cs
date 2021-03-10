using ChiselDebug.GraphFIR;
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
        public readonly List<Module> Modules = new List<Module>();

        public CircuitGraph(string name, List<Module> modules)
        {
            this.Name = name;
            this.Modules = modules;
        }

        public void InferTypes()
        {
            foreach (var mod in Modules)
            {
                mod.InferType();
            }
        }

        public List<Connection> SetState(CircuitState state)
        {
            List<Connection> consWithChanges = new List<Connection>();
            foreach (BinaryVarValue varValue in state.VariableValues.Values)
            {
                Scope scope = varValue.Variable.Scopes[0];
                if (scope.Type == ScopeType.Module)
                {
                    IContainerIO mod = Modules.First(x => x.Name == scope.Name);
                    string[] modulePath = varValue.Variable.Scopes.Skip(1).Select(x => x.Name).ToArray();
                    IContainerIO moduleIO = mod.GetIO(modulePath, true);
                    IContainerIO ioLink = moduleIO.GetIO(varValue.Variable.Reference);

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
