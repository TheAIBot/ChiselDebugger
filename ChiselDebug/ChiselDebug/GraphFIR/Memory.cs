using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.GraphFIR
{
    public class Memory : FIRRTLNode, IContainerIO
    {
        public readonly string Name;
        public readonly FIRIO InputType;
        public readonly ulong Size;
        public readonly int ReadLatency;
        public readonly int WriteLatency;
        public readonly ReadUnderWrite RUW;
        private readonly Dictionary<string, FIRIO> Ports = new Dictionary<string, FIRIO>();

        public Memory(string name, FIRIO inputType, ulong size, int readLatency, int writeLatency, ReadUnderWrite ruw)
        {
            if (!inputType.IsPassiveOfType<Input>())
            {
                throw new Exception("Input type must be a passive input type.");
            }

            this.Name = name;
            this.InputType = inputType.Copy(this);
            this.Size = size;
            this.ReadLatency = readLatency;
            this.WriteLatency = writeLatency;
            this.RUW = ruw;
        }

        internal MemReadPort AddReadPort(string portName)
        {
            MemReadPort port = new MemReadPort(this, portName);
            Ports.Add(portName, port);

            return port;
        }

        internal MemWritePort AddWritePort(string portName)
        {
            MemWritePort port = new MemWritePort(this, portName);
            Ports.Add(portName, port);

            return port;
        }

        internal MemRWPort AddReadWritePort(string portName)
        {
            MemRWPort port = new MemRWPort(this, portName);
            Ports.Add(portName, port);

            return port;
        }

        internal int GetAddressWidth()
        {
            return (int)Math.Round(Math.Log2(Size));
        }

        internal IOBundle GetIOAsBundle()
        {
            return new IOBundle(Name, Ports.Values.ToList(), false);
        }

        public override ScalarIO[] GetInputs()
        {
            return FIRRTLContainer.FlattenAndFilterIO<Input>(Ports);
        }

        public override ScalarIO[] GetOutputs()
        {
            return FIRRTLContainer.FlattenAndFilterIO<Output>(Ports);
        }

        public override FIRIO[] GetIO()
        {
            return Ports.Values.ToArray();
        }

        public override void InferType()
        {
            
        }

        public bool TryGetIO(string ioName, bool modulesOnly, out IContainerIO container)
        {
            if (Ports.TryGetValue(ioName, out FIRIO innerIO))
            {
                container = innerIO;
                return true;
            }

            container = null;
            return false;
        }

        public IContainerIO GetIO(string ioName, bool modulesOnly = false)
        {
            if (TryGetIO(ioName, modulesOnly, out var container))
            {
                return container;
            }

            throw new Exception($"Failed to find io. IO name: {ioName}");
        }
    }

    public class MemPort : IOBundle, IPreserveDuplex
    {
        internal readonly Input Address;
        internal readonly Input Enabled;
        internal readonly Input Clock;

        public MemPort(string name, List<FIRIO> io) : base(name, io)
        {
            this.Address = (Input)GetIO("addr");
            this.Enabled = (Input)GetIO("en");
            this.Clock = (Input)GetIO("clk");
        }

        protected static void AsMaskType(FIRIO maskFrom)
        {
            if (maskFrom is IOBundle bundle)
            {
                foreach (ScalarIO scalar in bundle.Flatten())
                {
                    scalar.SetType(new UIntType(1));
                }
            }
            else if (maskFrom is ScalarIO scalar)
            {
                scalar.SetType(new UIntType(1));
            }
        }
    }

    internal interface IPreserveDuplex { }

    public class MemReadPort : MemPort
    {
        internal readonly FIRIO DataOut;

        public MemReadPort(Memory mem, string name) : base(name, CreateIO(mem))
        {
            this.DataOut = (FIRIO)GetIO("data");
        }

        private static List<FIRIO> CreateIO(Memory mem)
        {
            FIRIO dataOut = mem.InputType.Flip();
            dataOut.SetName("data");

            List<FIRIO> io = new List<FIRIO>();
            io.Add(dataOut);
            io.Add(new Input(mem, "addr", new UIntType(mem.GetAddressWidth())));
            io.Add(new Input(mem, "en", new UIntType(1)));
            io.Add(new Input(mem, "clk", new ClockType()));
            io.Add(new Input(mem, "clk/prev", new ClockType()));

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
    }

    public class MemWritePort : MemPort
    {
        internal readonly FIRIO DataIn;
        internal readonly FIRIO Mask;

        public MemWritePort(Memory mem, string name) : base(name, CreateIO(mem))
        {
            this.DataIn = (FIRIO)GetIO("data");
            this.Mask = (FIRIO)GetIO("mask");
        }

        private static List<FIRIO> CreateIO(Memory mem)
        {
            FIRIO dataIn = mem.InputType.Copy();
            dataIn.SetName("data");

            FIRIO mask = mem.InputType.Copy();
            mask.SetName("mask");
            AsMaskType(mask);

            List<FIRIO> io = new List<FIRIO>();
            io.Add(dataIn);
            io.Add(mask);
            io.Add(new Input(mem, "addr", new UIntType(mem.GetAddressWidth())));
            io.Add(new Input(mem, "en", new UIntType(1)));
            io.Add(new Input(mem, "clk", new ClockType()));
            io.Add(new Input(mem, "clk/prev", new ClockType()));

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
    }

    public class MemRWPort : MemPort
    {
        internal readonly FIRIO DataOut;
        internal readonly FIRIO DataIn;
        internal readonly FIRIO Mask;

        public MemRWPort(Memory mem, string name) : base(name, CreateIO(mem))
        {
            this.DataOut = (FIRIO)GetIO("rdata");
            this.DataIn = (FIRIO)GetIO("data");
            this.Mask = (FIRIO)GetIO("mask");
        }

        private static List<FIRIO> CreateIO(Memory mem)
        {
            FIRIO dataOut = mem.InputType.Flip();
            dataOut.SetName("rdata");

            FIRIO dataIn = mem.InputType.Copy();
            dataIn.SetName("data");

            FIRIO mask = mem.InputType.Copy();
            mask.SetName("mask");
            AsMaskType(mask);

            List<FIRIO> io = new List<FIRIO>();
            io.Add(new Input(mem, "wmode", new UIntType(1)));
            io.Add(dataOut);
            io.Add(dataIn);
            io.Add(mask);
            io.Add(new Input(mem, "addr", new UIntType(mem.GetAddressWidth())));
            io.Add(new Input(mem, "en", new UIntType(1)));
            io.Add(new Input(mem, "clk", new ClockType()));
            io.Add(new Input(mem, "clk/prev", new ClockType()));

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
    }
}
