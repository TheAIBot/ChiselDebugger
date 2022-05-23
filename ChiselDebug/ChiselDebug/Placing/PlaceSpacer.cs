using ChiselDebug.Routing;
using ChiselDebug.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.Placing
{
    public sealed class PlaceSpacer
    {
        public void lol(PlacementInfo placement)
        {
            HashSet<Point> rectangleEdgePoints = GetRactangleEdgePoints(placement);

            List<Vector> expandVectors = GetRectangleExpandVectors(placement);
            ExpandVectors(rectangleEdgePoints, expandVectors);

            List<Point> corners = GetRectangleCorners(rectangleEdgePoints);

            Dictionary<int, List<int>> horizontalLineMaker = CreateHorizontalLineMaker(corners);
            Dictionary<int, List<int>> verticalLineMaker = CreateVerticalLineMaker(corners);

            Dictionary<int, List<Line>> horizontalLines = CreateHorizontalLines(horizontalLineMaker);
            Dictionary<int, List<Line>> verticalLines = CreateVerticalLines(verticalLineMaker);

            List<Rectangle> rectanglesOnBoard = CreateRectanglesFromHorizontalAndVerticalLines(horizontalLines, verticalLines);

            List<Rectangle> nonOccupiedRectangles = rectanglesOnBoard.Except(placement.UsedSpace.Values).ToList();

            // Create graph
            // Get all lines
            // Path in graph
            // Space rectangles so there is space for wires
        }

        private static List<Rectangle> CreateRectanglesFromHorizontalAndVerticalLines(Dictionary<int, List<Line>> horizontalLines, Dictionary<int, List<Line>> verticalLines)
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

        private static Dictionary<int, List<Line>> CreateHorizontalLines(Dictionary<int, List<int>> lineMaker)
        {
            return CreateLines(lineMaker, (x, y) => new Point(x, y));
        }

        private static Dictionary<int, List<Line>> CreateVerticalLines(Dictionary<int, List<int>> lineMaker)
        {
            return CreateLines(lineMaker, (x, y) => new Point(y, x));
        }

        private static Dictionary<int, List<Line>> CreateLines(Dictionary<int, List<int>> lineMaker, Func<int, int, Point> valuesToPoint)
        {
            Dictionary<int, List<Line>> axisLines = new Dictionary<int, List<Line>>();
            foreach (var horizontal in lineMaker)
            {
                Func<int, Point> axisValueToPoint = x => valuesToPoint(horizontal.Key, x);
                var startLinePoints = horizontal.Value.Skip(1).Select(x => axisValueToPoint(x));
                var endLinePoints = horizontal.Value.SkipLast(1).Select(x => axisValueToPoint(x));
                var lines = startLinePoints.Zip(endLinePoints).Select(x => new Line(x.First, x.Second)).ToList();
                axisLines.Add(horizontal.Key, lines);
            }

            return axisLines;
        }

        private static Dictionary<int, List<int>> CreateHorizontalLineMaker(List<Point> corners)
        {
            return CreateLineMaker(corners, x => x.Y, x => x.X);
        }

        private static Dictionary<int, List<int>> CreateVerticalLineMaker(List<Point> corners)
        {
            return CreateLineMaker(corners, x => x.X, x => x.Y);
        }

        private static Dictionary<int, List<int>> CreateLineMaker(List<Point> corners, Func<Point, int> groupOnAxis, Func<Point, int> axis)
        {
            Dictionary<int, List<int>> lineMaker = new Dictionary<int, List<int>>();
            foreach (var corner in corners)
            {
                List<int> axisGroup;
                if (!lineMaker.TryGetValue(groupOnAxis(corner), out axisGroup))
                {
                    axisGroup = new List<int>();
                    lineMaker.Add(groupOnAxis(corner), axisGroup);
                }

                axisGroup.Add(axis(corner));
            }

            foreach (var horizontal in lineMaker.Values)
            {
                horizontal.Sort();
            }

            return lineMaker;
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

        private static void ExpandVectors(HashSet<Point> rectangleEdgePoints, List<Vector> expandVectors)
        {
            Rectangle boardLimits = GetBoardSize(expandVectors);

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

        private static HashSet<Point> GetRactangleEdgePoints(PlacementInfo placement)
        {
            HashSet<Point> rectangleEdgePoints = new HashSet<Point>();
            foreach (var rectangle in placement.UsedSpace.Values)
            {
                for (int x = 0; x < rectangle.Width; x++)
                {
                    rectangleEdgePoints.Add(rectangle.Pos + new Point(x, 0));
                    rectangleEdgePoints.Add(rectangle.Pos + new Point(x, rectangle.Height - 1));
                }

                for (int y = 0; y < rectangle.Height; y++)
                {
                    rectangleEdgePoints.Add(rectangle.Pos + new Point(0, y));
                    rectangleEdgePoints.Add(rectangle.Pos + new Point(rectangle.Width - 1, y));
                }
            }

            return rectangleEdgePoints;
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
