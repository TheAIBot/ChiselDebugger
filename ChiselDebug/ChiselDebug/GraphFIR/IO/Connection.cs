namespace ChiselDebug.GraphFIR.IO
{
    public readonly record struct Connection(Source From, Source Condition)
    {
        public Connection(Source from) : this(from, null)
        { }

        public bool IsEnabled()
        {
            return Condition.Value.IsTrue();
        }
    }
}
