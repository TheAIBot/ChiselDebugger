using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using VCDReader;

[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.MethodLevel)]

namespace ChiselDebugTests
{
    public class WireIgnoreTracker
    {
        private int TotalWireStates = 0;
        private int IgnoredBecauseUnknown = 0;
        private int IgnoredBecauseNotExist = 0;
        private int IgnoredBecauseMemReadDelay = 0;

        private readonly Dictionary<string, int> IgnoreNotExistNames = new Dictionary<string, int>();
        private readonly Dictionary<string, int> IgnoreMemReadDelayNames = new Dictionary<string, int>();

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

            string scopedName = variable.RefNameWithScopes();
            if (!IgnoreNotExistNames.TryAdd(scopedName, 1))
            {
                IgnoreNotExistNames[scopedName] += 1;
            }
        }

        public void IgnorebecauseMemReadDelay(VarDef variable)
        {
            IgnoredBecauseMemReadDelay++;

            string scopedName = variable.RefNameWithScopes();
            if (!IgnoreMemReadDelayNames.TryAdd(scopedName, 1))
            {
                IgnoreMemReadDelayNames[scopedName] += 1;
            }
        }

        public void WriteToConsole()
        {
            Console.WriteLine($"Wire states: {TotalWireStates.ToString("N0")}");
            Console.WriteLine($"Ignored W: {IgnoredBecauseUnknown.ToString("N0")}");

            Console.WriteLine($"Ignored Not exist: {IgnoredBecauseNotExist.ToString("N0")}");
            foreach (var nameCount in IgnoreNotExistNames.OrderByDescending(x => x.Value))
            {
                Console.WriteLine($"\t{nameCount.Value.ToString("N0").PadLeft(10)} : {nameCount.Key}");
            }

            Console.WriteLine($"Ignored Mem read delay: {IgnoredBecauseMemReadDelay.ToString("N0")}");
            foreach (var nameCount in IgnoreMemReadDelayNames.OrderByDescending(x => x.Value))
            {
                Console.WriteLine($"\t{nameCount.Value.ToString("N0").PadLeft(10)} : {nameCount.Key}");
            }
        }
    }
}
