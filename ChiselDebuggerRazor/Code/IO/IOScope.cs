namespace ChiselDebuggerRazor.Code.IO
{
    internal sealed class IOScope
    {
        internal readonly string ScopeColor;
        internal readonly int XStart;
        internal readonly int YStart;
        internal readonly int Width = IOPositionCalc.ScopeWidth;
        internal readonly int Height;

        public IOScope(string color, int xStart, int yStart, int yEnd)
        {
            ScopeColor = color;
            XStart = xStart;
            YStart = yStart - IOPositionCalc.ScopeExtraY;
            Height = yEnd + IOPositionCalc.ScopeExtraY - YStart;
        }
    }
}
