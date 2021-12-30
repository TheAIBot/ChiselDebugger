using ChiselDebug.GraphFIR.Components;
using ChiselDebug.Placing;
using ChiselDebug.Utilities;
using System.Collections.Generic;

namespace ChiselDebuggerRazor.Code.Templates
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
}
