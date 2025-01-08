using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace VCDReader.Parsing
{
    internal class IDToVarDef
    {
        private readonly Dictionary<byte[], List<VarDef>> IDToVariable = new Dictionary<byte[], List<VarDef>>(BytesComparer.Default);
        private readonly Dictionary<ulong, List<VarDef>> OptiIDToVariable = new Dictionary<ulong, List<VarDef>>();
        public IEnumerable<List<VarDef>> Values => IDToVariable.Values;

        public void AddVariable(VarDef variable)
        {
            List<VarDef> varDefs;
            byte[] variableUTF8Excoded = Encoding.UTF8.GetBytes(variable.ID);
            if (IDToVariable.TryGetValue(variableUTF8Excoded, out var currDefs))
            {
                varDefs = currDefs;
            }
            else
            {
                varDefs = new List<VarDef>();
                IDToVariable.Add(variableUTF8Excoded, varDefs);
            }
            varDefs.Add(variable);
        }

        public void OptimizeVarDefAccess()
        {
            int maxLength = 0;
            foreach (var keyVal in IDToVariable)
            {
                maxLength = Math.Max(maxLength, keyVal.Key.Length);
                if (keyVal.Key.Length <= sizeof(ulong))
                {
                    ulong key = SpanToULong(keyVal.Key);
                    OptiIDToVariable.Add(key, keyVal.Value);
                }
            }
        }

        private ulong SpanToULong(ReadOnlySpan<byte> chars)
        {
            ulong key = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                key |= (ulong)chars[i] << (i * 8);
            }

            return key;
        }

        public bool TryGetValue(ReadOnlyMemory<byte> bytes, [NotNullWhen(true)] out List<VarDef>? variables)
        {
            if (bytes.Length <= sizeof(ulong))
            {
                ulong key = SpanToULong(bytes.Span);
                return OptiIDToVariable.TryGetValue(key, out variables);
            }
            else
            {
                Dictionary<byte[], List<VarDef>>.AlternateLookup<ReadOnlySpan<byte>> spanLookup = IDToVariable.GetAlternateLookup<ReadOnlySpan<byte>>();
                return spanLookup.TryGetValue(bytes.Span, out variables);
            }
        }
    }
}
