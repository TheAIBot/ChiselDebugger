using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Routing;
using ChiselDebuggerRazor.Code.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code.Templates
{
    public sealed class RouteTemplateConversion
    {
        public readonly ModuleLayout Ctrl;
        private readonly FIRRTLNode[] NodeOrder;
        private readonly FIRIO[] IOOrder;

        public RouteTemplateConversion(ModuleLayout ctrl, FIRRTLNode[] nodeOrder, FIRIO[] ioOrder)
        {
            this.Ctrl = ctrl;
            this.NodeOrder = nodeOrder;
            this.IOOrder = ioOrder;
        }

        public Task TemplateUpdatedAsync(RouteTemplate template)
        {
            return Ctrl.PlaceWiresAsync(Convert(template));
        }

        public List<WirePath> Convert(RouteTemplate template)
        {
            List<WirePath> convWires = new List<WirePath>();
            for (int i = 0; i < template.Wires.Count; i++)
            {
                WirePath fromWires = template.Wires[i];
                FIRRTLNode fromStartNode = fromWires.GetStartNode();
                FIRRTLNode fromEndNode = fromWires.GetEndNode();
                FIRIO fromStartIO = fromWires.GetStartIO();
                FIRIO fromEndIO = fromWires.GetEndIO();

                int startNodeIndex = template.NodeOrderIndex[fromStartNode];
                int endNodeIndex = template.NodeOrderIndex[fromEndNode];
                int startIOIndex = template.IOOrderIndex[fromStartIO];
                int endIOIndex = template.IOOrderIndex[fromEndIO];

                FIRRTLNode toStartNode = NodeOrder[startNodeIndex];
                FIRRTLNode toEndNode = NodeOrder[endNodeIndex];
                FIRIO toStartIO = IOOrder[startIOIndex];
                FIRIO toEndIO = IOOrder[endIOIndex];

                convWires.Add(fromWires.CopyWithNewNodes(toStartNode, toStartIO, toEndNode, toEndIO));
            }

            return convWires;
        }
    }
}
