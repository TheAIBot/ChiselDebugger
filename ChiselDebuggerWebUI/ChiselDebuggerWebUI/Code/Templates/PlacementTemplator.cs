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
    public class PlaceTemplate
    {
        public readonly PlacementInfo PlaceInfo;
        public readonly Positioned<FIRRTLNode>?[] NodeOrder;

        public PlaceTemplate(PlacementInfo placeInfo, FIRRTLNode[] nodeOrder)
        {
            this.PlaceInfo = placeInfo;
            this.NodeOrder = GetPlacementInNodeOrder(placeInfo, nodeOrder);
        }

        public static Positioned<FIRRTLNode>?[] GetPlacementInNodeOrder(PlacementInfo placeInfo, FIRRTLNode[] nodeOrder)
        {
            Dictionary<FIRRTLNode, Positioned<FIRRTLNode>> unordered = new Dictionary<FIRRTLNode, Positioned<FIRRTLNode>>();
            foreach (var posNode in placeInfo.NodePositions)
            {
                unordered.Add(posNode.Value, posNode);
            }

            Positioned<FIRRTLNode>?[] ordered = new Positioned<FIRRTLNode>?[nodeOrder.Length];
            for (int i = 0; i < ordered.Length; i++)
            {
                //Not all nodes in a module is placed. Those that are not placed
                //are replaced with null, so the node order is preserved.
                //Conversion can then use null to know that it should not convert
                //that node.
                if (unordered.TryGetValue(nodeOrder[i], out var nodePos))
                {
                    ordered[i] = nodePos;
                }
                else
                {
                    ordered[i] = null;
                }
            }

            return ordered;
        }
    }
    public class PlaceTemplateConversion
    {
        private readonly ModuleController Ctrl;
        private readonly FIRRTLNode[] NodeOrder;

        public PlaceTemplateConversion(ModuleController ctrl, FIRRTLNode[] nodeOrder)
        {
            this.Ctrl = ctrl;
            this.NodeOrder = nodeOrder;
        }

        public void TemplateUpdated(PlaceTemplate template)
        {
            Ctrl.PlaceNodes(Convert(template));
        }

        private PlacementInfo Convert(PlaceTemplate template)
        {
            List<Positioned<FIRRTLNode>> convertedNodePos = new List<Positioned<FIRRTLNode>>();
            Dictionary<FIRRTLNode, Rectangle> convertedSpace = new Dictionary<FIRRTLNode, Rectangle>();

            for (int i = 0; i < template.NodeOrder.Length; i++)
            {
                if (template.NodeOrder[i] == null)
                {
                    continue;
                }

                Positioned<FIRRTLNode> from = template.NodeOrder[i].Value;
                FIRRTLNode to = NodeOrder[i];

                convertedNodePos.Add(new Positioned<FIRRTLNode>(from.Position, to));
                convertedSpace.Add(to, template.PlaceInfo.UsedSpace[from.Value]);
            }

            return new PlacementInfo(convertedNodePos, convertedSpace, template.PlaceInfo.SpaceNeeded);
        }
    }

    public class PlacementTemplator
    {
        private readonly Dictionary<string, PlaceTemplate> Templates = new Dictionary<string, PlaceTemplate>();
        private readonly Dictionary<string, List<PlaceTemplateConversion>> Converters = new Dictionary<string, List<PlaceTemplateConversion>>();
        private readonly HashSet<string> TemplateGenerating = new HashSet<string>();

        public void SubscribeToTemplate(string moduleName, ModuleController ctrl, FIRRTLNode[] nodeOrder)
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
    }
}
