using ChiselDebug.GraphFIR.Components;
using ChiselDebug.Graphing;
using ChiselDebug.Routing;
using ChiselDebug.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.Placing
{
    internal sealed class AxisLineContainer
    {
        private readonly Dictionary<int, List<Line>> AxisPointLines;

        public AxisLineContainer(Dictionary<int, List<Line>> axisPointLines)
        {
            AxisPointLines = axisPointLines;
        }

        public IEnumerable<List<Line>> GetAxisLinesOrderedByAxisPosition()
        {
            return AxisPointLines.OrderBy(x => x.Key).Select(x => x.Value);
        }

        public bool TryGetLinesPerpendicularToAxisPosition(int position, out List<Line> axisPoisitionLines)
        {
            return AxisPointLines.TryGetValue(position, out axisPoisitionLines);
        }
    }

    internal sealed class RectangleBoard
    {

    }

    internal sealed class RectangleBoardCreator
    {
        private readonly AxisLineContainer HorizontalLines;
        private readonly AxisLineContainer VerticalLines;

        public RectangleBoardCreator(AxisLineContainer horizontalLines, AxisLineContainer verticalLines)
        {
            HorizontalLines = horizontalLines;
            VerticalLines = verticalLines;
        }

        public RectangleBoard Create()
        {
            List<Rectangle> rectanglesOnBoard = new List<Rectangle>();
            foreach (var axisPositionLines in HorizontalLines.GetAxisLinesOrderedByAxisPosition())
            {

            }
        }

        private List<Line> CombineLinesIntoRectangleLines(List<Line> axisLines, )



        private RectangleBoard CreateRectanglesFromHorizontalAndVerticalLines()
        {
            List<Rectangle> rectanglesOnBoard = new List<Rectangle>();
            foreach (var topHorizontal in horizontalLines.Values.SelectMany(x => x))
            {
                Point topLeftPoint = topHorizontal.Start;
                if (!verticalLines.TryGetValue(topHorizontal.End.X, out List<Line> xVerticalLines))
                {
                    continue;
                }

                Line rightVertical = xVerticalLines.SingleOrDefault(x => x.Start == topHorizontal.End);
                if (rightVertical == default)
                {
                    continue;
                }

                Point bottomRight = rightVertical.End;
                rectanglesOnBoard.Add(new Rectangle(topLeftPoint, bottomRight - topLeftPoint));
            }

            return rectanglesOnBoard;
        }
    }

    internal sealed class PlaceSpacer
    {
        public void lol(PlacementInfo placement)
        {
            HashSet<Point> rectangleEdgePoints = GetRactangleEdgePoints(placement.UsedSpace.Values);

            List<Vector> expandVectors = GetRectangleExpandVectors(placement);
            Rectangle boardLimits = GetBoardSize(expandVectors);
            ExpandVectors(rectangleEdgePoints, expandVectors, boardLimits);

            List<Point> corners = GetRectangleCorners(rectangleEdgePoints);

            Dictionary<int, List<int>> horizontalLineMaker = CreateHorizontalLineMaker(corners);
            Dictionary<int, List<int>> verticalLineMaker = CreateVerticalLineMaker(corners);

            Dictionary<int, List<Line>> horizontalLines = CreateHorizontalLines(horizontalLineMaker);
            Dictionary<int, List<Line>> verticalLines = CreateVerticalLines(verticalLineMaker);

            List<Rectangle> rectanglesOnBoard = CreateRectanglesFromHorizontalAndVerticalLines(horizontalLines, verticalLines);
            List<Rectangle> nonOccupiedRectangles = rectanglesOnBoard.Except(placement.UsedSpace.Values).ToList();

            Graph<FIRRTLNode> rectangleGraph = new Graph<FIRRTLNode>();
            Dictionary<Rectangle, Node<FIRRTLNode>> rectangleToNode = new Dictionary<Rectangle, Node<FIRRTLNode>>();
            foreach (var nodeWithRectangle in placement.UsedSpace)
            {
                var node = new Node<FIRRTLNode>(nodeWithRectangle.Key);
                rectangleToNode.Add(nodeWithRectangle.Value, node);
                rectangleGraph.AddNode(node);
            }
            foreach (var nonOccupiedRectangle in nonOccupiedRectangles)
            {
                var node = new Node<FIRRTLNode>(null);
                rectangleToNode.Add(nonOccupiedRectangle, node);
                rectangleGraph.AddNode(node);
            }

            CreateEdgesBetweenRectangleNodes(boardLimits, rectangleToNode);

            Dictionary<FIRRTLNode, Node<FIRRTLNode>> firrtlNodeToGraphNode = new Dictionary<FIRRTLNode, Node<FIRRTLNode>>();
            foreach (var node in rectangleGraph.Nodes)
            {
                if (node.Value == null)
                {
                    continue;
                }

                firrtlNodeToGraphNode.Add(node.Value, node);
            }

            var graph = new RectangleGraph(rectangleGraph, firrtlNodeToGraphNode, boardLimits);


            // Create graph -- Done
            // Get all lines
            // Path in graph
            // Space rectangles so there is space for wires
        }

        private static void CreateEdgesBetweenRectangleNodes(Rectangle boardLimits, Dictionary<Rectangle, Node<FIRRTLNode>> rectangleToNode)
        {
            Dictionary<Point, Node<FIRRTLNode>> pointToNode = new Dictionary<Point, Node<FIRRTLNode>>();
            foreach (var rectangleWithNode in rectangleToNode)
            {
                foreach (var rectanglePoint in GetPointsInsideRectangle(rectangleWithNode.Key, 0))
                {
                    pointToNode.Add(rectanglePoint, rectangleWithNode.Value);
                }
            }

            foreach (var rectangleWithNode in rectangleToNode)
            {
                foreach (var rectanglePoint in GetPointsInsideRectangle(rectangleWithNode.Key, 1))
                {
                    if (!boardLimits.Within(rectanglePoint))
                    {
                        continue;
                    }

                    Node<FIRRTLNode> adjacantNode = pointToNode[rectanglePoint];
                    rectangleWithNode.Value.AddEdgeTo(adjacantNode);
                }
            }
        }

        private static List<Point> GetRectangleCorners(HashSet<Point> rectangleEdgePoints)
        {
            List<Point> corners = new List<Point>();
            foreach (var point in rectangleEdgePoints)
            {
                bool connectedUp = rectangleEdgePoints.Contains(MoveDirs.Up.MovePoint(point));
                bool connectedDown = rectangleEdgePoints.Contains(MoveDirs.Down.MovePoint(point));
                bool connectedLeft = rectangleEdgePoints.Contains(MoveDirs.Left.MovePoint(point));
                bool connectedRight = rectangleEdgePoints.Contains(MoveDirs.Right.MovePoint(point));

                if ((connectedUp && connectedLeft) ||
                    (connectedUp && connectedRight) ||
                    (connectedDown && connectedLeft) ||
                    (connectedDown && connectedRight))
                {
                    corners.Add(point);
                }
            }

            return corners;
        }

        private static void ExpandVectors(HashSet<Point> rectangleEdgePoints, List<Vector> expandVectors, Rectangle boardLimits)
        {


            List<Vector> nextRoundVectors = new List<Vector>();
            while (expandVectors.Count > 0)
            {
                foreach (var vector in expandVectors)
                {
                    Point moved = vector.GetMovePoint();
                    if (!boardLimits.Within(moved))
                    {
                        continue;
                    }

                    if (!rectangleEdgePoints.Add(moved))
                    {
                        continue;
                    }

                    nextRoundVectors.Add(vector.Move());
                }

                var tmp = expandVectors;
                expandVectors = nextRoundVectors;
                nextRoundVectors = tmp;

                nextRoundVectors.Clear();
            }
        }

        private static Rectangle GetBoardSize(List<Vector> expandVectors)
        {
            Point minPoint = expandVectors.Select(x => x.Position).Min();
            Point maxPoint = expandVectors.Select(x => x.Position).Max();
            Point size = maxPoint - minPoint;
            Rectangle boardLimits = new Rectangle(minPoint, size);
            return boardLimits;
        }

        private static List<Vector> GetRectangleExpandVectors(PlacementInfo placement)
        {
            List<Vector> expandVectors = new List<Vector>();
            foreach (var rectangle in placement.UsedSpace.Values)
            {
                expandVectors.Add(new Vector(rectangle.Pos, new Point(0, -1)));
                expandVectors.Add(new Vector(rectangle.Pos, new Point(-1, 0)));

                expandVectors.Add(new Vector(rectangle.Pos + new Point(0, rectangle.Height - 1), new Point(0, 1)));
                expandVectors.Add(new Vector(rectangle.Pos + new Point(0, rectangle.Height - 1), new Point(-1, 0)));

                expandVectors.Add(new Vector(rectangle.Pos + new Point(rectangle.Width - 1, 0), new Point(0, -1)));
                expandVectors.Add(new Vector(rectangle.Pos + new Point(rectangle.Width - 1, 0), new Point(1, 0)));

                expandVectors.Add(new Vector(rectangle.Pos + new Point(rectangle.Width - 1, rectangle.Height - 1), new Point(0, 1)));
                expandVectors.Add(new Vector(rectangle.Pos + new Point(rectangle.Width - 1, rectangle.Height - 1), new Point(1, 0)));
            }

            return expandVectors;
        }

        private static HashSet<Point> GetRactangleEdgePoints(IEnumerable<Rectangle> rectangles)
        {
            HashSet<Point> rectangleEdgePoints = new HashSet<Point>();
            foreach (var rectangle in rectangles)
            {
                rectangleEdgePoints.IntersectWith(GetPointsInsideRectangle(rectangle, 0));
            }

            return rectangleEdgePoints;
        }

        private static IEnumerable<Point> GetPointsInsideRectangle(Rectangle rectangle, int extraDistanceFromCenter)
        {
            for (int x = 0; x < rectangle.Width; x++)
            {
                // Top horizontal
                yield return rectangle.Pos + new Point(x, -extraDistanceFromCenter);
                // Bottom horizontal
                yield return rectangle.Pos + new Point(x, rectangle.Height - 1 + extraDistanceFromCenter);
            }

            for (int y = 0; y < rectangle.Height; y++)
            {
                // Left vertical
                yield return rectangle.Pos + new Point(-extraDistanceFromCenter, y);
                // Right vertical
                yield return rectangle.Pos + new Point(rectangle.Width - 1 + extraDistanceFromCenter, y);
            }
        }

        private sealed record Vector(Point Position, Point Direction)
        {
            public Point GetMovePoint()
            {
                return Position + Direction;
            }

            public Vector Move()
            {
                return new Vector(GetMovePoint(), Direction);
            }
        }
    }
}
