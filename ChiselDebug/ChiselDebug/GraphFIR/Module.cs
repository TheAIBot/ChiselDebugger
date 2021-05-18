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
        private readonly Module ParentScopeView;
        public Output EnableCon { get; private set; }
        public bool IsConditional => EnableCon != null;
        

        public Module(string name, Module parentScope, FirrtlNode defNode) : base(defNode)
        {
            this.Name = name;
            this.ParentScopeView = parentScope;
            SetModResideIn(this);
        }

        public void SetEnableCond(Output enable)
        {
            EnableCon = enable;
        }

        public void AddNode(FIRRTLNode node)
        {
            node.SetModResideIn(this);
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
            reg.SetModResideIn(this);
            Nodes.Add(reg);

            DuplexIO regIO = reg.GetAsDuplex();
            NameToIO.Add(regIO.GetInput().Name, regIO);
            NameToIO.Add(regIO.GetOutput().Name, regIO);
        }

        public void AddWire(Wire wire)
        {
            wire.SetModResideIn(this);
            Nodes.Add(wire);

            DuplexIO wireIO = wire.GetAsDuplex();
            NameToIO.Add(wireIO.Name, wireIO);
        }

        public void AddModule(Module mod, string bundleName)
        {
            mod.SetModResideIn(this);
            Nodes.Add(mod);

            IOBundle bundle = new IOBundle(mod, bundleName, mod.ExternalIO.Values.ToList(), false);
            NameToIO.Add(bundleName, bundle);
            BundleToModule.Add(bundle, mod);
        }

        public void AddMemory(Memory mem)
        {
            mem.SetModResideIn(this);
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
            if (ParentScopeView != null)
            {
                ParentScopeView.AddMemoryPort(port);
            }
            NameToIO.Add(port.Name, port);
        }

        public void AddConditional(Conditional cond)
        {
            cond.SetModResideIn(this);
            foreach (var condMod in cond.CondMods)
            {
                condMod.Mod.SetModResideIn(this);
            }
            Nodes.Add(cond);
        }

        public Output AddDuplexOuputWire(Input input)
        {
            Wire wire = new Wire(GetDuplexOutputName(input), input, null);
            wire.SetModResideIn(this);

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

        public bool TryGetIOInternal(string ioName, bool modulesOnly, bool lookUp, out IContainerIO container)
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

            if (lookUp)
            {
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
            }

            if (ParentScopeView != null && ParentScopeView.TryGetIOInternal(ioName, modulesOnly, false, out container))
            {
                return true;
            }

            container = null;
            return false;
        }

        public override bool TryGetIO(string ioName, bool modulesOnly, out IContainerIO container)
        {
            return TryGetIOInternal(ioName, modulesOnly, true, out container);
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

        internal void RemoveAllWires()
        {
            FixDuplexOutputWires();

            for (int i = Nodes.Count - 1; i >= 0; i--)
            {
                FIRRTLNode node = Nodes[i];
                if (node is Wire wire)
                {
                    wire.BypassWireIO();
                    Nodes.RemoveAt(i);
                    NameToIO.Remove(wire.Name);
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
                    if (keyValue.Key.Con != null)
                    {
                        keyValue.Key.Con.ConnectToInput(wireIn);
                    }
                    foreach (var con in keyValue.Key.GetConnections())
                    {
                        con.From.ConnectToInput(wireIn, false, false, con.Condition);
                    }

                    keyValue.Value.BypassWireIO();
                }

                NameToIO.Remove(keyValue.Value.Name);
            }

            DuplexOutputWires.Clear();
        }

        internal IEnumerable<T> GetAllNestedNodesOfType<T>()
        {
            if (this is T tThis)
            {
                yield return tThis;
            }

            foreach (var node in Nodes)
            {
                if (node is Module mod)
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
                else if (node is T tNode)
                {
                    yield return tNode;
                }
            }
        }

        public override void Compute()
        {
            throw new Exception("This node is not computable");
        }
    }
}
