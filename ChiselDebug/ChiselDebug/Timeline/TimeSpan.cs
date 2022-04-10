namespace ChiselDebug.Timeline
{
    public readonly record struct TimeSpan(ulong StartInclusive, ulong EndExclusive)
    {
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
