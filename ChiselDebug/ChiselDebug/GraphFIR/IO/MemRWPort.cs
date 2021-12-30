﻿using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.IO
{
    public sealed class MemRWPort : MemPort
    {
        internal readonly FIRIO DataOut;
        internal readonly FIRIO DataIn;
        internal readonly FIRIO Mask;
        internal readonly FIRIO WriteMode;

        public MemRWPort(FIRRTLNode node, FIRIO inputType, int addressWidth, string name) : this(node, name, CreateIO(inputType, addressWidth, node))
        { }

        public MemRWPort(FIRRTLNode node, string name, List<FIRIO> io) : base(node, name, io)
        {
            this.DataOut = (FIRIO)GetIO("rdata");
            this.DataIn = (FIRIO)GetIO("wdata");
            this.Mask = (FIRIO)GetIO("wmask");
            this.WriteMode = (FIRIO)GetIO("wmode");

            InitDataToMask();
        }

        private static List<FIRIO> CreateIO(FIRIO inputType, int addressWidth, FIRRTLNode node)
        {
            FIRIO dataOut = inputType.Flip(node);
            dataOut.SetName("rdata");

            FIRIO dataIn = inputType.Copy(node);
            dataIn.SetName("wdata");

            FIRIO mask = inputType.Copy(node);
            mask.SetName("wmask");
            AsMaskType(mask);

            List<FIRIO> io = new List<FIRIO>();
            io.Add(new Input(node, "wmode", new UIntType(1)));
            io.Add(dataOut);
            io.Add(dataIn);
            io.Add(mask);
            io.Add(new Input(node, "addr", new UIntType(addressWidth)));
            io.Add(new Input(node, "en", new UIntType(1)));
            io.Add(new Input(node, "clk", new ClockType()));

            return io;
        }

        public override FIRIO GetInput()
        {
            return DataIn;
        }

        public override FIRIO GetOutput()
        {
            return DataOut;
        }

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node)
        {
            return new MemRWPort(node, Name, GetIOInOrder().Select(x => x.ToFlow(flow, node)).ToList());
        }

        public override bool TryGetIO(string ioName, out IContainerIO container)
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
            return true;
        }

        internal override FIRIO GetMask()
        {
            return Mask;
        }

        internal IOBundle GetAsReadPortBundle()
        {
            List<FIRIO> io = new List<FIRIO>();
            io.Add(DataOut);
            io.Add(Address);
            io.Add(Enabled);
            io.Add(Clock);

            List<string> names = new List<string>();
            names.Add("data");
            names.Add("addr");
            names.Add("en");
            names.Add("clk");

            return new IOBundle(Node, Name, io, names, false);
        }

        internal IOBundle GetAWritePortBundle()
        {
            List<FIRIO> io = new List<FIRIO>();
            io.Add(DataIn);
            io.Add(Address);
            io.Add(Enabled);
            io.Add(Clock);
            io.Add(Mask);
            io.Add(WriteMode);

            List<string> names = new List<string>();
            names.Add("data");
            names.Add("addr");
            names.Add("en");
            names.Add("clk");
            names.Add("mask");
            names.Add("valid");

            return new IOBundle(Node, Name, io, names, false);
        }
    }
}
