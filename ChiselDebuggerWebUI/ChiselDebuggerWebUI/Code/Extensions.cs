﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerWebUI
{
    public static class Extensions
    {
        public static string ToPixels(this int value)
        {
            return value + "px";
        }
    }
}
