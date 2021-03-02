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
                    Module mod = Modules.First(x => x.Name == scope.Name);
                    Connection con = mod.GetConnection(varValue.Variable.Scopes.AsSpan().Slice(1), varValue.Variable.Reference);

                    if (con.Value.UpdateValue(varValue))
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
