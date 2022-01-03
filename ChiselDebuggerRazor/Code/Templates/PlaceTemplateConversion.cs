using ChiselDebug.GraphFIR.Components;
using ChiselDebug.Placing;
using ChiselDebug.Utilities;
using ChiselDebuggerRazor.Code.Controllers;
using System.Collections.Generic;

namespace ChiselDebuggerRazor.Code.Templates
{
    public class PlaceTemplateConversion
    {
        public readonly ModuleLayout Ctrl;
        private readonly FIRRTLNode[] NodeOrder;

        public PlaceTemplateConversion(ModuleLayout ctrl, FIRRTLNode[] nodeOrder)
        {
            this.Ctrl = ctrl;
            this.NodeOrder = nodeOrder;
        }

        public void TemplateUpdated(PlaceTemplate template)
        {
            Ctrl.PlaceNodes(Convert(template));
        }

        public PlacementInfo Convert(PlaceTemplate template)
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
}
