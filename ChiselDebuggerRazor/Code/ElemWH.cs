using ChiselDebug.Utilities;
using System;

namespace ChiselDebuggerRazor.Code
{
    public record ElemWH(double Width, double Height)
    {
        public Point ToPoint()
        {
            return new Point((int)Math.Round(Width), (int)Math.Round(Height));
        }
    }
}
