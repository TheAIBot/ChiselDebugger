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
        private readonly Dictionary<Input, Wire> DuplexOutputWires = new Dictionary<Input, Wire>();
        

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

            IOBundle bundle = new IOBundle(mod, bundleName, mod.ExternalIO.Values.ToList());
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

        public Output AddDuplexOuputWire(Input input)
        {
            Wire wire = new Wire(GetDuplexOutputName(input), input, null);

            DuplexIO wireIO = wire.GetAsDuplex();
            NameToIO.Add(wireIO.Name, wireIO.GetOutput());
            DuplexOutputWires.Add(input, wire);

            return (Output)wireIO.GetOutput();
        }

        public string GetDuplexOutputName(Input input)
        {
            //Full name of io may collide with name of some other io, so
            //a string is added to the end which isn't a legal firrtl name
            return input.GetFullName() + "/out";
        }

        public bool HasDunplexInput(Input input)
        {
            string duplexInputName = GetDuplexOutputName(input);
            return NameToIO.ContainsKey(duplexInputName) || 
                InternalIO.ContainsKey(duplexInputName);
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

            foreach (var condNode in Nodes.OfType<Conditional>())
            {
                foreach (var condMod in condNode.CondMods)
                {
                    if (condMod.Mod.TryGetIO(ioName, modulesOnly, out container))
                    {
                        return true;
                    }
                }
            }

            container = null;
            return false;
        }

        public List<Output> GetAllModuleConnections()
        {
            List<Output> connestions = new List<Output>();
            foreach (Output output in GetInternalOutputs())
            {
                if (output.IsConnectedToAnything())
                {
                    connestions.Add(output);
                }
            }

            foreach (var node in Nodes)
            {
                foreach (Output output in node.GetOutputs())
                {
                    if (output.IsConnectedToAnything())
                    {
                        connestions.Add(output);
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

        public KeyValuePair<string, FIRIO>[] GetIOAliases()
        {
            return NameToIO.ToArray();
        }

        internal void RemoveAllWires()
        {
            FixDuplexOutputWires();

            for (int i = Nodes.Count - 1; i >= 0 ; i--)
            {
                FIRRTLNode node = Nodes[i];
                if (node is Wire wire)
                {
                    if (wire.CanBypassWire())
                    {
                        wire.BypassWireIO();
                        Nodes.RemoveAt(i);
                        NameToIO.Remove(wire.Name);
                    }
                }
            }
        }

        private void FixDuplexOutputWires()
        {
            foreach (var keyValue in DuplexOutputWires)
            {
                Output wireOut = (Output)keyValue.Value.Result;
                if (wireOut.IsConnectedToAnything())
                {
                    if (!keyValue.Key.IsConnectedToAnything())
                    {
                        throw new Exception("Input to duplex input wire must be connected to something.");
                    }

                    //Everything connected to duplex input is now being connected to the
                    //wires input
                    Input wireIn = (Input)keyValue.Value.In;
                    if (keyValue.Key != null)
                    {
                        keyValue.Key.Con.ConnectToInput(wireIn);
                    }
                    foreach (var inputCondCon in keyValue.Key.GetConditionalConnections())
                    {
                        inputCondCon.ConnectToInput(wireIn, false, false, true);
                    }

                    keyValue.Value.BypassWireIO();
                }

                NameToIO.Remove(keyValue.Value.Name);
            }

            DuplexOutputWires.Clear();
        }

        internal void RemoveUnusedConnections()
        {
            //foreach (var node in Nodes)
            //{
            //    if (node is Module mod)
            //    {
            //        mod.DisconnectUnusedIO();
            //    }
            //    else if (node is Conditional cond)
            //    {
            //        foreach (var condMod in cond.CondMods)
            //        {
            //            condMod.Mod.DisconnectUnusedIO();
            //        }
            //    }
            //}
        }

        internal void SetConditional(Output enableCon)
        {
            foreach (var io in InternalIO.Values)
            {
                if (io is ScalarIO scalar)
                {
                    scalar.SetEnabledCondition(enableCon);
                }
                else
                {
                    foreach (var scalarIO in io.Flatten())
                    {
                        scalarIO.SetEnabledCondition(enableCon);
                    }
                }
            }
            foreach (var io in ExternalIO.Values)
            {
                if (io is ScalarIO scalar)
                {
                    scalar.SetEnabledCondition(enableCon);
                }
                else
                {
                    foreach (var scalarIO in io.Flatten())
                    {
                        scalarIO.SetEnabledCondition(enableCon);
                    }
                }
            }
            foreach (var node in Nodes)
            {
                foreach (var nodeIO in node.GetIO())
                {
                    if (nodeIO is ScalarIO scalar)
                    {
                        scalar.SetEnabledCondition(enableCon);
                    }
                    else
                    {
                        foreach (var scalarIO in nodeIO.Flatten())
                        {
                            scalarIO.SetEnabledCondition(enableCon);
                        }
                    }

                }
            }
        }

        internal void CopyInternalAsExternalIO(Module mod)
        {
            foreach (Input input in GetInternalInputs())
            {
                if (!HasDunplexInput(input))
                {
                    AddDuplexOuputWire(input);
                }
            }
            foreach (IOBundle modBundle in BundleToModule.Keys)
            {
                foreach (Input input in modBundle.Flatten().OfType<Input>())
                {
                    if (!HasDunplexInput(input))
                    {
                        AddDuplexOuputWire(input);
                    }
                }
            }

            mod.ReserveMemory(InternalIO.Count + NameToIO.Count);

            HashSet<string> ioAdded = new HashSet<string>();
            foreach (var inIO in GetInternalIO())
            {
                var copy = inIO.Flip(mod);
                //IOHelper.BiDirFullyConnectIO(inIO, copy, true);

                ioAdded.Add(inIO.Name);
                mod.AddExternalIO(copy);
            }

            foreach (var nodeIO in NameToIO)
            {
                if (ioAdded.Add(nodeIO.Key))
                {
                    FIRIO copy = nodeIO.Value.Flip(mod);
                    copy.SetName(nodeIO.Key);
                    //IOHelper.BiDirFullyConnectIO(nodeIO.Value, copy, true);

                    mod.AddExternalIO(copy);
                }
            }
        }

        internal void ExternalConnectUsedIO(Module parentMod)
        {
            HashSet<IPortsIO> handledPortsIO = new HashSet<IPortsIO>();
            //Propagate hidden ports
            foreach (var intKeyVal in InternalIO)
            {
                var intIO = intKeyVal.Value;
                var extIO = ExternalIO[intKeyVal.Key];

                //Ports can be nested and propagating one port may reveal more
                //ports that should be propagated. Keep checking if more ports
                //should be propagated until all ports have been checked.
                handledPortsIO.Clear();
                while (true)
                {
                    var intHiddenPorts = intIO.GetAllIOOfType<IPortsIO>();

                    //if this io contain no ports to begin with then
                    //skip it
                    if (!intHiddenPorts.Any())
                    {
                        break;
                    }

                    FIRIO parentIO = (FIRIO)parentMod.GetIO(intKeyVal.Key);
                    var parentModHiddenPorts = parentIO.GetAllIOOfType<IPortsIO>();

                    bool handledNewPort = false;

                    foreach (var hiddenPorts in intHiddenPorts.Zip(parentModHiddenPorts))
                    {
                        IPortsIO internalHidden = hiddenPorts.First;
                        IPortsIO parentModHidden = hiddenPorts.Second;

                        //Has seen port before?
                        if (!handledPortsIO.Add(internalHidden))
                        {
                            continue;
                        }

                        handledNewPort = true;
                        IPortsIO externalHidden = (IPortsIO)GetPairedIO((FIRIO)internalHidden);

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

            ScalarIO[] oneExt = new ScalarIO[1];
            ScalarIO[] oneInt = new ScalarIO[1];

            for (int i = 0; i < extKeyVals.Length; i++)
            {
                var extKeyVal = extKeyVals[i];
                var intKeyVal = intKeyVals[i];

                ScalarIO[] extFlat;
                ScalarIO[] intFlat;
                if (extKeyVal.Value is ScalarIO extIO && intKeyVal.Value is ScalarIO intIO)
                {
                    oneExt[0] = extIO;
                    oneInt[0] = intIO;

                    extFlat = oneExt;
                    intFlat = oneInt;
                }
                else
                {
                    extFlat = extKeyVal.Value.Flatten().ToArray();
                    intFlat = intKeyVal.Value.Flatten().ToArray();
                }

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
            foreach (var intIO in InternalIO.Values)
            {
                if (intIO is ScalarIO scalar)
                {
                    if (!scalar.IsConnectedToAnything())
                    {
                        ScalarIO extIO = (ScalarIO)GetPairedIO(scalar);
                        extIO.DisconnectAll();
                    }
                }
                else
                {
                    foreach (var scalarIntIO in intIO.Flatten())
                    {
                        if (!scalarIntIO.IsConnectedToAnything())
                        {
                            ScalarIO extIO = (ScalarIO)GetPairedIO(scalarIntIO);
                            extIO.DisconnectAll();
                        }
                    }
                }
            }
        }

        internal IEnumerable<T> GetAllNestedNodesOfType<T>()
        {
            if (this is T tThis)
            {
                yield return tThis;
            }

            foreach (var node in Nodes)
            {
                if (node is T tNode)
                {
                    yield return tNode;
                }
                else if (node is Module mod)
                {
                    foreach (var tFound in mod.GetAllNestedNodesOfType<T>())
                    {
                        yield return tFound;
                    }
                }
                else if (node is Conditional cond)
                {
                    foreach (var condMod in cond.CondMods)
                    {
                        foreach (var tFound in condMod.Mod.GetAllNestedNodesOfType<T>())
                        {
                            yield return tFound;
                        }
                    }
                }
            }
        }

        public override void Compute()
        {
            throw new Exception("This node is not computable");
        }

        internal override void InferType()
        {
            foreach (var node in Nodes)
            {
                node.InferType();
            }

            foreach (Output output in GetInternalOutputs())
            {
                if (output.IsConnectedToAnything())
                {
                    output.InferType();
                    Debug.Assert(output.Type != null);

                    foreach (var input in output.GetConnectedInputs())
                    {
                        input.InferType();
                        Debug.Assert(input.Type != null);
                    }
                }
            }

            foreach (var node in Nodes)
            {
                foreach (Output output in node.GetOutputs())
                {
                    if (output.IsConnectedToAnything())
                    {
                        output.InferType();
                        Debug.Assert(output.Type != null);

                        foreach (var input in output.GetConnectedInputs())
                        {
                            input.InferType();
                            Debug.Assert(input.Type != null);
                        }
                    }
                }
            }
        }

        internal void FinishConnections()
        {
            foreach (Output output in GetInternalOutputs())
            {
                if (output.Type != null)
                {
                    output.SetDefaultvalue();
                }
            }

            foreach (var node in Nodes)
            {
                foreach (Output output in node.GetOutputs())
                {
                    if (output.Type != null)
                    {
                        output.SetDefaultvalue();
                    }
                }
            }

            foreach (var node in Nodes)
            {
                if (node is Module mod)
                {
                    mod.FinishConnections();
                }
                else if (node is Conditional cond)
                {
                    foreach (var condMod in cond.CondMods)
                    {
                        condMod.Mod.FinishConnections();
                    }
                }
            }
        }
    }
}
