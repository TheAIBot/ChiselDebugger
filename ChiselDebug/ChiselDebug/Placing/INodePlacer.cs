using ChiselDebug.GraphFIR.Components;
using ChiselDebug.Utilities;

namespace ChiselDebug.Placing
{
    public interface INodePlacer
    {
        void SetNodeSize(FIRRTLNode node, Point size);
        bool IsReadyToPlace();
        PlacementInfo PositionModuleComponents();
    }
}
