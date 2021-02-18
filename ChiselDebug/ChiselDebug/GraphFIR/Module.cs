using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR
{
    public class Module : FIRRTLContainer
    {
        public readonly string Name;
        public List<FIRRTLNode> Nodes = new List<FIRRTLNode>();
        public List<Module> Modules = new List<Module>();

        public Module(string name)
        {
            this.Name = name;
        }

        public void AddNode(FIRRTLNode node)
        {
            Nodes.Add(node);
        }

        public List<Connection> GetAllModuleConnections()
        {
            List<Connection> connestions = new List<Connection>();
            foreach (var output in InternalOutputs)
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

        public Input GetInternalInput(string name)
        {
            return InternalInputs.Find(x => x.Name == name);
        }

        public FIRRTLNode[] GetAllNodes()
        {
            return Nodes.ToArray();
        }
    }
}
