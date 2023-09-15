using System;
using System.Collections.Generic;
using System.Linq;
using VCDReader;

namespace ChiselDebugTests
{
    public class WireIgnoreTracker
    {
        private int TotalWireStates = 0;
        private int IgnoredBecauseUnknown = 0;
        private int IgnoredBecauseNotExist = 0;
        private int IgnoredBecauseMemReadDelay = 0;

        private readonly Dictionary<VarDef, int> IgnoreNotExistNames = new Dictionary<VarDef, int>();
        private readonly Dictionary<VarDef, int> IgnoreMemReadDelayNames = new Dictionary<VarDef, int>();

        public void AddWireState()
        {
            TotalWireStates++;
        }

        public void IgnoreBecauseUnknown(VarDef variable)
        {
            IgnoredBecauseUnknown++;
        }

        public void IgnoreBecauseNotExist(VarDef variable)
        {
            IgnoredBecauseNotExist++;

            if (!IgnoreNotExistNames.TryAdd(variable, 1))
            {
                IgnoreNotExistNames[variable] += 1;
            }
        }

        public void IgnorebecauseMemReadDelay(VarDef variable)
        {
            IgnoredBecauseMemReadDelay++;

            if (!IgnoreMemReadDelayNames.TryAdd(variable, 1))
            {
                IgnoreMemReadDelayNames[variable] += 1;
            }
        }

        public void WriteToConsole()
        {
            Console.WriteLine($"Wire states: {TotalWireStates:N0}");
            Console.WriteLine($"Ignored W: {IgnoredBecauseUnknown:N0}");

            Console.WriteLine($"Ignored Not exist: {IgnoredBecauseNotExist:N0}");
            foreach (var nameCount in IgnoreNotExistNames.OrderByDescending(x => x.Value))
            {
                Console.WriteLine($"\t{nameCount.Value,10:N0} : {nameCount.Key.RefNameWithScopes()}");
            }

            Console.WriteLine($"Ignored Mem read delay: {IgnoredBecauseMemReadDelay:N0}");
            foreach (var nameCount in IgnoreMemReadDelayNames.OrderByDescending(x => x.Value))
            {
                Console.WriteLine($"\t{nameCount.Value,10:N0} : {nameCount.Key.RefNameWithScopes()}");
            }
        }
    }
}
