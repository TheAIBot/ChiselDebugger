﻿using ChiselDebug.GraphFIR.Components;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
#nullable enable

namespace ChiselDebug.GraphFIR.IO
{
    public sealed class MemReadPort : MemPort
    {
        internal readonly FIRIO DataOut;

        public MemReadPort(FIRRTLNode? node, FIRIO inputType, int addressWidth, string name) : this(node, name, CreateIO(inputType, addressWidth, node))
        { }

        public MemReadPort(FIRRTLNode? node, string name, List<FIRIO> io) : base(node, name, io)
        {
            ArgumentNullException.ThrowIfNull(name);
            this.DataOut = (FIRIO)GetIO("data");
        }

        private static List<FIRIO> CreateIO(FIRIO inputType, int addressWidth, FIRRTLNode? node)
        {
            FIRIO dataOut = inputType.Flip(node);
            dataOut.SetName("data");

            List<FIRIO> io = new List<FIRIO>
            {
                dataOut,
                new Sink(node, "addr", new UIntType(addressWidth)),
                new Sink(node, "en", new UIntType(1)),
                new Sink(node, "clk", new ClockType())
            };

            return io;
        }

        public override FIRIO GetSink()
        {
            throw new Exception("Can't get input from this IO.");
        }

        public override FIRIO GetSource()
        {
            return DataOut;
        }

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode? node)
        {
            return new MemReadPort(node, Name!, GetIOInOrder().Select(x => x.ToFlow(flow, node)).ToList());
        }

        public override bool TryGetIO(string ioName, [NotNullWhen(true)] out IContainerIO? container)
        {
            if (base.TryGetIO(ioName, out container))
            {
                return true;
            }
            else if (DataOut.TryGetIO(ioName, out container))
            {
                return true;
            }

            return false;
        }

        internal override bool HasMask()
        {
            return false;
        }

        internal override FIRIO GetMask()
        {
            throw new Exception("This memory port does not have a mask.");
        }
    }
}
