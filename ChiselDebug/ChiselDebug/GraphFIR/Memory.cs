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
        private readonly MemoryIO MemIO;

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
            this.MemIO = new MemoryIO(name, new List<FIRIO>(), InputType, GetAddressWidth(), this);
        }

        internal MemReadPort AddReadPort(string portName)
        {
            return MemIO.AddReadPort(portName);
        }

        internal MemWritePort AddWritePort(string portName)
        {
            return MemIO.AddWritePort(portName);
        }

        internal MemRWPort AddReadWritePort(string portName)
        {
            return MemIO.AddReadWritePort(portName);
        }

        internal int GetAddressWidth()
        {
            return (int)Math.Round(Math.Log2(Size));
        }

        internal MemoryIO GetIOAsBundle()
        {
            return MemIO;
        }

        public override ScalarIO[] GetInputs()
        {
            return MemIO.Flatten().OfType<Input>().ToArray();
        }

        public override ScalarIO[] GetOutputs()
        {
            return MemIO.Flatten().OfType<Output>().ToArray();
        }

        public override FIRIO[] GetIO()
        {
            return MemIO.GetIOInOrder();
        }

        public override void InferType()
        {
            
        }

        public bool TryGetIO(string ioName, bool modulesOnly, out IContainerIO container)
        {
            if (MemIO.TryGetIO(ioName, modulesOnly, out IContainerIO innerIO))
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

    public class MemoryIO : IOBundle
    {
        private readonly List<MemPort> HiddenPorts = new List<MemPort>();
        private readonly FIRIO InputType;
        private readonly int AddressWidth;
        private readonly FIRRTLNode Node;

        public MemoryIO(string name, List<FIRIO> io, FIRIO inputType, int addressWidth, FIRRTLNode node) : base(name, io)
        {
            this.InputType = inputType.Copy(null);
            this.AddressWidth = addressWidth;
            this.Node = node;
        }

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node = null)
        {
            return new MemoryIO(Name, GetIOInOrder().Select(x => x.ToFlow(flow, node)).ToList(), InputType, AddressWidth, node);
        }

        internal MemReadPort AddReadPort(string portName)
        {
            MemReadPort port = new MemReadPort(InputType, AddressWidth, Node, portName);
            HiddenPorts.Add(port);

            return port;
        }

        internal MemWritePort AddWritePort(string portName)
        {
            MemWritePort port = new MemWritePort(InputType, AddressWidth, Node, portName);
            HiddenPorts.Add(port);

            return port;
        }

        internal MemRWPort AddReadWritePort(string portName)
        {
            MemRWPort port = new MemRWPort(InputType, AddressWidth, Node, portName);
            HiddenPorts.Add(port);

            return port;
        }

        internal bool HasHiddenPorts()
        {
            return HiddenPorts.Count > 0;
        }

        internal List<MemPort> CopyHiddenPortsFrom(MemoryIO otherMem)
        {
            List<MemPort> newPorts = new List<MemPort>();
            foreach (var port in otherMem.HiddenPorts)
            {
                MemPort newPort = (MemPort)port.Flip(Node);
                HiddenPorts.Add(newPort);
                newPorts.Add(newPort);
            }

            return newPorts;
        }

        internal void MakePortsVisible()
        {
            foreach (var port in HiddenPorts)
            {
                AddIO(port.Name, port);
            }

            HiddenPorts.Clear();
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

        public MemReadPort(FIRIO inputType, int addressWidth, FIRRTLNode node, string name) : this(name, CreateIO(inputType, addressWidth, node))
        { }

        public MemReadPort(string name, List<FIRIO> io) : base(name, io)
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
            io.Add(new Input(node, "clk/prev", new ClockType()));

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
            return new MemReadPort(Name, GetIOInOrder().Select(x => x.ToFlow(flow, node)).ToList());
        }
    }

    public class MemWritePort : MemPort
    {
        internal readonly FIRIO DataIn;
        internal readonly FIRIO Mask;

        public MemWritePort(FIRIO inputType, int addressWidth, FIRRTLNode node, string name) : this(name, CreateIO(inputType, addressWidth, node))
        { }

        public MemWritePort(string name, List<FIRIO> io) : base(name, io)
        {
            this.DataIn = (FIRIO)GetIO("data");
            this.Mask = (FIRIO)GetIO("mask");
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
            io.Add(new Input(node, "clk/prev", new ClockType()));

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

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node = null)
        {
            return new MemWritePort(Name, GetIOInOrder().Select(x => x.ToFlow(flow, node)).ToList());
        }
    }

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
            io.Add(new Input(node, "clk/prev", new ClockType()));

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
    }
}
