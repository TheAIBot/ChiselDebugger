using ChiselDebuggerRazor.Code.Controllers;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ChiselDebuggerRazor.Code
{
    internal sealed class PlaceAndRouteStats
    {
        private int TotalPlaceCount = 0;
        private int CurrentPlaceCount = 0;
        private int TotalRouteCount = 0;
        private int CurrentRouteCount = 0;
        private DateTime StartTime;
        private DateTime EndTime;
        private readonly ConcurrentDictionary<ModuleLayout, bool> HasSeenPlacing = new ConcurrentDictionary<ModuleLayout, bool>();
        private readonly ConcurrentDictionary<ModuleLayout, bool> HasSeenRouting = new ConcurrentDictionary<ModuleLayout, bool>();

        public void IncrementNeedToPlace()
        {
            IncrementTotalStat(ref TotalPlaceCount);
        }

        public void IncrementNeedToRoute()
        {
            IncrementTotalStat(ref TotalRouteCount);
        }

        private void IncrementTotalStat(ref int stat)
        {
            int value = Interlocked.Increment(ref stat);

            //Setting StartTime is not thread safe but that
            //shouldn't be a problem because the time should
            //be almost the same in such cases.
            if (value == 1)
            {
                StartTime = DateTime.Now;
            }
        }

        public void PlaceDone(ModuleLayout mod)
        {
            if (HasSeenPlacing.TryAdd(mod, true))
            {
                Interlocked.Increment(ref CurrentPlaceCount);
            }

            UpdateIfAllDone();
        }

        public void RouteDone(ModuleLayout mod)
        {
            if (HasSeenRouting.TryAdd(mod, true))
            {
                Interlocked.Increment(ref CurrentRouteCount);
            }

            UpdateIfAllDone();
        }

        private void UpdateIfAllDone()
        {
            if (CurrentPlaceCount >= TotalPlaceCount &&
                CurrentRouteCount >= TotalRouteCount)
            {
                EndTime = DateTime.Now;
                TimeSpan time = EndTime - StartTime;

                Console.WriteLine($"Placed: {CurrentPlaceCount}");
                Console.WriteLine($"Routed: {CurrentRouteCount}");
                Console.WriteLine($"Time: {time.TotalMilliseconds:N0}ms");
            }
        }
    }
}
