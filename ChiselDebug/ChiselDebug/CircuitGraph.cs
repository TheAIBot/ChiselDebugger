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
                    List<string> magic = varValue.Variable.Scopes.Select(x => x.Name).ToList();
                    magic.Add(varValue.Variable.Reference);
                    Connection con = ((ScalarIO)mod.GetIO(magic.ToArray().AsSpan(1))).Con;

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
