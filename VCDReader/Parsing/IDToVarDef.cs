using System;
using System.Collections.Generic;
using System.Linq;

namespace VCDReader.Parsing
{
    internal class IDToVarDef
    {
        private Dictionary<SpanString, List<VarDef>> IDToVariable = new Dictionary<SpanString, List<VarDef>>();
        private Dictionary<ulong, List<VarDef>> OptiIDToVariable = new Dictionary<ulong, List<VarDef>>();
        private bool IsOptimized = false;
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
            if (IDToVariable.Keys.All(x => x.Span.Length <= sizeof(ulong)))
            {
                foreach (var keyVal in IDToVariable)
                {
                    OptiIDToVariable.Add(SpanToULong(keyVal.Key.Span), keyVal.Value);
                }

                IsOptimized = true;
            }
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

        public bool TryGetValue(ReadOnlyMemory<char> chars, out List<VarDef>? variables)
        {
            if (IsOptimized)
            {
                ulong key = SpanToULong(chars.Span);
                return OptiIDToVariable.TryGetValue(key, out variables);
            }
            else
            {
                return IDToVariable.TryGetValue(new SpanString(chars), out variables);
            }
        }
    }
}
