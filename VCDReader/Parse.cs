using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VCDReader.Parsing.Antlr;

namespace VCDReader
{
    public static class Parse
    {
        //public static VCD FromFile()
        //{

        //}

        public static VCD FromString(string vcdString)
        {
            ICharStream charStream = new AntlrInputStream(vcdString);
            VCDLexer lexer = new VCDLexer(charStream);
            //lexer.TokenFactory = new CommonTokenFactory(true);
            //ITokenStream tokenStream = new UnbufferedTokenStream(lexer);
            CommonTokenStream tokenStream = new CommonTokenStream(lexer);
            tokenStream.Fill();
            var fe = tokenStream.GetTokens().ToList();
            VCDParser parser = new VCDParser(tokenStream);
            return Visitor.VisitVcd(parser.vcd());
        }
    }

    public enum ScopeType
    {
        Begin,
        Fork,
        Function,
        Module,
        Task
    }

    public enum VarType
    {
        Event,
        Integer,
        Parameter,
        Real,
        RealTime,
        Reg,
        Supply0,
        Supply1,
        Time,
        Tri,
        TriAnd,
        TriOr,
        TriReg,
        Tri0,
        Tri1,
        Wand,
        Wire,
        Wor
    }

    public enum TimeUnit
    {
        S,
        Ms,
        Us,
        Ns,
        Ps,
        Fs
    }

    public enum BitState
    {
        Zero = 0,
        One = 1,
        X = 2,
        Z = 3
    }

    public static class Extensions
    {
        private static readonly BitState[] LeftExtend = new BitState[] { BitState.Zero, BitState.Zero, BitState.X, BitState.Z };
        private static readonly char[] BitAsChar = new char[] { '0', '1', 'X', 'Z' };

        public static BitState LeftExtendWith(this BitState bit)
        {
            return LeftExtend[(int)bit];
        }

        public static char ToChar(this BitState bit)
        {
            return BitAsChar[(int)bit];
        }

        public static bool IsBinary(this BitState bit)
        {
            return bit == BitState.Zero || bit == BitState.One;
        }
    }

    public class VCD
    {
        public readonly List<IHeaderCmd> Declarations;
        private readonly Dictionary<string, VarDef> IDToVariable;

        public readonly Date? Date;
        public readonly TimeScale? Time;
        public readonly Version? Version;

        private readonly Parsing.Antlr.VCDParser.SimCmdStreamContext SimStream;

        public IEnumerable<VarDef> Variables => IDToVariable.Values;

        internal VCD(List<IHeaderCmd> declarations, Dictionary<string, VarDef> idToVariable, Parsing.Antlr.VCDParser.SimCmdStreamContext simStream)
        {
            this.Declarations = declarations;
            this.IDToVariable = idToVariable;
            this.SimStream = simStream;

            this.Date = Declarations.FirstOrDefault(x => x is Date) as Date;
            this.Time = Declarations.FirstOrDefault(x => x is TimeScale) as TimeScale;
            this.Version = Declarations.FirstOrDefault(x => x is Version) as Version;
        }

        public IEnumerable<ISimCmd> GetSimulationCommands()
        {
            var currStream = SimStream;
            while (currStream != null)
            {
                yield return Visitor.VisitSimCmd(currStream.simCmd());
                currStream = currStream.simCmdStream();
            }
        }
    }

    public interface IHeaderCmd { }
    public record Comment(string Content) : IHeaderCmd, ISimCmd;
    public record Date(string Content) : IHeaderCmd;
    public record Scope(ScopeType Type, string Name) : IHeaderCmd;
    public record TimeScale(int Scale, TimeUnit Unit) : IHeaderCmd;
    public record UpScope() : IHeaderCmd;
    public record VarDef(VarType Type, int Size, string ID, string Reference, Scope[] Scopes) : IHeaderCmd;
    public record Version(string VersionText, string SystemTask) : IHeaderCmd;

    public interface ISimCmd { }
    public record  DumpAll(List<IValueChange> Values) : ISimCmd;
    public record DumpOn() : ISimCmd;
    public record DumpOff() : ISimCmd;
    public record DumpVars(List<IValueChange> InitialValues) : ISimCmd;
    public record SimTime(int Time) : ISimCmd;


