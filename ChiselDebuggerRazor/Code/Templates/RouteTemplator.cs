using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Placing;
using ChiselDebug.Routing;
using ChiselDebuggerRazor.Code.Controllers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code.Templates
{
    public sealed class RouteTemplator
    {
        private readonly ConcurrentDictionary<string, RouteTemplate> Templates = new ConcurrentDictionary<string, RouteTemplate>();
        private readonly Dictionary<string, List<RouteTemplateConversion>> Converters = new Dictionary<string, List<RouteTemplateConversion>>();
        private readonly HashSet<string> TemplateGenerating = new HashSet<string>();
        private readonly IWorkLimiter WorkLimiter;

        public RouteTemplator(IWorkLimiter workLimiter)
        {
            WorkLimiter = workLimiter;
        }

        public Task SubscribeToTemplateAsync(string moduleName, ModuleLayout ctrl, FIRRTLNode[] nodeOrder, FIRIO[] ioOrder)
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

            if (Templates.TryGetValue(moduleName, out var template))
            {
                return conversion.TemplateUpdatedAsync(template);
            }

            return Task.CompletedTask;
        }

        public Task AddTemplateParametersAsync(string moduleName, SimpleRouter router, PlacementInfo placeInfo, FIRRTLNode[] nodeOrder, FIRIO[] ioOrder, CancellationToken cancelToken)
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

            return WorkLimiter.AddWorkAsync(() =>
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

                return AddTemplateAsync(moduleName, wires, nodeOrder, ioOrder);
            }, 2);
        }

        private Task AddTemplateAsync(string moduleName, List<WirePath> wires, FIRRTLNode[] nodeOrder, FIRIO[] ioOrder)
        {
            RouteTemplate template = new RouteTemplate(wires, nodeOrder, ioOrder);
            Templates.AddOrUpdate(moduleName, template, (_, _) => template);

            lock (Converters)
            {
                if (Converters.TryGetValue(moduleName, out var convs))
                {
                    return Task.WhenAll(convs.Select(x => x.TemplateUpdatedAsync(template)));
                }
            }

            return Task.CompletedTask;
        }

        public bool TryGetTemplate(string moduleName, ModuleLayout modLayout, [NotNullWhen(true)] out List<WirePath>? wires)
        {
            RouteTemplate? modTemplate;
            if (!Templates.TryGetValue(moduleName, out modTemplate))
            {
                wires = null;
                return false;
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
