using System.Globalization;

namespace ChiselDebuggerRazor.Code
{
    public static class Extensions
    {
        public static string ToPixels(this int value)
        {
            return $"{value}px";
        }

        public static string ToPixels(this float value)
        {
            return string.Create(CultureInfo.InvariantCulture, $"{value}px");
        }

        public static string ToPercent(this float value)
        {
            return string.Create(CultureInfo.InvariantCulture, $"{value}%");
        }

        public static string ToHtmlNumber(this float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToStringWithDots(this ulong value)
        {
            return value.ToString("N0", CultureInfo.InvariantCulture);
        }
    }
}
