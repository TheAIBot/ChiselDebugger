﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerWebUI.Code
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
    }
}
