namespace ChiselDebug.GraphFIR.IO
{
    public readonly struct Connection
    {
        public readonly Output From;
        public readonly Output Condition;

        public Connection(Output from)
        {
            this.From = from;
            this.Condition = null;
        }

        public Connection(Output from, Output condition)
        {
            this.From = from;
            this.Condition = condition;
        }

        public bool IsEnabled()
        {
            return Condition.Value.IsTrue();
        }
    }
}
