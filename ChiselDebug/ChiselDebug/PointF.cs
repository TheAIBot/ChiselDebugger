using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebug
{
    public struct PointF
    {
        public float X;
        public float Y;

        public PointF(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public override bool Equals(object obj)
        {
            return obj is PointF point &&
                   X == point.X &&
                   Y == point.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return $"[{X}, {Y}]";
        }

        public static PointF operator +(PointF a, PointF b)
        {
            return new PointF(a.X + b.X, a.Y + b.Y);
        }

        public static PointF operator -(PointF a, PointF b)
        {
            return new PointF(a.X - b.X, a.Y - b.Y);
        }
        public static PointF operator -(PointF a)
        {
            return new PointF(-a.X, -a.Y);
        }

        public static PointF operator *(PointF a, float b)
        {
            return new PointF(a.X * b, a.Y * b);
        }

        public static bool operator ==(PointF left, PointF right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(PointF left, PointF right)
        {
            return !(left == right);
        }

        public static PointF Max(PointF a, PointF b)
        {
            return new PointF(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
        }

        public static PointF Min(PointF a, PointF b)
        {
            return new PointF(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
        }

        public PointF Abs()
        {
            return new PointF(Math.Abs(X), Math.Abs(Y));
        }
    }
}
