using System;

namespace ChiselDebug.GraphFIR.IO
{
    public readonly record struct Connection(Source From, Source? Condition)
    {
        public Connection(Source from) : this(from, null)
        { }

        public bool IsEnabled()
        {
            ValueType? conditionValue = Condition?.Value;
            if (conditionValue == null)
            {
                throw new InvalidOperationException(" Connection is either not condition or condition had no value.");
            }

            return conditionValue.IsTrue();
        }
    }
}
