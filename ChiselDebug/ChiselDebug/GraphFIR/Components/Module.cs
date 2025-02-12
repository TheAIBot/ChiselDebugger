﻿using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using VCDReader;
#nullable enable

namespace ChiselDebug.GraphFIR.Components
{
    public sealed class Module : FIRRTLContainer
    {
        public readonly string Name;
        public readonly string? InstanceName;
        private readonly List<FIRRTLNode> Nodes = new List<FIRRTLNode>();
        private readonly Dictionary<string, FIRIO> NameToIO = new Dictionary<string, FIRIO>();
        private readonly Dictionary<Sink, Wire> DuplexOutputWires = new Dictionary<Sink, Wire>();
        public Source? EnableCon { get; private set; }

        [MemberNotNullWhen(true, nameof(EnableCon))]
        public bool IsConditional => EnableCon != null;

        public Module(string name, string? instanceName, Module? parentMod, IFirrtlNode? defNode) : base(defNode)
        {
            Name = name;
            InstanceName = instanceName;
            SetModResideIn(parentMod);
        }

        public void SetEnableCond(Source enable)
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

        public void RemoveNode(FIRRTLNode node)
        {
            node.SetModResideIn(null);
            if (!Nodes.Remove(node))
            {
                throw new ArgumentException("The node is not part of this module.");
            }
            foreach (var io in node.GetVisibleIO())
            {
                if (!io.IsAnonymous)
                {
                    NameToIO.Remove(io.Name);
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
                ResideIn!.AddMemoryPort(port);
            }
            else
            {
                NameToIO.Add(port.Name!, port);
            }
        }

        public Source AddDuplexOuputWire(Sink input)
        {
            Wire wire = new Wire(GetDuplexOutputName(input), input, null);
            wire.SetModResideIn(this);

            DuplexIO wireIO = wire.GetAsDuplex();
            NameToIO.Add(wireIO.Name!, wireIO.GetSource());
            DuplexOutputWires.Add(input, wire);

            return (Source)wireIO.GetSource();
        }

        public string GetDuplexOutputName(Sink input)
        {
            //Full name of io may collide with name of some other io, so
            //a string is added to the end which isn't a legal firrtl name
            return input.GetFullName() + "/out";
        }

        public bool TryGetIOInternal(string ioName, bool lookUp, bool lookDown, [NotNullWhen(true)] out IContainerIO? container)
        {
            if (NameToIO.TryGetValue(ioName, out FIRIO? innerIO))
            {
                container = innerIO;
                return true;
            }

            if (InternalIO.TryGetValue(ioName, out FIRIO? io))
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
                        if (condMod.TryGetIOInternal(ioName, true, false, out container))
                        {
                            return true;
                        }
                    }
                }
            }

            if (lookDown && IsConditional && ResideIn!.TryGetIOInternal(ioName, false, true, out container))
            {
                return true;
            }

            container = null;
            return false;
        }

        public override bool TryGetIO(string ioName, [NotNullWhen(true)] out IContainerIO? container)
        {
            return TryGetIOInternal(ioName, true, true, out container);
        }

        public override IEnumerable<FIRIO> GetVisibleIO()
        {
            yield return new ModuleIO(this, InstanceName, ExternalIO.Values.ToList());
        }

        public List<Source> GetAllModuleConnections()
        {
            List<Source> connestions = new List<Source>();
            foreach (Source output in GetInternalSources())
            {
                if (output.IsConnectedToAnything())
                {
                    connestions.Add(output);
                }
            }

            foreach (var node in Nodes)
            {
                foreach (Source output in node.GetSources())
                {
                    if (output.IsConnectedToAnything())
                    {
                        connestions.Add(output);
                    }
                }
            }

            return connestions;
        }

        public List<Sink> GetAllModuleInputs()
        {
            List<Sink> connestions = new List<Sink>();
            foreach (Sink input in GetInternalSinks())
            {
                if (input.IsConnectedToAnything())
                {
                    connestions.Add(input);
                }
            }

            foreach (var node in Nodes)
            {
                foreach (Sink input in node.GetSinks())
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

        public FIRIO[] GetAllIOOrdered()
        {
            List<ScalarIO> allOrdered = GetAllInternalIOOrdered();
            foreach (var node in Nodes)
            {
                foreach (var io in node.GetIO())
                {
                    io.Flatten(allOrdered);
                }
            }

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
                Source wireOut = (Source)keyValue.Value.Result;
                if (wireOut.IsConnectedToAnything())
                {
                    if (!keyValue.Key.IsConnectedToAnything())
                    {
                        throw new Exception("Input to duplex input wire must be connected to something.");
                    }

                    //Everything connected to duplex input is now being connected to the
                    //wires input
                    Sink wireIn = (Sink)keyValue.Value.In;
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
                        foreach (var tFound in condMod.GetAllNestedNodesOfType<T>())
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
