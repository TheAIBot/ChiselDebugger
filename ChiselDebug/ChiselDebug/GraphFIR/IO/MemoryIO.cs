using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.IO
{
    public sealed class MemoryIO : IOBundle
    {
        private readonly List<MemPort> Ports = new List<MemPort>();
        private readonly FIRIO InputType;
        private readonly int AddressWidth;

        public MemoryIO(FIRRTLNode node, string name, List<FIRIO> io, FIRIO inputType, int addressWidth) : this(node, name, io, inputType, addressWidth, new List<MemPort>())
        { }

        private MemoryIO(FIRRTLNode node, string name, List<FIRIO> io, FIRIO inputType, int addressWidth, List<MemPort> ports) : base(node, name, io)
        {
            this.Ports = ports;
            this.InputType = inputType.Copy(null);
            this.AddressWidth = addressWidth;
        }

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node)
        {
            return new MemoryIO(node, Name, GetIOInOrder().Select(x => x.ToFlow(flow, node)).ToList(), InputType.ToFlow(flow, node), AddressWidth, Ports.Select(x => (MemPort)x.ToFlow(flow, node)).ToList());
        }

        internal MemReadPort AddReadPort(string portName)
        {
            MemReadPort port = new MemReadPort(Node, InputType, AddressWidth, portName);
            Ports.Add(port);
            AddIO(port.Name, port);

            return port;
        }

        internal MemWritePort AddWritePort(string portName)
        {
            MemWritePort port = new MemWritePort(Node, InputType, AddressWidth, portName);
            Ports.Add(port);
            AddIO(port.Name, port);

            return port;
        }

        internal MemRWPort AddReadWritePort(string portName)
        {
            MemRWPort port = new MemRWPort(Node, InputType, AddressWidth, portName);
            Ports.Add(port);
            AddIO(port.Name, port);

            return port;
        }

        public FIRIO GetDataType()
        {
            return InputType;
        }

        public MemPort[] GetAllPorts()
        {
            return Ports.ToArray();
        }
    }
}
