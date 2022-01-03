using System;

namespace ChiselDebug
{
    public class ChiselDebugException : Exception
    {
        public ChiselDebugException(string message) : base(message)
        { }
    }
}
