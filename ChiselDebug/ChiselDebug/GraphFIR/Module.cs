﻿using FIRRTL;
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
        private readonly Dictionary<string, FIRIO> NameToIO = new Dictionary<string, FIRIO>();
        

        public Module(string name)
        {
            this.Name = name;
        }

        public void AddNode(FIRRTLNode node)
        {
            Nodes.Add(node);
            foreach (var input in node.GetInputs())
            {
                if (!input.IsAnonymous)
                {
                    NameToIO.Add(input.Name, input);
                }
            }
            foreach (var output in node.GetOutputs())
            {
                if (!output.IsAnonymous)
                {
                    NameToIO.Add(output.Name, output);
                }
            }
        }

        public void AddIORename(string name, FIRIO io)
        {
            NameToIO.Add(name, io);
        }

        public override IContainerIO GetIO(string ioName)
        {
            if (NameToIO.TryGetValue(ioName, out FIRIO innerIO))
            {
                return innerIO;
            }

            if (InternalIO.TryGetValue(ioName, out FIRIO io))
            {
                return io;
            }

            throw new Exception($"Failed to find io. IO name: {ioName}");
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
