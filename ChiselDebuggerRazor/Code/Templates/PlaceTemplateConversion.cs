using ChiselDebug.GraphFIR.Components;
using ChiselDebug.Placing;
using ChiselDebug.Utilities;
using ChiselDebuggerRazor.Code.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code.Templates
{
    public sealed class PlaceTemplateConversion
    {
        public readonly ModuleLayout Ctrl;
        private readonly FIRRTLNode[] NodeOrder;

        public PlaceTemplateConversion(ModuleLayout ctrl, FIRRTLNode[] nodeOrder)
        {
            this.Ctrl = ctrl;
            this.NodeOrder = nodeOrder;
        }

        public Task TemplateUpdatedAsync(PlaceTemplate template)
        {
            return Ctrl.PlaceNodesAsync(Convert(template));
        }

        public PlacementInfo Convert(PlaceTemplate template)
        {
            List<Positioned<FIRRTLNode>> convertedNodePos = new List<Positioned<FIRRTLNode>>();
            Dictionary<FIRRTLNode, Rectangle> convertedSpace = new Dictionary<FIRRTLNode, Rectangle>();

            for (int i = 0; i < template.NodeOrder.Length; i++)
            {
                Positioned<FIRRTLNode>? from = template.NodeOrder[i];
                if (from == null)
                {
                    continue;
                }

                FIRRTLNode to = NodeOrder[i];

                convertedNodePos.Add(new Positioned<FIRRTLNode>(from.Value.Position, to));
                convertedSpace.Add(to, template.PlaceInfo.UsedSpace[from.Value.Value]);
            }

            return new PlacementInfo(convertedNodePos, convertedSpace, template.PlaceInfo.SpaceNeeded);
        }
    }
}
