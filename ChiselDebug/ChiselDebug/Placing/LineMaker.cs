using ChiselDebug.Routing;
using ChiselDebug.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.Placing
{
    internal sealed class LineMaker : ILineMaker
    {
        private readonly Func<Point, Point> Transformation;

        public LineMaker(Func<Point, Point> transformation)
        {
            Transformation = transformation;
        }

        public AxisLineContainer CreateLines(List<Point> corners)
        {
            Dictionary<int, List<int>> axisPointPoints = CreateAxisPoints(corners);
            Dictionary<int, List<Line>> axisPointLines = CreateAxisPointLines(axisPointPoints);

            return new AxisLineContainer(axisPointLines);
        }

        private Dictionary<int, List<int>> CreateAxisPoints(List<Point> corners)
        {
            Dictionary<int, List<int>> lineMaker = new Dictionary<int, List<int>>();
            foreach (var corner in corners)
            {
                List<int> axisGroup;
                Point transformedCorner = Transformation(corner);
                if (!lineMaker.TryGetValue(transformedCorner.X, out axisGroup))
                {
                    axisGroup = new List<int>();
                    lineMaker.Add(transformedCorner.X, axisGroup);
                }

                axisGroup.Add(transformedCorner.Y);
            }

            foreach (var horizontal in lineMaker.Values)
            {
                horizontal.Sort();
            }

            return lineMaker;
        }

        private Dictionary<int, List<Line>> CreateAxisPointLines(Dictionary<int, List<int>> lineMaker)
        {
            Dictionary<int, List<Line>> axisLines = new Dictionary<int, List<Line>>();
            foreach (var horizontal in lineMaker)
            {
                Func<int, Point> axisValueToPoint = x => Transformation(new Point(horizontal.Key, x));
                var startLinePoints = horizontal.Value.SkipLast(1).Select(x => axisValueToPoint(x));
                var endLinePoints = horizontal.Value.Skip(1).Select(x => axisValueToPoint(x));
                var lines = startLinePoints.Zip(endLinePoints).Select(x => new Line(x.First, x.Second)).ToList();
                axisLines.Add(horizontal.Key, lines);
            }

            return axisLines;
        }
    }
}
