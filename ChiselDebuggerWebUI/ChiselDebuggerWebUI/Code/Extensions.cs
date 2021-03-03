using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerWebUI.Code
{
    public static class Extensions
    {
        public static string ToPixels(this int value)
        {
            return value + "px";
        }

        public static string ToPercent(this float value)
        {
            return value.ToString(CultureInfo.InvariantCulture) + "%";
        }

        public static string ToHtmlNumber(this float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
