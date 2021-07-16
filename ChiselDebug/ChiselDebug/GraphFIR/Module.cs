﻿using ChiselDebug.GraphFIR.IO;
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
        public readonly string InstanceName;
        private readonly List<FIRRTLNode> Nodes = new List<FIRRTLNode>();
        private readonly Dictionary<string, FIRIO> NameToIO = new Dictionary<string, FIRIO>();
        private readonly Dictionary<Input, Wire> DuplexOutputWires = new Dictionary<Input, Wire>();
        public Output EnableCon { get; private set; }
        public bool IsConditional => EnableCon != null;
        

        public Module(string name, string instanceName, Module parentMod, FirrtlNode defNode) : base(defNode)
        {
            this.Name = name;
            this.InstanceName = instanceName;
            SetModResideIn(parentMod);
        }

        public void SetEnableCond(Output enable)
        {
            EnableCon = enable;
        }

        public void AddNode(FIRRTLNode node)
        {
            node.SetModResideIn(this);
            Nodes.Add(node);
            foreach (var io in node.GetVisibleIO())
            {
                if (!io.IsAnonymous)
                {
                    NameToIO.Add(io.Name, io);
                }
            }
        }

        public void AddIORename(string name, FIRIO io)
        {
            NameToIO.Add(name, io);
        }

        public void AddMemoryPort(MemPort port)
        {
            if (IsConditional)
            {
                ResideIn.AddMemoryPort(port);
            }
            else
            {
                NameToIO.Add(port.Name, port);
            }
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

        public bool TryGetIOInternal(string ioName, bool lookUp, out IContainerIO container)
        {
            if (NameToIO.TryGetValue(ioName, out FIRIO innerIO))
            {
                container = innerIO;
                return true;
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
                        if (condMod.Mod.TryGetIO(ioName, out container))
                        {
                            return true;
                        }
                    }
                }
            }

            if (IsConditional && ResideIn.TryGetIOInternal(ioName, false, out container))
            {
                return true;
            }

            container = null;
            return false;
        }

        public override bool TryGetIO(string ioName, out IContainerIO container)
        {
            return TryGetIOInternal(ioName, true, out container);
        }

        public override IEnumerable<FIRIO> GetVisibleIO()
        {
            yield return GetIOAsBundle();
        }

        internal ModuleIO GetIOAsBundle()
        {
            return new ModuleIO(this, InstanceName, ExternalIO.Values.ToList());
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

        public List<Input> GetAllModuleInputs()
        {
            List<Input> connestions = new List<Input>();
            foreach (Input input in GetInternalInputs())
            {
                if (input.IsConnectedToAnything())
                {
                    connestions.Add(input);
                }
            }

            foreach (var node in Nodes)
            {
                foreach (Input input in node.GetInputs())
                {
                    if (input.IsConnectedToAnything())
                    {
                        connestions.Add(input);
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

        internal void RemoveAllDuplexWires()
        {
            FixDuplexOutputWires();
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
