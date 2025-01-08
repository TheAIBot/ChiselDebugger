using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace VCDReader.Parsing
{
    internal sealed class IDToVarDef
    {
        private readonly FrozenDictionary<byte[], List<VarDef>>.AlternateLookup<ReadOnlySpan<byte>> IDToVariable;
        private readonly FrozenDictionary<ulong, List<VarDef>> OptimizedShortIDToVariable;
        public IEnumerable<List<VarDef>> Values => IDToVariable.Dictionary.Values;

        public IDToVarDef(FrozenDictionary<byte[], List<VarDef>> iDToVariable, FrozenDictionary<ulong, List<VarDef>> optimizedShortIDToVariable)
        {
            IDToVariable = iDToVariable.GetAlternateLookup<ReadOnlySpan<byte>>();
            OptimizedShortIDToVariable = optimizedShortIDToVariable;
        }

        public bool TryGetValue(ReadOnlySpan<byte> bytes, [NotNullWhen(true)] out List<VarDef>? variables)
        {
            if (bytes.Length <= sizeof(ulong))
            {
                ulong key = IDCollection.SpanToULong(bytes);
                return OptimizedShortIDToVariable.TryGetValue(key, out variables);
            }
            else
            {
                return IDToVariable.TryGetValue(bytes, out variables);
            }
        }
    }
}
