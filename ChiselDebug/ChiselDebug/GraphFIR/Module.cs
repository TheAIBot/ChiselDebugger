using ChiselDebug.GraphFIR.IO;
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
        private readonly Dictionary<string, FIRIO> NameToIO = new Dictionary<string, FIRIO>();
        private readonly Dictionary<IOBundle, Module> BundleToModule = new Dictionary<IOBundle, Module>();
        

        public Module(string name)
        {
            this.Name = name;
        }

        public void AddNode(FIRRTLNode node)
        {
            Nodes.Add(node);
            foreach (var io in node.GetIO())
            {
                if (!io.IsAnonymous)
                {
                    NameToIO.Add(io.Name, io);
                }
            }
        }

        public void AddRegister(Register reg)
        {
            Nodes.Add(reg);

            DuplexIO regIO = reg.GetAsDuplex();
            NameToIO.Add(regIO.GetInput().Name, regIO);
            NameToIO.Add(regIO.GetOutput().Name, regIO);
        }

        public void AddWire(Wire wire)
        {
            Nodes.Add(wire);

            DuplexIO wireIO = wire.GetAsDuplex();
            NameToIO.Add(wireIO.Name, wireIO);
        }

        public void AddModule(Module mod, string bundleName)
        {
            Nodes.Add(mod);

            IOBundle bundle = new IOBundle(bundleName, mod.ExternalIO.Values.ToList(), false);
            NameToIO.Add(bundleName, bundle);
            BundleToModule.Add(bundle, mod);
        }

        public void AddMemory(Memory mem)
        {
            Nodes.Add(mem);

            IOBundle memIO = mem.GetIOAsBundle();
            NameToIO.Add(memIO.Name, memIO);
        }

        public void AddIORename(string name, FIRIO io)
        {
            NameToIO.Add(name, io);
        }

        public void AddMemoryPort(Memory mem, MemPort port)
        {
            IOBundle memIO = mem.GetIOAsBundle();
            NameToIO[memIO.Name] = memIO;

            NameToIO.Add(port.Name, port);
        }

        public Memory GetMemory(string name)
        {
            return (Memory)Nodes.Single(x => x is Memory mem && mem.Name == name);
        }

        public void AddConditional(Conditional cond)
        {
            Nodes.Add(cond);
        }

        public override bool TryGetIO(string ioName, bool modulesOnly, out IContainerIO container)
        {
            if (NameToIO.TryGetValue(ioName, out FIRIO innerIO))
            {
                if (modulesOnly &&
                    innerIO is IOBundle bundle &&
                    BundleToModule.TryGetValue(bundle, out var mod))
                {
                    container = mod;
                    return true;
                }
                else
                {
                    container = innerIO;
                    return true;
                }
            }

            if (InternalIO.TryGetValue(ioName, out FIRIO io))
            {
                container = io;
                return true;
            }

            container = null;
            return false;
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

        public FIRRTLNode[] GetAllNodesIncludeModule()
        {
            return GetAllNodes().Append(this).ToArray();
        }

        public override FIRIO[] GetAllIOOrdered()
        {
            List<FIRIO> allOrdered = new List<FIRIO>();
            allOrdered.AddRange(base.GetAllIOOrdered());
            allOrdered.AddRange(Nodes.SelectMany(x => x.GetIO().SelectMany(x => x.Flatten())));

            return allOrdered.ToArray();
        }

        internal void RemoveAllWires()
        {
            for (int i = Nodes.Count - 1; i >= 0 ; i--)
            {
                FIRRTLNode node = Nodes[i];
                if (node is Wire wire)
                {
                    wire.BypassWireIO();
                    Nodes.RemoveAt(i);
                    NameToIO.Remove(wire.Name);
                }
                else if (node is Module mod)
                {
                    mod.RemoveAllWires();
                }
            }
        }

        internal void CorrectIO()
        {
            foreach (var node in Nodes)
            {
                if (node is Module mod)
                {
                    mod.CorrectIO();
                }
                else
                {
                    foreach (Input input in node.GetInputs())
                    {
                        input.MakeSinkOnly();
                    }
                }
            }

            foreach (Input input in GetInternalInputs())
            {
                input.MakeSinkOnly();
            }
        }

        internal void CopyInternalAsExternalIO(Module mod)
        {
            foreach (var inIO in GetInternalIO())
            {
                var copy = inIO.Flip(null);
                IOHelper.OneWayOnlyConnect(inIO, copy);

                mod.AddExternalIO(copy);
            }

            foreach (var nodeIO in NameToIO)
            {
                FIRIO copy = nodeIO.Value.Flip(null);
                copy.SetName(nodeIO.Key);
                IOHelper.OneWayOnlyConnect(nodeIO.Value, copy);

                mod.AddExternalIO(copy);
            }
        }

        internal void ExternalConnectUsedIO(Module parentMod)
        {
            var extKeyVals = ExternalIO.ToArray();
            var intKeyVals = InternalIO.ToArray();

            for (int i = 0; i < extKeyVals.Length; i++)
            {
                var extKeyVal = extKeyVals[i];
                var intKeyVal = intKeyVals[i];

                ScalarIO[] extFlat = extKeyVal.Value.Flatten().ToArray();
                ScalarIO[] intFlat = intKeyVal.Value.Flatten().ToArray();

                for (int x = 0; x < extFlat.Length; x++)
                {
                    if (intFlat[x] is Input intIn && intIn.IsConnectedToAnything())
                    {
                        FIRIO parentIO = (FIRIO)parentMod.GetIO(intKeyVal.Key);
                        ScalarIO[] parentIOFlat = parentIO.Flatten().ToArray();

                        ((Output)extFlat[x]).ConnectToInput(parentIOFlat[x]);
                    }
                }
            }
        }

        internal void DisconnectUnusedIO()
        {
            var extKeyVals = ExternalIO.ToArray();
            var intKeyVals = InternalIO.ToArray();

            for (int i = 0; i < extKeyVals.Length; i++)
            {
                var extKeyVal = extKeyVals[i];
                var intKeyVal = intKeyVals[i];

                ScalarIO[] extFlat = extKeyVal.Value.Flatten().ToArray();
                ScalarIO[] intFlat = intKeyVal.Value.Flatten().ToArray();

                for (int x = 0; x < extFlat.Length; x++)
                {
                    if (intFlat[x] is Output intOut && !intOut.IsConnectedToAnything())
                    {
                        ((Input)extFlat[x]).Disconnect();
                    }
                }
            }
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
