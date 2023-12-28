using System;

namespace ChiselDebug.Utilities
{
    public record struct Point(int X, int Y)
    {
        public static readonly Point Zero = new Point(0, 0);

        public bool ApproxEquals(Point other, int allowedDelta)
        {
            return X - allowedDelta <= other.X && other.X <= X + allowedDelta &&
                   Y - allowedDelta <= other.Y && other.Y <= Y + allowedDelta;
        }

        public static Point operator +(Point a, Point b)
        {
            return new Point(a.X + b.X, a.Y + b.Y);
        }

        public static Point operator -(Point a, Point b)
        {
            return new Point(a.X - b.X, a.Y - b.Y);
        }
        public static Point operator -(Point a)
        {
            return new Point(-a.X, -a.Y);
        }

        public static Point operator *(Point a, int b)
        {
            return new Point(a.X * b, a.Y * b);
        }

        public static Point operator /(Point a, float b)
        {
            return new Point((int)(a.X / b), (int)(a.Y / b));
        }

        public static Point Max(Point a, Point b)
        {
            return new Point(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
        }

        public static Point Min(Point a, Point b)
        {
            return new Point(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
        }

        public static int ManhattanDistance(Point a, Point b)
        {
            // The sum of the two absolute values may overflow in which case max int
            // is the desired value. int.CreateSaturating handles the overflow logic.
            return int.CreateSaturating(Math.Abs((long)a.X - b.X) + Math.Abs((long)a.Y - b.Y));
        }

        public Point Abs()
        {
            return new Point(Math.Abs(X), Math.Abs(Y));
        }
    }
}
