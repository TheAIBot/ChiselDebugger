using ChiselDebug.GraphFIR.Components;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.IO
{
    public sealed class MemWritePort : MemPort
    {
        internal readonly FIRIO DataIn;
        internal readonly FIRIO Mask;

        public MemWritePort(FIRRTLNode node, FIRIO inputType, int addressWidth, string name) : this(node, name, CreateIO(inputType, addressWidth, node))
        { }

        public MemWritePort(FIRRTLNode node, string name, List<FIRIO> io) : base(node, name, io)
        {
            this.DataIn = (FIRIO)GetIO("data");
            this.Mask = (FIRIO)GetIO("mask");

            InitDataToMask();
        }

        private static List<FIRIO> CreateIO(FIRIO inputType, int addressWidth, FIRRTLNode node)
        {
            FIRIO dataIn = inputType.Copy(node);
            dataIn.SetName("data");

            FIRIO mask = inputType.Copy(node);
            mask.SetName("mask");
            AsMaskType(mask);

            List<FIRIO> io = new List<FIRIO>();
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
            throw new Exception("Can't get output from this IO.");
        }

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node)
        {
            return new MemWritePort(node, Name, GetIOInOrder().Select(x => x.ToFlow(flow, node)).ToList());
        }

        internal override bool HasMask()
        {
            return true;
        }

        internal override FIRIO GetMask()
        {
            return Mask;
        }
    }
}
