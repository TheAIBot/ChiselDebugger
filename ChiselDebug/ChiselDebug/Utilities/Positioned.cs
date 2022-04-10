using System;

namespace ChiselDebug.Utilities
{
    public readonly record struct Positioned<T>(Point Position, T Value)
    {
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
