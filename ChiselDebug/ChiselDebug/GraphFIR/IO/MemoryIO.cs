using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR.IO
{
    public class MemoryIO : IOBundle, IPortsIO
    {
        private readonly List<MemPort> VisiblePorts = new List<MemPort>();
        private readonly FIRIO InputType;
        private readonly int AddressWidth;

        public MemoryIO(FIRRTLNode node, string name, List<FIRIO> io, FIRIO inputType, int addressWidth) : this(node, name, io, inputType, addressWidth, new List<MemPort>())
        { }

        private MemoryIO(FIRRTLNode node, string name, List<FIRIO> io, FIRIO inputType, int addressWidth, List<MemPort> visiblePorts) : base(node, name, io)
        {
            this.VisiblePorts = visiblePorts;
            this.InputType = inputType.Copy(null);
            this.AddressWidth = addressWidth;
        }

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node)
        {
            return new MemoryIO(node, Name, GetIOInOrder().Select(x => x.ToFlow(flow, node)).ToList(), InputType.ToFlow(flow, node), AddressWidth, VisiblePorts.Select(x => (MemPort)x.ToFlow(flow, node)).ToList());
        }

        internal MemReadPort AddReadPort(string portName)
        {
            MemReadPort port = new MemReadPort(Node, InputType, AddressWidth, portName);
            VisiblePorts.Add(port);
            AddIO(port.Name, port);

            return port;
        }

        internal MemWritePort AddWritePort(string portName)
        {
            MemWritePort port = new MemWritePort(Node, InputType, AddressWidth, portName);
            VisiblePorts.Add(port);
            AddIO(port.Name, port);

            return port;
        }

        internal MemRWPort AddReadWritePort(string portName)
        {
            MemRWPort port = new MemRWPort(Node, InputType, AddressWidth, portName);
            VisiblePorts.Add(port);
            AddIO(port.Name, port);

            return port;
        }

        public FIRIO GetDataType()
        {
            return InputType;
        }

        FIRIO[] IPortsIO.GetAllPorts()
        {
            return VisiblePorts.ToArray();
        }

        FIRIO[] IPortsIO.GetOrMakeFlippedPortsFrom(FIRIO[] otherPorts)
        {
            FIRIO[] newPorts = new FIRIO[otherPorts.Length];

            for (int i = 0; i < newPorts.Length; i++)
            {
                MemPort portThere = (MemPort)otherPorts[i];
                MemPort portHere;
                if (!portThere.IsAnonymous && TryGetIO(portThere.Name, false, out var portContainer))
                {
                    portHere = (MemPort)portContainer;
                }
                else
                {
                    portHere = (MemPort)otherPorts[i].Flip(Node);
                    VisiblePorts.Add(portHere);
                    AddIO(portHere.Name, portHere);
                }

                newPorts[i] = portHere;
            }

            return newPorts;
        }
    }
}
