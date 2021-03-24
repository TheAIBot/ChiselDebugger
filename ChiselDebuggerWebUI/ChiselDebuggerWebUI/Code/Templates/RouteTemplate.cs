using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Routing;
using System.Collections.Generic;

namespace ChiselDebuggerWebUI.Code.Templates
{
    public class RouteTemplate
    {
        public readonly List<WirePath> Wires;
        public readonly Dictionary<FIRRTLNode, int> NodeOrderIndex = new Dictionary<FIRRTLNode, int>();
        public readonly Dictionary<FIRIO, int> IOOrderIndex = new Dictionary<FIRIO, int>();

        public RouteTemplate(List<WirePath> wires, FIRRTLNode[] nodeOrder, FIRIO[] ioOrder)
        {
            this.Wires = wires;
            for (int i = 0; i < nodeOrder.Length; i++)
            {
                NodeOrderIndex.Add(nodeOrder[i], i);
            }

            for (int i = 0; i < ioOrder.Length; i++)
            {
                IOOrderIndex.Add(ioOrder[i], i);
            }
        }
    }
}
