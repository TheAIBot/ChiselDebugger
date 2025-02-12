﻿using System;
using System.Collections.Generic;
using System.Linq;
using VCDReader.Parsing;

namespace VCDReader
{
    public sealed class VCD : IDisposable
    {
        public readonly List<IDeclCmd> Declarations;
        private readonly IDToVarDef IDToVariable;

        public readonly Date? Date;
        public readonly TimeScale? Time;
        public readonly Version? Version;

        private readonly VCDLexer Lexer;

        public IEnumerable<List<VarDef>> Variables => IDToVariable.Values;

        internal VCD(List<IDeclCmd> declarations, IDToVarDef idToVariable, VCDLexer lexer)
        {
            Declarations = declarations;
            IDToVariable = idToVariable;
            Lexer = lexer;

            Date = Declarations.FirstOrDefault(x => x is Date) as Date;
            Time = Declarations.FirstOrDefault(x => x is TimeScale) as TimeScale;
            Version = Declarations.FirstOrDefault(x => x is Version) as Version;
        }

        public IEnumerable<SimPass> GetSimulationCommands()
        {
            SimPass pass = new SimPass();
            BitAllocator bitAlloc = new BitAllocator();
            while (Lexer.IsWordsRemaining())
            {
                pass.Reset();
                Visitor.VisitSimCmd(Lexer, IDToVariable, pass, bitAlloc);
                if (pass.HasCmd())
                {
                    yield return pass;
                }
            }
        }

        public void Dispose()
        {
#pragma warning disable IDISP007 // Don't dispose injected
            Lexer.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected
        }
    }
}
