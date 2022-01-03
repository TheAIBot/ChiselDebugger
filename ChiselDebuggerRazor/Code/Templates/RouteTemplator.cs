using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Placing;
using ChiselDebug.Routing;
using ChiselDebuggerRazor.Code.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ChiselDebuggerRazor.Code.Templates
{
    public class RouteTemplator
    {
        private readonly Dictionary<string, RouteTemplate> Templates = new Dictionary<string, RouteTemplate>();
        private readonly Dictionary<string, List<RouteTemplateConversion>> Converters = new Dictionary<string, List<RouteTemplateConversion>>();
        private readonly HashSet<string> TemplateGenerating = new HashSet<string>();

        public void SubscribeToTemplate(string moduleName, ModuleLayout ctrl, FIRRTLNode[] nodeOrder, FIRIO[] ioOrder)
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

        public void AddTemplateParameters(string moduleName, SimpleRouter router, PlacementInfo placeInfo, FIRRTLNode[] nodeOrder, FIRIO[] ioOrder, CancellationToken cancelToken)
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
                List<WirePath> wires;
                if (!router.TryPathLines(placeInfo, cancelToken, out wires))
                {
                    return;
                }
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }

                AddTemplate(moduleName, wires, nodeOrder, ioOrder);
            });
        }

        private void AddTemplate(string moduleName, List<WirePath> wires, FIRRTLNode[] nodeOrder, FIRIO[] ioOrder)
        {
            RouteTemplate template = new RouteTemplate(wires, nodeOrder, ioOrder);
            lock (Templates)
            {
                Templates.Add(moduleName, template);
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

        public bool TryGetTemplate(string moduleName, ModuleLayout modLayout, out List<WirePath> wires)
        {
            RouteTemplate modTemplate;
            lock (Templates)
            {
                if (!Templates.TryGetValue(moduleName, out modTemplate))
                {
                    wires = null;
                    return false;
                }
            }

            lock (Converters)
            {
                if (Converters.TryGetValue(moduleName, out var cons))
                {
                    wires = cons.First(x => x.Ctrl == modLayout).Convert(modTemplate);
                    return true;
                }
            }

            wires = null;
            return false;
        }
    }
}
