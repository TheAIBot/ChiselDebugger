using ChiselDebug;
using ChiselDebug.GraphFIR;
using ChiselDebuggerWebUI.Pages.FIRRTLUI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerWebUI.Code.Templates
{
    public class PlacementTemplator
    {
        private readonly Dictionary<string, PlaceTemplate> Templates = new Dictionary<string, PlaceTemplate>();
        private readonly Dictionary<string, List<PlaceTemplateConversion>> Converters = new Dictionary<string, List<PlaceTemplateConversion>>();
        private readonly HashSet<string> TemplateGenerating = new HashSet<string>();

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

        public void AddTemplateParameters(string moduleName, SimplePlacer placer, FIRRTLNode[] nodeOrder)
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
                AddTemplate(moduleName, placeInfo, nodeOrder);
            });
        }

        private void AddTemplate(string moduleName, PlacementInfo placeInfo, FIRRTLNode[] nodeOrder)
        {
            PlaceTemplate template = new PlaceTemplate(placeInfo, nodeOrder);
            lock (Templates)
            {
                Templates.Add(moduleName, template);
                Debug.WriteLine($"Added place template for {moduleName}");
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
