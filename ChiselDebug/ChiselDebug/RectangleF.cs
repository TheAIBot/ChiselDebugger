using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebug
{
    public readonly struct RectangleF
    {
        public readonly PointF Pos;
        public readonly float Width;
        public readonly float Height;

        public float LeftX => Pos.X;
        public float TopY => Pos.Y;

        public float RightX => Pos.X + Width;

        public float BottomY => Pos.Y + Height;

        public PointF Size => new PointF(Width, Height);


        public RectangleF(float x, float y, float width, float height) : this(new PointF(x, y), width, height)
        { }

        public RectangleF(PointF pos, float width, float height)
        {
            this.Pos = pos;
            this.Width = width;
            this.Height = height;
        }

        public RectangleF(PointF pos, PointF size) : this(pos, size.X, size.Y)
        { }

        public bool Within(PointF pos)
        {
            return LeftX <= pos.X && pos.X <= RightX &&
                   TopY <= pos.Y && pos.Y <= BottomY;
        }

        public bool WithinButExcludeRectEdge(PointF pos)
        {
            return LeftX < pos.X && pos.X < RightX &&
                   TopY < pos.Y && pos.Y < BottomY;
        }

        public override bool Equals(object obj)
        {
            return obj is RectangleF rectangle &&
                   Pos == rectangle.Pos &&
                   Width == rectangle.Width &&
                   Height == rectangle.Height;
        }

        public static bool operator ==(RectangleF left, RectangleF right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RectangleF left, RectangleF right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Pos, Width, Height);
        }
    }
}
