using ChiselDebug;
using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Routing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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
    public class RouteTemplateConversion
    {
        private readonly ModuleController Ctrl;
        private readonly FIRRTLNode[] NodeOrder;
        private readonly FIRIO[] IOOrder;

        public RouteTemplateConversion(ModuleController ctrl, FIRRTLNode[] nodeOrder, FIRIO[] ioOrder)
        {
            this.Ctrl = ctrl;
            this.NodeOrder = nodeOrder;
            this.IOOrder = ioOrder;
        }

        public void TemplateUpdated(RouteTemplate template)
        {
            Ctrl.PlaceWires(Convert(template));
        }

        private List<WirePath> Convert(RouteTemplate template)
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

    public class RouteTemplator
    {
        private readonly Dictionary<string, RouteTemplate> Templates = new Dictionary<string, RouteTemplate>();
        private readonly Dictionary<string, List<RouteTemplateConversion>> Converters = new Dictionary<string, List<RouteTemplateConversion>>();
        private readonly HashSet<string> TemplateGenerating = new HashSet<string>();

        public void SubscribeToTemplate(string moduleName, ModuleController ctrl, FIRRTLNode[] nodeOrder, FIRIO[] ioOrder)
        {
            RouteTemplateConversion conversion = new RouteTemplateConversion(ctrl, nodeOrder, ioOrder);
            lock (Converters)
            {
                if (Converters.TryGetValue(moduleName, out var convs))
                {
                    convs.Add(conversion);
                }
                else
                {
                    Converters.Add(moduleName, new List<RouteTemplateConversion>() { conversion });
                }
            }

            lock (Templates)
            {
                if (Templates.TryGetValue(moduleName, out var template))
                {
                    conversion.TemplateUpdated(template);
                }
            }
        }

        public void AddTemplateParameters(string moduleName, SimpleRouter router, PlacementInfo placeInfo, FIRRTLNode[] nodeOrder, FIRIO[] ioOrder)
        {
            lock (TemplateGenerating)
            {
                //If template is already generating or generated then
                //no need to generate again
                if (!TemplateGenerating.Add(moduleName))
                {
                    return;
                }
            }

            WorkLimiter.AddWork(() =>
            {
                var wires = router.PathLines(placeInfo);
                AddTemplate(moduleName, wires, nodeOrder, ioOrder);
            });
        }

        private void AddTemplate(string moduleName, List<WirePath> wires, FIRRTLNode[] nodeOrder, FIRIO[] ioOrder)
        {
            RouteTemplate template = new RouteTemplate(wires, nodeOrder, ioOrder);
            lock (Templates)
            {
                Templates.Add(moduleName, template);
                Debug.WriteLine($"Added wire template for {moduleName}");
            }

            lock (Converters)
            {
                if (Converters.TryGetValue(moduleName, out var convs))
                {
                    foreach (var conv in convs)
                    {
                        conv.TemplateUpdated(template);
                    }
                }
            }
        }
    }
}
