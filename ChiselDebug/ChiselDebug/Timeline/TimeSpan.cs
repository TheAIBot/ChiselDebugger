namespace ChiselDebug.Timeline
{
    public class TimeSpan
    {
        public readonly ulong StartInclusive;
        public readonly ulong EndExclusive;

        public TimeSpan(ulong start, ulong end)
        {
            this.StartInclusive = start;
            this.EndExclusive = end;
        }

        public ulong InclusiveEnd()
        {
            return EndExclusive - 1;
        }

        public bool IsTimeInTimeSpan(ulong time)
        {
            return StartInclusive <= time && time < EndExclusive;
        }
    }
}
