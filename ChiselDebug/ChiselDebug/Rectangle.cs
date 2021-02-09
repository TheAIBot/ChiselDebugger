﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerWebUI.Code
{
    public readonly struct Rectangle
    {
        public readonly Point Pos;
        public readonly int Width;
        public readonly int Height;

        public int LeftX => Pos.X;
        public int TopY => Pos.Y;

        public int RightX => Pos.X + Width;

        public int BottomY => Pos.Y + Height;


        public Rectangle(int x, int y, int width, int height) : this(new Point(x, y), width, height)
        {
        }

        public Rectangle(Point pos, int width, int height)
        {
            this.Pos = pos;
            this.Width = width;
            this.Height = height;
        }

        public override bool Equals(object obj)
        {
            return obj is Rectangle rectangle &&
                   Pos == rectangle.Pos &&
                   Width == rectangle.Width &&
                   Height == rectangle.Height;
        }

        public static bool operator ==(Rectangle left, Rectangle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Rectangle left, Rectangle right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Pos, Width, Height);
        }
    }
}