using ChiselDebug.Utilities;
using System;
using System.Collections.Generic;

namespace ChiselDebug.Routing
{
    public readonly struct Line
    {
        public readonly Point Start;
        public readonly Point End;

        public Line(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        public int GetManhattanDistance()
        {
            return Point.ManhattanDistance(Start, End);
        }

        public override bool Equals(object obj)
        {
            return obj is Line line &&
                   EqualityComparer<Point>.Default.Equals(Start, line.Start) &&
                   EqualityComparer<Point>.Default.Equals(End, line.End);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Start, End);
        }
    }
}
