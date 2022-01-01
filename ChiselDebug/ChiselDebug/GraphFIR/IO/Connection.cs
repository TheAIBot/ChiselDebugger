using System;

namespace ChiselDebug.GraphFIR.IO
{
    public readonly struct Connection
    {
        public readonly Source From;
        public readonly Source Condition;

        public Connection(Source from)
        {
            this.From = from;
            this.Condition = null;
        }

        public Connection(Source from, Source condition)
        {
            this.From = from;
            this.Condition = condition;
        }

        public bool IsEnabled()
        {
            return Condition.Value.IsTrue();
        }

        public override bool Equals(object obj)
        {
            return obj is Connection other &&
                From == other.From &&
                Condition == other.Condition;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(From, Condition);
        }

        public static bool operator ==(Connection a, Connection b)
        {
            return a.From == b.From && a.Condition == b.Condition;
        }

        public static bool operator !=(Connection a, Connection b)
        {
            return !(a == b);
        }
    }
}
