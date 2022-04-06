using ChiselDebug.GraphFIR.Components;
using ChiselDebug.Placing;
using ChiselDebuggerRazor.Code.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ChiselDebuggerRazor.Code.Templates
{
    public class PlacementTemplator
    {
        private readonly Dictionary<string, PlaceTemplate> Templates = new Dictionary<string, PlaceTemplate>();
        private readonly Dictionary<string, List<PlaceTemplateConversion>> Converters = new Dictionary<string, List<PlaceTemplateConversion>>();
        private readonly HashSet<string> TemplateGenerating = new HashSet<string>();
        private readonly WorkLimiter WorkLimiter;

        public PlacementTemplator(WorkLimiter workLimiter)
        {
            WorkLimiter = workLimiter;
        }

        public void SubscribeToTemplate(string moduleName, ModuleLayout ctrl, FIRRTLNode[] nodeOrder)
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

            lock (Templates)
            {
                if (Templates.TryGetValue(moduleName, out var template))
                {
                    conversion.TemplateUpdated(template);
                }
            }
        }

        public void AddTemplateParameters(string moduleName, PlacingBase placer, FIRRTLNode[] nodeOrder, CancellationToken cancelToken)
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
                var placeInfo = placer.PositionModuleComponents();
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }

                AddTemplate(moduleName, placeInfo, nodeOrder);
            });
        }

        private void AddTemplate(string moduleName, PlacementInfo placeInfo, FIRRTLNode[] nodeOrder)
        {
            PlaceTemplate template = new PlaceTemplate(placeInfo, nodeOrder);
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

        public bool TryGetTemplate(string moduleName, ModuleLayout modLayout, out PlacementInfo placement)
        {
            PlaceTemplate modTemplate;
            lock (Templates)
            {
                if (!Templates.TryGetValue(moduleName, out modTemplate))
                {
                    placement = null;
                    return false;
                }
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
