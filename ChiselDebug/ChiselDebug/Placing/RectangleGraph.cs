using ChiselDebug.GraphFIR.Components;
using ChiselDebug.Graphing;
using ChiselDebug.Routing;
using ChiselDebug.Utilities;
using System.Collections.Generic;

namespace ChiselDebug.Placing
{
    internal sealed class RectangleGraph
    {
        private readonly Graph<FIRRTLNode> Graph;
        private readonly Dictionary<FIRRTLNode, Node<FIRRTLNode>> FIRRTLNodeToGraphNode;
        private readonly Rectangle BoardArea;

        public RectangleGraph(Graph<FIRRTLNode> graph, Dictionary<FIRRTLNode, Node<FIRRTLNode>> firrtlNodeToGraphNode, Rectangle boardArea)
        {
            Graph = graph;
            FIRRTLNodeToGraphNode = firrtlNodeToGraphNode;
            BoardArea = boardArea;
        }

        public void SpaceNodesByLinesBetweenThem(List<LineInfo> lines)
        {

        }

        private List<Node<FIRRTLNode>> GetPath(IOInfo from, IOInfo to)
        {
            PriorityQueue<Node<FIRRTLNode>, int> bestNodes = new PriorityQueue<Node<FIRRTLNode>, int>();
        }

        private void AddToNodeSizes(List<Node<FIRRTLNode>> path)
        {

        }

        private static Point GetRectanglesClosestPointToOtherPoint(Point distanceFrom, Rectangle rectangle)
        {

        }

        private sealed class RectangleContainedLines
        {
            public int HorizontalLines;
            public int VerticalLines;
        }
    }
}
