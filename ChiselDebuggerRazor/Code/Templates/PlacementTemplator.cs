using ChiselDebug.GraphFIR.Components;
using ChiselDebug.Placing;
using ChiselDebuggerRazor.Code.Controllers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code.Templates
{
    public sealed class PlacementTemplator
    {
        private readonly ConcurrentDictionary<string, PlaceTemplate> Templates = new ConcurrentDictionary<string, PlaceTemplate>();
        private readonly Dictionary<string, List<PlaceTemplateConversion>> Converters = new Dictionary<string, List<PlaceTemplateConversion>>();
        private readonly HashSet<string> TemplateGenerating = new HashSet<string>();
        private readonly IWorkLimiter WorkLimiter;

        public PlacementTemplator(IWorkLimiter workLimiter)
        {
            WorkLimiter = workLimiter;
        }

        public Task SubscribeToTemplate(string moduleName, ModuleLayout ctrl, FIRRTLNode[] nodeOrder)
        {
            PlaceTemplateConversion conversion = new PlaceTemplateConversion(ctrl, nodeOrder);
            lock (Converters)
            {
                if (Converters.TryGetValue(moduleName, out var convs))
                {
                    convs.Add(conversion);
                }
                else
                {
                    Converters.Add(moduleName, new List<PlaceTemplateConversion>() { conversion });
                }
            }

            if (Templates.TryGetValue(moduleName, out var template))
            {
                return conversion.TemplateUpdated(template);
            }

            return Task.CompletedTask;
        }

        public Task AddTemplateParameters(string moduleName, INodePlacer placer, FIRRTLNode[] nodeOrder, CancellationToken cancelToken)
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
                var placeInfo = placer.PositionModuleComponents();
                if (cancelToken.IsCancellationRequested)
                {
                    return Task.CompletedTask;
                }

                return AddTemplate(moduleName, placeInfo, nodeOrder);
            }, 1);
        }

        private Task AddTemplate(string moduleName, PlacementInfo placeInfo, FIRRTLNode[] nodeOrder)
        {
            PlaceTemplate template = new PlaceTemplate(placeInfo, nodeOrder);
            Templates.AddOrUpdate(moduleName, template, (_, _) => template);

            lock (Converters)
            {
                if (Converters.TryGetValue(moduleName, out var convs))
                {
                    return Task.WhenAll(convs.Select(x => x.TemplateUpdated(template)));
                }
            }

            return Task.CompletedTask;
        }

        public bool TryGetTemplate(string moduleName, ModuleLayout modLayout, out PlacementInfo placement)
        {
            PlaceTemplate modTemplate;
            if (!Templates.TryGetValue(moduleName, out modTemplate))
            {
                placement = null;
                return false;
            }

            lock (Converters)
            {
                if (Converters.TryGetValue(moduleName, out var cons))
                {
                    placement = cons.First(x => x.Ctrl == modLayout).Convert(modTemplate);
                    return true;
                }
            }

            placement = null;
            return false;
        }
    }
}
