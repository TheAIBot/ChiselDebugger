using System;

namespace ChiselDebug
{
    public sealed class ChiselDebugException : Exception
    {
        public ChiselDebugException(string message) : base(message)
        { }
    }
}
