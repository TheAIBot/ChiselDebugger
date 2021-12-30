using ChiselDebug.Utilities;
using System;

namespace ChiselDebuggerRazor.Code
{
    public class ElemWH
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public Point ToPoint()
        {
            return new Point((int)Math.Round(Width), (int)Math.Round(Height));
        }
    }
}
