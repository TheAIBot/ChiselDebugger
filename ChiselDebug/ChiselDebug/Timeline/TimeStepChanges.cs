using System.Collections.Generic;
using VCDReader;

namespace ChiselDebug.Timeline
{
    internal class TimeStepChanges
    {
        public readonly ulong Time;
        public readonly List<BinaryVarValue> Changes = new List<BinaryVarValue>();

        public TimeStepChanges(ulong time)
        {
            this.Time = time;
        }
    }
}
