namespace ChiselDebuggerWebUI.Code
{
    internal class IOScope
    {
        internal readonly string ScopeColor;
        internal readonly int XStart;
        internal readonly int YStart;
        internal readonly int Width = IOPositionCalc.ScopeWidth;
        internal readonly int Height;

        public IOScope(string color, int xStart, int yStart, int yEnd)
        {
            this.ScopeColor = color;
            this.XStart = xStart;
            this.YStart = yStart - IOPositionCalc.ScopeExtraY;
            this.Height = (yEnd + IOPositionCalc.ScopeExtraY) - YStart;
        }
    }
}
