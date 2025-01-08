using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Text;

namespace VCDReader.Parsing
{
    internal sealed class IDCollection
    {
        private readonly Dictionary<byte[], List<VarDef>> IDToVariable = new(BytesComparer.Default);

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

        public IDToVarDef GetIdToVarDef()
        {
            return new IDToVarDef(IDToVariable.ToFrozenDictionary(BytesComparer.Default), OptimizeVarDefAccess());
        }

        private FrozenDictionary<ulong, List<VarDef>> OptimizeVarDefAccess()
        {
            Dictionary<ulong, List<VarDef>> optimizedAccess = [];
            foreach (var keyVal in IDToVariable)
            {
                if (keyVal.Key.Length <= sizeof(ulong))
                {
                    ulong key = SpanToULong(keyVal.Key);
                    optimizedAccess.Add(key, keyVal.Value);
                }
            }

            return optimizedAccess.ToFrozenDictionary();
        }

        internal static ulong SpanToULong(ReadOnlySpan<byte> chars)
        {
            ulong key = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                key |= (ulong)chars[i] << (i * 8);
            }

            return key;
        }
    }
}