    public interface IValueChange : ISimCmd { }
    public class BinaryChange : IValueChange
    {
        public readonly BitState[] Bits;
        public readonly VarDef Variable;

        public BinaryChange(BitState[] bits, VarDef variable)
        {
            this.Bits = new BitState[variable.Size];
            this.Variable = variable;

            Array.Fill(Bits, Bits[^1].LeftExtendWith());
            bits.CopyTo(bits, 0);
        }

        public string BitsToString()
        {
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < Bits.Length; i++)
            {
                sBuilder.Append(Bits[i].ToChar());
            }

            return sBuilder.ToString();
        }

        public bool IsValidBinary()
        {
            for (int i = 0; i < Bits.Length; i++)
            {
                if (!Bits[i].IsBinary())
                {
                    return false;
                }
            }

            return true;
        }
    }
    public record RealChange(double Value, VarDef Variable) : IValueChange;

    internal static class Visitor
    {
        public static VCD VisitVcd([NotNull] VCDParser.VcdContext context)
        {
            var declAndVarID = VisitDeclCmdStream(context.declCmdStream());
            return new VCD(declAndVarID.declarations, declAndVarID.idToVariable, context.simCmdStream());
        }

        internal static (List<IHeaderCmd> declarations, Dictionary<string, VarDef> idToVariable) VisitDeclCmdStream([NotNull] VCDParser.DeclCmdStreamContext context)
        {
            List<IHeaderCmd> declarations = new List<IHeaderCmd>();
            Dictionary<string, VarDef> idToVariable = new Dictionary<string, VarDef>();
            Stack<Scope> scopes = new Stack<Scope>();

            var currContext = context;
            while (currContext != null && currContext.declCmd() != null)
            {
                declarations.Add(VisitDeclCmd(currContext.declCmd(), idToVariable, scopes));
                currContext = currContext.declCmdStream();
            }

            if (scopes.Count != 0)
            {
                throw new Exception("Not all declaration scopes were closed.");
            }

            return (declarations, idToVariable);
        }

        public static IHeaderCmd VisitDeclCmd([NotNull] VCDParser.DeclCmdContext context, Dictionary<string, VarDef> idToVariable, Stack<Scope> scopes)
        {
            switch (context.GetChild(0).GetText())
            {
                case "$comment":
                    return new Comment(context.GetChild(1).GetText());
                case "$date":
                    return new Date(context.GetChild(1).GetText());
                case "$scope":
                    {
                        ScopeType type = VisitScopeType(context.scopeType());
                        string id = VisitId(context.AsciiString(0));

                        Scope scope = new Scope(type, id);
                        scopes.Push(scope);

                        return scope;
                    }
                case "$timescale":
                    {
                        int scale = VisitTimeNumber(context.timeNumber());
                        TimeUnit unit = VisitTimeUnit(context.timeUnit());

                        return new TimeScale(scale, unit);
                    }
                case "$upscope":
                    scopes.Pop();
                    return new UpScope();
                case "$var":
                    {
                        VarType type = VisitVarType(context.varType());
                        int size = VisitSize(context.AsciiString(0));
                        string id = VisitId(context.AsciiString(1));
                        string reference = VisitId(context.AsciiString(2));

                        return new VarDef(type, size, id, reference, scopes.ToArray());
                    }
                case "$version":
                    {
                        string versionTxt = context.GetChild(1).GetText();
                        string systemTaskString = context.GetChild(2).GetText();

                        return new Version(versionTxt, systemTaskString);
                    }
                default:
                    throw new Exception($"Invalid declaration command: {context.GetText()}");
            }
        }

        public static ISimCmd VisitSimCmd([NotNull] VCDParser.SimCmdContext context)
        {
            switch (context.GetChild(0).GetText())
            {
                case "$dumpall":
                    return new DumpAll(VisitValueChangeStream(context.valueChangeStream()));
                case "$dumpoff":
                    return new DumpOff();
                case "$dumpon":
                    return new DumpOn();
                case "$dumpvars":
                    return new DumpVars(VisitValueChangeStream(context.valueChangeStream()));
                case "$comment":
                    return new Comment(VisitAsciiString(context.AsciiString()));                    
            }

            if (context.GetChild(0).GetText().StartsWith('#'))
            {
                return VisitSimTime(context.AsciiString());
            }
            else if (context.valueChange() is var change && change != null)
            {
                return VisitValueChange(change);
            }
            else
            {
                throw new Exception($"Invalid simulation command: {context.GetText()}");
            }
        }

        public static List<IValueChange> VisitValueChangeStream([NotNull] VCDParser.ValueChangeStreamContext context)
        {
            List<IValueChange> changes = new List<IValueChange>();
            
            var currContext = context;
            while (currContext != null)
            {
                changes.Add(VisitValueChange(currContext.valueChange()));
                currContext = currContext.valueChangeStream();
            }

            return changes;
        }

        public static IValueChange VisitValueChange([NotNull] VCDParser.ValueChangeContext context)
        {
            throw new NotImplementedException();
        }

        private static string VisitAsciiString(ITerminalNode context)
        {
            return context?.GetText() ?? string.Empty;
        }

        public static int Visit([NotNull] IParseTree tree)
        {
            throw new NotImplementedException();
        }

        public static int VisitChildren([NotNull] IRuleNode node)
        {
            throw new NotImplementedException();
        }



        public static int VisitErrorNode([NotNull] IErrorNode node)
        {
            throw new NotImplementedException();
        }

        public static string VisitId([NotNull] ITerminalNode context)
        {
            return context.GetText();
        }

        public static ScopeType VisitScopeType([NotNull] VCDParser.ScopeTypeContext context)
        {
            return context.GetText() switch
            {
                "begin" => ScopeType.Begin,
                "fork" => ScopeType.Fork,
                "function" => ScopeType.Function,
                "module" => ScopeType.Module,
                "task" => ScopeType.Task,
                var error => throw new Exception($"Invalid scope type: {error}")
            };
        }

        public static SimTime VisitSimTime([NotNull] ITerminalNode context)
        {
            if (!context.GetText().StartsWith('#'))
            {
                throw new Exception($"Invalid simulation time: {context.GetText()}");
            }
            return new SimTime(int.Parse(context.GetText().Substring(1), CultureInfo.InvariantCulture));
        }

        public static int VisitSize([NotNull] ITerminalNode context)
        {
            return int.Parse(context.GetText(), CultureInfo.InvariantCulture);
        }

        public static string VisitSystemTask([NotNull] VCDParser.SystemTaskContext context)
        {
            if (!context.GetText().StartsWith('$'))
            {
                throw new Exception($"Invalid system task: {context.GetText()}");
            }
            return context.GetText().Substring(1);
        }

        public static int VisitTerminal([NotNull] ITerminalNode node)
        {
            throw new NotImplementedException();
        }

        public static int VisitTimeNumber([NotNull] VCDParser.TimeNumberContext context)
        {
            return context.GetText() switch
            {
                "1" => 1,
                "10" => 10,
                "100" => 100,
                var error => throw new Exception($"Invalid time scale: {error}")
            };
        }

        public static TimeUnit VisitTimeUnit([NotNull] VCDParser.TimeUnitContext context)
        {
            return context.GetText() switch
            {
                "s" => TimeUnit.S,
                "ms" => TimeUnit.Ms,
                "us" => TimeUnit.Us,
                "ns" => TimeUnit.Ns,
                "ps" => TimeUnit.Ps,
                "fs" => TimeUnit.Fs,
                var error => throw new Exception($"Invalid time unit: {error}")
            };
        }

        public static VarType VisitVarType([NotNull] VCDParser.VarTypeContext context)
        {
            return context.GetText() switch
            {
                "event" => VarType.Event,
                "integer" => VarType.Integer,
                "parameter" => VarType.Parameter,
                "real" => VarType.Real,
                "realtime" => VarType.RealTime,
                "reg" => VarType.Reg,
                "supply0" => VarType.Supply0,
                "supply1" => VarType.Supply1,
                "time" => VarType.Time,
                "tri" => VarType.Tri,
                "triand" => VarType.TriAnd,
                "trior" => VarType.TriOr,
                "trireg" => VarType.TriReg,
                "tri0" => VarType.Tri0,
                "tri1" => VarType.Tri1,
                "wand" => VarType.Wand,
                "wire" => VarType.Wire,
                "wor" => VarType.Wor,
                _ => throw new Exception($"Invalid variable type: {context.GetText()}")
            };
        }
    }
}
