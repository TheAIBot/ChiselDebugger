using System;
using System.Runtime.InteropServices;

namespace VCDReader
{
    public readonly struct UnsafeMemory<T>
    {
        private readonly T[] Arr;
        private readonly int StartIndex;
        public readonly int Length;
        private static readonly T[] LengthOne = new T[1];

        public Span<T> Span => GetSpan();

        public static implicit operator UnsafeMemory<T>(T[] arr)
        {
            return new UnsafeMemory<T>(arr, 0, arr.Length);
        }

        public UnsafeMemory(T[] arr, int startIndex, int length)
        {
            if (length == 0)
            {
                this.Arr = LengthOne;
                this.StartIndex = 0;
            }
            else
            {
                this.Arr = arr;
                this.StartIndex = startIndex;
            }
            this.Length = length;
        }

        public Span<T> GetSpan()
        {
            return MemoryMarshal.CreateSpan(ref Arr[StartIndex], Length);
        }

        public UnsafeMemory<T> Slice(int start)
        {
            return Slice(start, Length - start);
        }

        public UnsafeMemory<T> Slice(int start, int length)
        {
            return new UnsafeMemory<T>(Arr, StartIndex + start, length);
        }
    }
}
