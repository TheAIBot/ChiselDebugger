using System;
using VCDReader;

namespace ChiselDebug.Timeline
{
    internal readonly record struct TimeStepChanges(ulong Time, int StartIndex, int Length)
    {
        internal ReadOnlySpan<BinaryVarValue> GetChanges(BinaryVarValue[] changes)
        {
            return changes.AsSpan(StartIndex, Length);
        }
    }
}
