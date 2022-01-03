using System;

namespace ChiselDebug.Utilities
{
    internal struct RefEnabledList<T>
    {
        private T[] Values;
        public int Count { get; private set; }

        public readonly ref T this[int index] { get { return ref Values[index]; } }

        public RefEnabledList()
        {
            Values = Array.Empty<T>();
            Count = 0;
        }

        public RefEnabledList(int capacity)
        {
            Values = new T[capacity];
            Count = 0;
        }

        public void Add(T value)
        {
            Add(ref value);
        }

        public void Add(ref T value)
        {
            EnsureCanFitOneMoreValue();

            Values[Count++] = value;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException(nameof(index));
            }

            if (index == Count - 1)
            {
                Count--;
                return;
            }

            Array.Copy(Values, index + 1, Values, index, Count - index - 1);
            Count--;
        }

        public void Clear()
        {
            Count = 0;
        }

        public T[] ToArray()
        {
            T[] copy = new T[Count];
            Array.Copy(Values, copy, Count);
            return copy;
        }

        private void EnsureCanFitOneMoreValue()
        {
            if (Count == Values.Length)
            {
                Array.Resize(ref Values, Math.Max(1, Values.Length * 2));
            }
        }
    }
}
