﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebug
{
    public readonly struct Line
    {
        public readonly Point Start;
        public readonly Point End;

        public Line(Point start, Point end)
        {
            this.Start = start;
            this.End = end;
        }

        public int GetManhattanDistance()
        {
            Point diff = (End - Start).Abs();
            return diff.X + diff.Y;
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
