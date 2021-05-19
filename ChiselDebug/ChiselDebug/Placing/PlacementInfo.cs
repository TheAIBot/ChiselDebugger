using ChiselDebug.GraphFIR;
using System.Collections.Generic;

namespace ChiselDebug
{
    public class PlacementInfo
    {
        public readonly List<Positioned<FIRRTLNode>> NodePositions;
        public readonly Dictionary<FIRRTLNode, Rectangle> UsedSpace;
        public Point SpaceNeeded { get; private set; }
        
        public PlacementInfo() : this(new List<Positioned<FIRRTLNode>>(), new Dictionary<FIRRTLNode, Rectangle>(), Point.Zero)
        { }

        public PlacementInfo(List<Positioned<FIRRTLNode>> nodePoses, Dictionary<FIRRTLNode, Rectangle> usedSpace, Point spaceNeeded)
        {
            this.NodePositions = nodePoses;
            this.UsedSpace = usedSpace;
            this.SpaceNeeded = spaceNeeded;
        }

        internal void AddNodePlacement(FIRRTLNode node, Rectangle shape)
        {
            NodePositions.Add(new Positioned<FIRRTLNode>(shape.Pos, node));
            SpaceNeeded = Point.Max(SpaceNeeded, new Point(shape.RightX, shape.BottomY));

            UsedSpace.Add(node, shape);
        }

        internal void AddEndStuff(Point endPadding)
        {
            SpaceNeeded += endPadding;
        }

        public void SetSpaceNeeded(Point spaceNeeded)
        {
            SpaceNeeded = spaceNeeded;
        }
    }
}
