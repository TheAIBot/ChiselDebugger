using ChiselDebug.GraphFIR.Components;
using ChiselDebug.Routing;
using ChiselDebug.Utilities;

namespace ChiselDebug.Placing
{
    public interface INodePlacer
    {
        void SetNodeSize(FIRRTLNode node, Point size, DirectedIO[] inputOffsets, DirectedIO[] outputOffsets);
        bool IsReadyToPlace();
        PlacementInfo PositionModuleComponents();
    }
}
