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
