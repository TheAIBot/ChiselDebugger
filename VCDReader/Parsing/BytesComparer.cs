using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace VCDReader.Parsing
{
    internal sealed class BytesComparer : IEqualityComparer<byte[]>, IAlternateEqualityComparer<ReadOnlySpan<byte>, byte[]>
    {
        public static readonly BytesComparer Default = new BytesComparer();

        public byte[] Create(ReadOnlySpan<byte> alternate)
        {
            return alternate.ToArray();
        }

        public bool Equals(byte[]? x, byte[]? y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return Equals(x.AsSpan(), y);
        }

        public bool Equals(ReadOnlySpan<byte> alternate, byte[] other)
        {
            return alternate.SequenceEqual(other);
        }

        public int GetHashCode([DisallowNull] byte[] obj)
        {
            return GetHashCode(obj.AsSpan());
        }

        public int GetHashCode(ReadOnlySpan<byte> alternate)
        {
            var hashCode = new HashCode();
            for (int i = 0; i < alternate.Length; i++)
            {
                hashCode.Add(alternate[i]);
            }

            return hashCode.ToHashCode();
        }
    }
}
