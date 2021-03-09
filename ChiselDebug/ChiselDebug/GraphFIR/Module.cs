using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using VCDReader;

namespace ChiselDebug.GraphFIR
{
    public class Module : FIRRTLContainer
    {
        public readonly string Name;
        public List<FIRRTLNode> Nodes = new List<FIRRTLNode>();
        private Dictionary<string, Connection> NameToConnection = new Dictionary<string, Connection>();
        

        public Module(string name)
        {
            this.Name = name;
        }

        public void AddNode(FIRRTLNode node)
        {
            Nodes.Add(node);
        }

        public void AddOutputRename(string name, Output output)
        {
            NameToConnection.Add(name, output.Con);
        }

        public void FinishModuleSetup()
        {
            foreach (var node in Nodes)
            {
                if (node is FIRRTLPrimOP prim)
                {
                    NameToConnection.Add(prim.Result.Name, prim.Result.Con);
                }
                else
                {
                    foreach (var output in node.GetOutputs())
                    {
                        NameToConnection.Add(output.Name, output.Con);
                    }
                }
            }

            foreach (var output in GetOutputs())
            {
                NameToConnection.Add(output.Name, output.Con);
            }
            foreach (var output in GetInternalOutputs())
            {
                NameToConnection.Add(output.Name, output.Con);
            }
        }

        public Connection GetConnection(Span<Scope> scopes, string connectionName)
        {
            if (scopes.IsEmpty)
            {
                return NameToConnection[connectionName];
            }
            else
            {
                Scope scope = scopes[0];
                if (scope.Type == ScopeType.Module)
                {
                    Module mod = (Module)Nodes.First(x => x is Module mod && mod.Name == scope.Name);
                    return mod.GetConnection(scopes.Slice(1), connectionName);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        public List<Connection> GetAllModuleConnections()
        {
            List<Connection> connestions = new List<Connection>();
            foreach (var output in GetInternalOutputs())
            {
                if (output.Con.IsUsed())
                {
                    connestions.Add(output.Con);
                }
            }

            foreach (var node in Nodes)
            {
                foreach (var output in node.GetOutputs())
                {
                    if (output.Con.IsUsed())
                    {
                        connestions.Add(output.Con);
                    }
                }
            }

            return connestions;
        }

        public FIRRTLNode[] GetAllNodes()
        {
            return Nodes.ToArray();
        }

        public override void InferType()
        {
            foreach (var node in Nodes)
            {
                node.InferType();
            }
        }
    }
}
