using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.IO
{
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
}
