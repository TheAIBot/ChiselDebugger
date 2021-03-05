using System.Collections.Generic;
using VCDReader;

namespace ChiselDebug.Timeline
{
    internal class TimeStepChanges
    {
        public readonly ulong Time;
        public readonly List<VarValue> Changes = new List<VarValue>();

        public TimeStepChanges(ulong time)
        {
            this.Time = time;
        }
    }
}
