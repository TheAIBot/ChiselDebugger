using ChiselDebug.GraphFIR;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChiselDebug
{
    public class Placer
    {
        private readonly Module Mod;
        private readonly HashSet<FIRRTLNode> MissingNodeDims;
        private readonly Dictionary<FIRRTLNode, Point> NodeSizes = new Dictionary<FIRRTLNode, Point>();

        public delegate void PlacedHandler(PlacementInfo placements);
        public event PlacedHandler OnPlacedNodes;

        public Placer(Module mod)
        {
            this.Mod = mod;
            this.MissingNodeDims = new HashSet<FIRRTLNode>(Mod.GetAllNodes());
        }

        private PlacementInfo PositionModuleComponents()
        {
            PlacementInfo placments = new PlacementInfo();

            int x = 30;
            foreach (var node in Mod.GetAllNodes())
            {
                Point size = NodeSizes[node];
                Point pos = new Point(x, 0);

                placments.AddNodePlacement(node, new Rectangle(pos, size));

                x += 200;
            }

            return placments;
        }

        public void SetNodeSize(FIRRTLNode node, Point size)
        {
            NodeSizes[node] = size;

            MissingNodeDims.Remove(node);
            if (MissingNodeDims.Count == 0)
            {
                OnPlacedNodes?.Invoke(PositionModuleComponents());
            }
        }
    }

    public class PlacementInfo
    {
        public readonly List<Positioned<FIRRTLNode>> NodePositions = new List<Positioned<FIRRTLNode>>();
        public readonly Dictionary<FIRRTLNode, Rectangle> UsedSpace = new Dictionary<FIRRTLNode, Rectangle>();
        public Point SpaceNeeded { get; private set; } = new Point(0, 0);

        internal void AddNodePlacement(FIRRTLNode node, Rectangle shape)
        {
            NodePositions.Add(new Positioned<FIRRTLNode>(shape.Pos, node));
            SpaceNeeded = Point.Max(SpaceNeeded, new Point(shape.RightX, shape.BottomY));

            UsedSpace.Add(node, shape);
        }
    }
}
