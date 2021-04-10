using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly Dictionary<Input, DuplexIO> DuplexOutputWires = new Dictionary<Input, DuplexIO>();
        

        public Module(string name, FirrtlNode defNode) : base(defNode)
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

            IOBundle bundle = new IOBundle(mod, bundleName, mod.ExternalIO.Values.ToList(), false);
            NameToIO.Add(bundleName, bundle);
            BundleToModule.Add(bundle, mod);
        }

        public void AddMemory(Memory mem)
        {
            Nodes.Add(mem);

            MemoryIO memIO = mem.GetIOAsBundle();
            NameToIO.Add(memIO.Name, memIO);
        }

        public void AddIORename(string name, FIRIO io)
        {
            NameToIO.Add(name, io);
        }

        public void AddMemoryPort(MemPort port)
        {
            NameToIO.Add(port.Name, port);
        }

        public void AddConditional(Conditional cond)
        {
            Nodes.Add(cond);
        }

        public void AddDuplexOuputWire(Input input, DuplexIO wireDuplex)
        {
            DuplexOutputWires.Add(input, wireDuplex);
        }

        public bool TryGetDuplexOutputWire(Input input, out DuplexIO wireDuplex)
        {
            return DuplexOutputWires.TryGetValue(input, out wireDuplex);
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
            FixDuplexOutputWires();

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

        private void FixDuplexOutputWires()
        {
            foreach (var keyValue in DuplexOutputWires)
            {
                Input wireIn = (Input)keyValue.Value.GetInput();
                if (keyValue.Key.Con != null)
                {
                    keyValue.Key.Con.From.ConnectToInput(wireIn);
                }
                foreach (var inputCondCon in keyValue.Key.GetConditionalConnections())
                {
                    inputCondCon.From.ConnectToInput(wireIn, false, false, true);
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
            Dictionary<Input, Input> inToIntModIn = new Dictionary<Input, Input>();

            foreach (var inIO in GetInternalIO())
            {
                var copy = inIO.Flip(mod);
                IOHelper.BiDirFullyConnectIO(inIO, copy, true);

                mod.AddExternalIO(copy);

                Input[] inIOInputs = inIO.GetAllIOOfType<Input>().ToArray();
                Output[] extModOutputs = copy.GetAllIOOfType<Output>().ToArray();
                Debug.Assert(inIOInputs.Length == extModOutputs.Length);
                for (int i = 0; i < inIOInputs.Length; i++)
                {
                    if (DuplexOutputWires.ContainsKey(inIOInputs[i]))
                    {
                        inToIntModIn.Add(inIOInputs[i], (Input)mod.GetPairedIO(extModOutputs[i]));
                    }
                }
            }

            foreach (var nodeIO in NameToIO)
            {
                if (nodeIO.Key == "io.expt~220")
                {

                }
                FIRIO copy = nodeIO.Value.Flip(mod);
                copy.SetName(nodeIO.Key);
                IOHelper.BiDirFullyConnectIO(nodeIO.Value, copy, true);

                mod.AddExternalIO(copy);

                Input[] inIOInputs = nodeIO.Value.GetAllIOOfType<Input>().ToArray();
                Output[] extModOutputs = copy.GetAllIOOfType<Output>().ToArray();
                Debug.Assert(inIOInputs.Length == extModOutputs.Length);
                for (int i = 0; i < inIOInputs.Length; i++)
                {
                    if (DuplexOutputWires.ContainsKey(inIOInputs[i]))
                    {
                        inToIntModIn.Add(inIOInputs[i], (Input)mod.GetPairedIO(extModOutputs[i]));
                    }
                }
            }

            foreach (var keyValue in DuplexOutputWires)
            {
                Input modIntInput = inToIntModIn[keyValue.Key];
                DuplexIO modIntDuplex = (DuplexIO)mod.GetIO(keyValue.Value.Name);
                mod.AddDuplexOuputWire(modIntInput, modIntDuplex);
            }
        }

        internal void ExternalConnectUsedIO(Module parentMod)
        {
            //Propagate hidden ports
            foreach (var intKeyVal in InternalIO)
            {
                var intIO = intKeyVal.Value;
                var extIO = ExternalIO[intKeyVal.Key];

                //Ports can be nested and propagating one port may reveal more
                //ports that should be propagated. Keep checking if more ports
                //should be propagated until all ports have been checked.
                HashSet<IPortsIO> handledPortsIO = new HashSet<IPortsIO>();
                while (true)
                {
                    var intHidesPorts = intIO.GetAllIOOfType<IPortsIO>().ToArray();
                    
                    //if this io contain no ports to begin with then
                    //skip it
                    if (intHidesPorts.Length == 0)
                    {
                        break;
                    }

                    bool handledNewPort = false;

                    FIRIO parentIO = (FIRIO)parentMod.GetIO(intKeyVal.Key);

                    for (int i = 0; i < intHidesPorts.Length; i++)
                    {
                        //Has seen port before?
                        if (!handledPortsIO.Add(intHidesPorts[i]))
                        {
                            continue;
                        }

                        handledNewPort = true;
                        IPortsIO internalHidden = intHidesPorts[i];
                        IPortsIO externalHidden = (IPortsIO)GetPairedIO((FIRIO)internalHidden);
                        IPortsIO parentModHidden = parentIO.GetAllIOOfType<IPortsIO>().ToArray()[i];

                        FIRIO[] intPortsNeedsProp = internalHidden.GetAllPorts().Where(x => !IsPartOfPair(x)).ToArray();

                        //Add internal ports to external io
                        FIRIO[] newExtPorts = externalHidden.GetOrMakeFlippedPortsFrom(intPortsNeedsProp);

                        //Add external ports to whatever io it's connecting to
                        FIRIO[] newParentPorts = parentModHidden.GetOrMakeFlippedPortsFrom(newExtPorts);
                        Debug.Assert(newExtPorts.Length == newParentPorts.Length);

                        for (int y = 0; y < newExtPorts.Length; y++)
                        {
                            //New ports need to be paired in the module as they
                            //just passed from internal to external module io
                            if (!IsPartOfPair(newExtPorts[y]))
                            {
                                AddPairedIO(intPortsNeedsProp[y], newExtPorts[y]);

                                //FIRRTL scoping is truely stupid. If a memory port is created in an inner
                                //scope then it can still be used in an outer scope. To handle that all
                                //newly aded memory ports are added to the outer scope.
                                if (newParentPorts[y] is MemPort memPort &&
                                    !parentMod.TryGetIO(newParentPorts[y].Name, false, out var _))
                                {
                                    parentMod.AddMemoryPort(memPort);
                                }

                                //Connect new external ports to where they should
                                //be connected
                                IOHelper.BiDirFullyConnectIO(newExtPorts[y], newParentPorts[y], true);
                            }
                        }

                        //IOHelper.PropegatePorts(parentModHidden);
                    }

                    //If went through all ports and found no new ones
                    //then no more new ports can be present meaning
                    //they have all been found
                    if (!handledNewPort)
                    {
                        break;
                    }
                }
            }

            var extKeyVals = ExternalIO.ToArray();
            var intKeyVals = InternalIO.ToArray();

            for (int i = 0; i < extKeyVals.Length; i++)
            {
                var extKeyVal = extKeyVals[i];
                var intKeyVal = intKeyVals[i];

                ScalarIO[] extFlat = extKeyVal.Value.Flatten().ToArray();
                ScalarIO[] intFlat = intKeyVal.Value.Flatten().ToArray();
                Debug.Assert(extFlat.Length == intFlat.Length);

                for (int x = 0; x < extFlat.Length; x++)
                {
                    if (intFlat[x].IsConnectedToAnything())
                    {
                        if (parentMod.TryGetIO(intKeyVal.Key, false, out var parentContainer))
                        {
                            FIRIO parentIO = (FIRIO)parentContainer;
                            ScalarIO[] parentIOFlat = parentIO.Flatten().ToArray();

                            IOHelper.BiDirFullyConnectIO(extFlat[x], parentIOFlat[x], true);
                        }
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
                Debug.Assert(extFlat.Length == intFlat.Length);

                for (int x = 0; x < extFlat.Length; x++)
                {
                    if (!intFlat[x].IsConnectedToAnything())
                    {
                        if (intFlat[x] is Input input && input.HasSinkSource())
                        {
                            continue;
                        }
                        extFlat[x].DisconnectAll();
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
