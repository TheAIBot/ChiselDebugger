using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChiselDebug;

namespace ChiselDebuggerWebUI.Code
{
    public readonly struct Positioned<T>
    {
        public readonly Point Position;
        public readonly T Value;

        public Positioned(Point position, T value)
        {
            this.Position = position;
            this.Value = value;
        }

        public override string ToString()
        {
            return $"{Position.ToString()} {Value.ToString()}";
        }
    }
}
