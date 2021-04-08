using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.IO
{
    public class MemReadPort : MemPort
    {
        internal readonly FIRIO DataOut;

        public MemReadPort(FIRRTLNode node, FIRIO inputType, int addressWidth, string name) : this(node, name, CreateIO(inputType, addressWidth, node))
        { }

        public MemReadPort(FIRRTLNode node, string name, List<FIRIO> io) : base(node, name, io)
        {
            this.DataOut = (FIRIO)GetIO("data");
        }

        private static List<FIRIO> CreateIO(FIRIO inputType, int addressWidth, FIRRTLNode node)
        {
            FIRIO dataOut = inputType.Flip(node);
            dataOut.SetName("data");

            List<FIRIO> io = new List<FIRIO>();
            io.Add(dataOut);
            io.Add(new Input(node, "addr", new UIntType(addressWidth)));
            io.Add(new Input(node, "en", new UIntType(1)));
            io.Add(new Input(node, "clk", new ClockType()));

            return io;
        }

        public override FIRIO GetInput()
        {
            throw new Exception("Can't get input from this IO.");
        }

        public override FIRIO GetOutput()
        {
            return DataOut;
        }

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node = null)
        {
            return new MemReadPort(node ?? Node, Name, GetIOInOrder().Select(x => x.ToFlow(flow, node)).ToList());
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
