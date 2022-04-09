using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Placing;
using ChiselDebug.Routing;
using ChiselDebuggerRazor.Code.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code.Templates
{
    public class RouteTemplator
    {
        private readonly Dictionary<string, RouteTemplate> Templates = new Dictionary<string, RouteTemplate>();
        private readonly Dictionary<string, List<RouteTemplateConversion>> Converters = new Dictionary<string, List<RouteTemplateConversion>>();
        private readonly HashSet<string> TemplateGenerating = new HashSet<string>();
        private readonly IWorkLimiter WorkLimiter;

        public RouteTemplator(IWorkLimiter workLimiter)
        {
            WorkLimiter = workLimiter;
        }

        public Task SubscribeToTemplate(string moduleName, ModuleLayout ctrl, FIRRTLNode[] nodeOrder, FIRIO[] ioOrder)
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
                    return conversion.TemplateUpdated(template);
                }
            }

            return Task.CompletedTask;
        }

        public Task AddTemplateParameters(string moduleName, SimpleRouter router, PlacementInfo placeInfo, FIRRTLNode[] nodeOrder, FIRIO[] ioOrder, CancellationToken cancelToken)
        {
            lock (TemplateGenerating)
            {
                //If template is already generating or generated then
                //no need to generate again
                if (!TemplateGenerating.Add(moduleName))
                {
                    return Task.CompletedTask;
                }
            }

            return WorkLimiter.AddWork(() =>
            {
                List<WirePath> wires;
                if (!router.TryPathLines(placeInfo, cancelToken, out wires))
                {
                    return Task.CompletedTask;
                }
                if (cancelToken.IsCancellationRequested)
                {
                    return Task.CompletedTask;
                }

                return AddTemplate(moduleName, wires, nodeOrder, ioOrder);
            });
        }

        private Task AddTemplate(string moduleName, List<WirePath> wires, FIRRTLNode[] nodeOrder, FIRIO[] ioOrder)
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
                    return Task.WhenAll(convs.Select(x => x.TemplateUpdated(template)));
                }
            }

            return Task.CompletedTask;
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
