using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Placing;
using ChiselDebug.Routing;
using ChiselDebuggerRazor.Code.Controllers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code.Templates
{
    public sealed class RouteTemplator
    {
        private readonly ConcurrentDictionary<string, RouteTemplate> Templates = new();
        private readonly ConcurrentDictionary<string, ConcurrentBag<RouteTemplateConversion>> Converters = new();
        private readonly ConcurrentDictionary<string, bool> TemplateGenerating = new();
        private readonly IWorkLimiter WorkLimiter;

        public RouteTemplator(IWorkLimiter workLimiter)
        {
            WorkLimiter = workLimiter;
        }

        public Task SubscribeToTemplate(string moduleName, ModuleLayout ctrl, FIRRTLNode[] nodeOrder, FIRIO[] ioOrder)
        {
            RouteTemplateConversion conversion = new RouteTemplateConversion(ctrl, nodeOrder, ioOrder);
            var converterList = new ConcurrentBag<RouteTemplateConversion>() {
                conversion
            };
            Converters.AddOrUpdate(moduleName, converterList, (_, converters) => {
                converters.Add(conversion);
                return converters;
            });

            if (Templates.TryGetValue(moduleName, out var template))
            {
                return conversion.TemplateUpdated(template);
            }

            return Task.CompletedTask;
        }

        public Task AddTemplateParameters(string moduleName, SimpleRouter router, PlacementInfo placeInfo, FIRRTLNode[] nodeOrder, FIRIO[] ioOrder, CancellationToken cancelToken)
        {
            //If template is already generating or generated then
            //no need to generate again
            if (!TemplateGenerating.TryAdd(moduleName, true))
            {
                return Task.CompletedTask;
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
            }, 2);
        }

        private Task AddTemplate(string moduleName, List<WirePath> wires, FIRRTLNode[] nodeOrder, FIRIO[] ioOrder)
        {
            RouteTemplate template = new RouteTemplate(wires, nodeOrder, ioOrder);
            Templates.AddOrUpdate(moduleName, template, (_, _) => template);

            if (Converters.TryGetValue(moduleName, out var convs))
            {
                return Task.WhenAll(convs.Select(x => x.TemplateUpdated(template)));
            }

            return Task.CompletedTask;
        }

        public bool TryGetTemplate(string moduleName, ModuleLayout modLayout, out List<WirePath> wires)
        {
            RouteTemplate modTemplate;
            if (!Templates.TryGetValue(moduleName, out modTemplate))
            {
                wires = null;
                return false;
            }

            if (Converters.TryGetValue(moduleName, out var cons))
            {
                wires = cons.First(x => x.Ctrl == modLayout).Convert(modTemplate);
                return true;
            }

            wires = null;
            return false;
        }
    }
}
