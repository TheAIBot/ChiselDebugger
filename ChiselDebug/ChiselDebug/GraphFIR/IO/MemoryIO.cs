using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.IO
{
    public class MemoryIO : IOBundle, IHiddenPorts
    {
        private readonly List<MemPort> HiddenPorts = new List<MemPort>();
        private readonly FIRIO InputType;
        private readonly int AddressWidth;

        public MemoryIO(FIRRTLNode node, string name, List<FIRIO> io, FIRIO inputType, int addressWidth) : base(node, name, io)
        {
            this.InputType = inputType.Copy(null);
            this.AddressWidth = addressWidth;
        }

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node = null)
        {
            return new MemoryIO(node ?? Node, Name, GetIOInOrder().Select(x => x.ToFlow(flow, node)).ToList(), InputType, AddressWidth);
        }

        internal MemReadPort AddReadPort(string portName)
        {
            MemReadPort port = new MemReadPort(Node, InputType, AddressWidth, portName);
            port.SetParentIO(this);
            HiddenPorts.Add(port);

            return port;
        }

        internal MemWritePort AddWritePort(string portName)
        {
            MemWritePort port = new MemWritePort(Node, InputType, AddressWidth, portName);
            port.SetParentIO(this);
            HiddenPorts.Add(port);

            return port;
        }

        internal MemRWPort AddReadWritePort(string portName)
        {
            MemRWPort port = new MemRWPort(Node, InputType, AddressWidth, portName);
            port.SetParentIO(this);
            HiddenPorts.Add(port);

            return port;
        }

        bool IHiddenPorts.HasHiddenPorts()
        {
            return HiddenPorts.Count > 0;
        }

        FIRIO[] IHiddenPorts.GetHiddenPorts()
        {
            return HiddenPorts.ToArray();
        }

        FIRIO[] IHiddenPorts.CopyHiddenPortsFrom(IHiddenPorts otherWithPorts)
        {
            FIRIO[] otherPorts = otherWithPorts.GetHiddenPorts();
            FIRIO[] newPorts = new FIRIO[otherPorts.Length];

            for (int i = 0; i < newPorts.Length; i++)
            {
                MemPort newPort = (MemPort)otherPorts[i].Flip(Node);
                newPort.SetParentIO(this);
                HiddenPorts.Add(newPort);
                newPorts[i] = newPort;
            }

            return newPorts;
        }

        void IHiddenPorts.MakePortsVisible()
        {
            foreach (var port in HiddenPorts)
            {
                AddIO(port.Name, port);
            }

            HiddenPorts.Clear();
        }
    }
}
