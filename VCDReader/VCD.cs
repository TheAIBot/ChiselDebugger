using System.Collections.Generic;
using System.Linq;
using VCDReader.Parsing;

namespace VCDReader
{
    public class VCD
    {
        public readonly List<IDeclCmd> Declarations;
        private readonly Dictionary<string, VarDef> IDToVariable;

        public readonly Date? Date;
        public readonly TimeScale? Time;
        public readonly Version? Version;

        private VCDLexer Lexer;

        public IEnumerable<VarDef> Variables => IDToVariable.Values;

        internal VCD(List<IDeclCmd> declarations, Dictionary<string, VarDef> idToVariable, VCDLexer lexer)
        {
            this.Declarations = declarations;
            this.IDToVariable = idToVariable;
            this.Lexer = lexer;

            this.Date = Declarations.FirstOrDefault(x => x is Date) as Date;
            this.Time = Declarations.FirstOrDefault(x => x is TimeScale) as TimeScale;
            this.Version = Declarations.FirstOrDefault(x => x is Version) as Version;
        }

        public IEnumerable<ISimCmd> GetSimulationCommands()
        {
            while (Lexer.IsWordsRemaining())
            {
                yield return Visitor.VisitSimCmd(Lexer, IDToVariable);
            }
        }
    }
}
