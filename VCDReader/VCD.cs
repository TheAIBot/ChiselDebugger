using System;
using System.Collections.Generic;
using System.Linq;
using VCDReader.Parsing;

namespace VCDReader
{
    public class VCD : IDisposable
    {
        public readonly List<IDeclCmd> Declarations;
        private readonly Dictionary<string, List<VarDef>> IDToVariable;

        public readonly Date? Date;
        public readonly TimeScale? Time;
        public readonly Version? Version;

        private VCDLexer Lexer;

        public IEnumerable<List<VarDef>> Variables => IDToVariable.Values;

        internal VCD(List<IDeclCmd> declarations, Dictionary<string, List<VarDef>> idToVariable, VCDLexer lexer)
        {
            this.Declarations = declarations;
            this.IDToVariable = idToVariable;
            this.Lexer = lexer;

            this.Date = Declarations.FirstOrDefault(x => x is Date) as Date;
            this.Time = Declarations.FirstOrDefault(x => x is TimeScale) as TimeScale;
            this.Version = Declarations.FirstOrDefault(x => x is Version) as Version;
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
            Lexer.Dispose();
        }
    }
}
