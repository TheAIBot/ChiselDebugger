using ChiselDebug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerWebUI.Code
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
