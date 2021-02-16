namespace ChiselDebug.GraphFIR
{
    public readonly struct SizedType
    {
        private const int WIDTH_NOT_KNOWN = -1;

        public readonly GroundType GType;
        public readonly int Width;
        public bool IsWidthKnown { get { return Width != WIDTH_NOT_KNOWN; } }


        public SizedType(GroundType type, int width)
        {
            this.GType = type;
            this.Width = width;
        }

        public SizedType(GroundType type)
        {
            this.GType = type;
            this.Width = WIDTH_NOT_KNOWN;
        }
    }
}
