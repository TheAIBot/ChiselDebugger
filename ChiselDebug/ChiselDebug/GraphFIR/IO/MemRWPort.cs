using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.IO
{
    public class MemRWPort : MemPort
    {
        internal readonly FIRIO DataOut;
        internal readonly FIRIO DataIn;
        internal readonly FIRIO Mask;

        public MemRWPort(FIRIO inputType, int addressWidth, FIRRTLNode node, string name) : this(name, CreateIO(inputType, addressWidth, node))
        { }

        public MemRWPort(string name, List<FIRIO> io) : base(name, io)
        {
            this.DataOut = (FIRIO)GetIO("rdata");
            this.DataIn = (FIRIO)GetIO("wdata");
            this.Mask = (FIRIO)GetIO("wmask");
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

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node = null)
        {
            return new MemRWPort(Name, GetIOInOrder().Select(x => x.ToFlow(flow, node)).ToList());
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
