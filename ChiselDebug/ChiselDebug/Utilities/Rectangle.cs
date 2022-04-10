using System;

namespace ChiselDebug.Utilities
{
    public readonly record struct Rectangle(Point Pos, int Width, int Height)
    {
        public int LeftX => Pos.X;
        public int TopY => Pos.Y;
        public int RightX => Pos.X + Width;
        public int BottomY => Pos.Y + Height;
        public Point Size => new Point(Width, Height);

        public Rectangle(Point pos, Point size) : this(pos, size.X, size.Y)
        { }

        public bool Within(Point pos)
        {
            return LeftX <= pos.X && pos.X <= RightX &&
                   TopY <= pos.Y && pos.Y <= BottomY;
        }
    }
}
