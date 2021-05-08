using System;
using System.Collections.Generic;
using VCDReader;

namespace ChiselDebug.Timeline
{
    internal struct TimeStepChanges
    {
        public readonly ulong Time;
        public readonly int StartIndex;
        public readonly int Length;

        public TimeStepChanges(ulong time, int startIndex, int length)
        {
            this.Time = time;
            this.StartIndex = startIndex;
            this.Length = length;
        }

        internal ReadOnlySpan<BinaryVarValue> GetChanges(BinaryVarValue[] changes)
        {
            return changes.AsSpan(StartIndex, Length);
        }
    }
}
