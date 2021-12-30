using System;
using System.Collections.Generic;

namespace VCDReader.Parsing
{
    internal class IDToVarDef
    {
        private Dictionary<SpanString, List<VarDef>> IDToVariable = new Dictionary<SpanString, List<VarDef>>();
        private Dictionary<ulong, List<VarDef>> OptiIDToVariable = new Dictionary<ulong, List<VarDef>>();
        private char[] MaxLengthID = Array.Empty<char>();
        public IEnumerable<List<VarDef>> Values => IDToVariable.Values;

        public void AddVariable(VarDef variable)
        {
            List<VarDef> varDefs;
            SpanString spanStr = new SpanString(variable.ID);
            if (IDToVariable.TryGetValue(spanStr, out var currDefs))
            {
                varDefs = currDefs;
            }
            else
            {
                varDefs = new List<VarDef>();
                IDToVariable.Add(spanStr, varDefs);
            }
            varDefs.Add(variable);
        }

        public void OptimizeVarDefAccess()
        {
            int maxLength = 0;
            foreach (var keyVal in IDToVariable)
            {
                maxLength = Math.Max(maxLength, keyVal.Key.Span.Length);
                if (keyVal.Key.Span.Length <= sizeof(ulong))
                {
                    ulong key = SpanToULong(keyVal.Key.Span);
                    OptiIDToVariable.Add(key, keyVal.Value);
                }
            }

            MaxLengthID = new char[maxLength];
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

        private ulong SpanToULong(ReadOnlySpan<char> chars)
        {
            ulong key = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                key |= (ulong)chars[i] << (i * 8);
            }

            return key;
        }

        public bool TryGetValue(ReadOnlyMemory<byte> bytes, out List<VarDef>? variables)
        {
            if (bytes.Length <= sizeof(ulong))
            {
                ulong key = SpanToULong(bytes.Span);
                return OptiIDToVariable.TryGetValue(key, out variables);
            }
            else
            {
                bytes.Span.CopyToCharArray(MaxLengthID);
                ReadOnlyMemory<char> charsMem = new ReadOnlyMemory<char>(MaxLengthID, 0, bytes.Span.Length);
                return IDToVariable.TryGetValue(new SpanString(charsMem), out variables);
            }
        }
    }
}
