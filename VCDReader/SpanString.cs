using System;
using System.Linq;

namespace VCDReader
{
    internal readonly struct SpanString : IEquatable<SpanString>
    {
        private readonly ReadOnlyMemory<char> SpanStr;
        public ReadOnlySpan<char> Span => SpanStr.Span;

        public SpanString(string str)
        {
            this.SpanStr = str.AsMemory();
        }

        public SpanString(ReadOnlyMemory<char> spanStr)
        {
            this.SpanStr = spanStr;
        }

        public bool Equals(SpanString other)
        {
            return other.SpanStr.Span.SequenceEqual(SpanStr.Span);
        }

        public override int GetHashCode()
        {
            return string.GetHashCode(SpanStr.Span);
        }
    }
}
