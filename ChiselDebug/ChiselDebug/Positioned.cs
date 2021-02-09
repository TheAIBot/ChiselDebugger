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

        public Positioned<U> Cast<U>()
        {
            if (Value is U casted)
            {
                return new Positioned<U>(Position, casted);
            }
            else
            {
                throw new InvalidCastException($"Failed to cast from {typeof(T)} to {typeof(U)}.");
            }
        }

        public override string ToString()
        {
            return $"{Position} {Value}";
        }
    }
}
