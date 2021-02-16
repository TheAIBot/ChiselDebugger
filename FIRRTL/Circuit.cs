using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace FIRRTL
{
    public interface FirrtlNode { }
    public interface IInfo { };
    public record NoInfo() : IInfo;
    public record FileInfo(string Info) : IInfo;
    public record MultiInfo(List<IInfo> Infos) : IInfo;

    public abstract record InfoMode;
    public record IgnoreInfo() : InfoMode;
    public record UseInfo() : InfoMode;
    public record GenInfo(string Filename) : InfoMode;
    public record AppendInfo(string Filename) : InfoMode;

    public enum Dir
    {
        Input,
        Output
    }

    public enum MPortDir
    {
        MInfer,
        MRead,
        MWrite,
        MReadWrite
    }

    public enum Orientation
    {
        Normal,
        Flipped
    }

    public enum ReadUnderWrite
    {
        Undefined,
        Old,
        New
    }

    public enum Formal
    {
        Assert,
        Assume,
        Cover
    }

    public enum KindType
    {
        Wire,
        Position,
        Reg,
        Instance,
        Port,
        Node,
        Mem,
        Exp,
        Unknown
    }

    public enum FlowType
    {
        Source,
        Sink,
        Duplex,
        Unknown
    }

    public interface IFIRType { }
    public record UnknownType() : IFIRType;
    public record GroundType(int Width) : IFIRType
    {
        public const int UnknownWidth = -1;
        public bool IsWidthKnown => Width != -1;
    }
    public record UIntType(int Width) : GroundType(Width);
    public record SIntType(int Width) : GroundType(Width);
    public record FixedType(int Width, int Point) : GroundType(Width);


    public record ClockType() : GroundType(1);
    public record AsyncResetType() : GroundType(1);
    public record ResetType() : GroundType(1);
    public record AnalogType(int Width) : GroundType(Width);

    public record AggregateType() : IFIRType;
    public record Field(string Name, Orientation Flip, IFIRType Type);
    public record BundleType(List<Field> Fields) : AggregateType;
    public record VectorType(IFIRType Type, int Size) : AggregateType;

    public record StringLit(string Str) : FirrtlNode;

    public record Param(string Name) : FirrtlNode;
    public record IntParam(string Name, BigInteger Value) : Param(Name);
    public record StringParam(string Name, StringLit Value) : Param(Name);
    public record DoubleParam(string Name, double Value) : Param(Name);
    public record RawStringParam(string Name, string Value) : Param(Name);

    public record PrimOp(string Name) : FirrtlNode
    {
        public override string ToString()
        {
            return Name;
        }
    }
    public record Add() : PrimOp("add");
    public record Sub() : PrimOp("sub");
    public record Mul() : PrimOp("mul");
    public record Div() : PrimOp("div");
    public record Rem() : PrimOp("rem");
    public record Lt() : PrimOp("lt");
    public record Leq() : PrimOp("leq");
    public record Gt() : PrimOp("gt");
    public record Geq() : PrimOp("geq");
    public record Eq() : PrimOp("eq");
    public record Neq() : PrimOp("neq");
    public record Pad() : PrimOp("pad");
    public record Shl() : PrimOp("shl");
    public record Shr() : PrimOp("shr");
    public record Dshl() : PrimOp("dshl");
    public record Dshr() : PrimOp("dshr");
    public record Cvt() : PrimOp("cvt");
    public record Neg() : PrimOp("neg");
    public record Not() : PrimOp("not");
    public record And() : PrimOp("and");
    public record Or() : PrimOp("or");
    public record Xor() : PrimOp("xor");
    public record Andr() : PrimOp("andr");
    public record Orr() : PrimOp("orr");
    public record Xorr() : PrimOp("xorr");
    public record Cat() : PrimOp("cat");
    public record Bits() : PrimOp("bits");
    public record Head() : PrimOp("head");
    public record Tail() : PrimOp("tail");
    public record IncP() : PrimOp("incp");
    public record DecP() : PrimOp("decp");
    public record SetP() : PrimOp("setp");
    public record AsUInt() : PrimOp("asUInt");
    public record AsSInt() : PrimOp("asSInt");
    public record AsClock() : PrimOp("asClock");
    public record AsAsyncReset() : PrimOp("asAsyncReset");
    public record AsFixedPoint() : PrimOp("asFixedPoint");
    public record AsInterval() : PrimOp("asInterval");
    public record Squeeze() : PrimOp("squz");
    public record Wrap() : PrimOp("wrap");
    public record Clip() : PrimOp("clip");

    public record Expression() : FirrtlNode;
    public record DoPrim(PrimOp Op, List<Expression> Args, List<BigInteger> Consts, IFIRType Type) : Expression;
    public record ValidIf(Expression Cond, Expression Value, IFIRType Type) : Expression;
    public record Mux(Expression Cond, Expression TrueValue, Expression FalseValue, IFIRType Type) : Expression;

    public interface RefLikeExpression { }
    public record Reference(string Name, IFIRType Type, KindType Kind, FlowType Flow) : Expression, RefLikeExpression;
    public record SubField(Expression Expr, string Name, IFIRType Type, FlowType Flow) : Expression, RefLikeExpression;
    public record SubIndex(Expression Expr, int Value, IFIRType Type, FlowType Flow) : Expression, RefLikeExpression;
    public record SubAccess(Expression Expr, Expression Index, IFIRType Type, FlowType Flow) : Expression, RefLikeExpression;

    public record Literal(BigInteger Value, int Width) : Expression;
    public record UIntLiteral(BigInteger Value, int Width) : Literal(Value, Width);
    public record SIntLiteral(BigInteger Value, int Width) : Literal(Value, Width);

    public record Statement() : FirrtlNode;
    public record EmptyStmt() : Statement;
    public record Block(List<Statement> Statements) : Statement;
    public record Conditionally(IInfo Info,  Expression Pred, Statement WhenTrue, Statement Alt) : Statement;

    public record Stop(IInfo Info, int Ret, Expression Clk, Expression Enabled) : Statement;
    public record Attach(IInfo Info, List<Expression> Exprs) : Statement;
    public record Print(IInfo Info, StringLit MsgFormat, List<Expression> Args, Expression Clk, Expression Enabled) : Statement;
    public record Verification(Formal Op, IInfo Info, Expression Clk, Expression Pred, Expression Enabled, StringLit MsgFormat) : Statement;
    public record Connect(IInfo Info, Expression Loc, Expression Expr) : Statement;
    public record PartialConnect(IInfo Info, Expression Loc, Expression Expr) : Statement;
    public record IsInvalid(IInfo Info, Expression Expr) : Statement;
    public record CDefMemory(IInfo Info, string Name, IFIRType Type, BigInteger Size, bool Sequence, ReadUnderWrite Ruw) : Statement;
    public record CDefMPort(IInfo Info, string Name, IFIRType Type, string Mem, List<Expression> Exps, MPortDir Direction) : Statement;

    public interface IsDeclaration { }
    public record DefWire(IInfo Info, string Name, IFIRType Type) : Statement, IsDeclaration;
    public record DefRegister(IInfo Info, string Name, IFIRType Type, Expression Clock, Expression Reset, Expression init) : Statement, IsDeclaration;
    public record DefInstance(IInfo Info, string Name, string Module, IFIRType Type) : Statement, IsDeclaration;
    public record DefNode(IInfo Info, string Name, Expression Value) : Statement, IsDeclaration;
    public record DefMemory(
        IInfo Info,
        string Name,
        IFIRType Type,
        BigInteger Depth,
        int WriteLatency,
        int ReadLatency,
        List<string> Readers,
        List<string> Writers,
        List<string> ReadWriters,
        ReadUnderWrite Ruw) : Statement, IsDeclaration;



    public record DefModule() : FirrtlNode, IsDeclaration;
    public record Port(IInfo Info, string Name, Dir Direction, IFIRType Type) : FirrtlNode, IsDeclaration;
    public record Module(IInfo Info, string Name, List<Port> Ports, Statement Body) : DefModule;
    public record ExtModule(IInfo Info, string Name, List<Port> Ports, string DefName, List<Param> Params) : DefModule;
    public record Circuit(IInfo Info, List<DefModule> Modules, string Main) : FirrtlNode;

    internal class Visitor
    {
        private const string HexPattern = "\"*h([+-]?[a-zA-Z0-9]+)\"*";
        private const string OctalPattern = "\"*o([+-]?[0-7]+)\"*";
        private const string BinaryPattern = "\"*b([+-]?[01]+)\"*";
        private const string DecPattern = @"([+\-]?[1-9]\d*)";
        private const string ZeroPattern = "0";
        private const string DecimalPattern = @"([+-]?[0-9]\d*\.[0-9]\d*)";
        private static readonly Dictionary<string, PrimOp> StringToPrimOp;

        private InfoMode InfoM;

        static Visitor()
        {
            PrimOp[] allPrimOps = new PrimOp[]
            {
                new Add(), new  Sub(), new  Mul(), 
                new  Div(), new  Rem(), new  Lt(), 
                new  Leq(), new  Gt(), new  Geq(), 
                new  Eq(), new  Neq(), new  Pad(), 
                new  AsUInt(), new  AsSInt(), 
                new  AsInterval(), new  AsClock(), 
                new AsAsyncReset(), new  Shl(), 
                new  Shr(), new  Dshl(), new  Dshr(), 
                new  Neg(), new  Cvt(), new  Not(), 
                new  And(), new  Or(), new  Xor(), 
                new  Andr(), new  Orr(), new  Xorr(), 
                new  Cat(), new  Bits(), new Head(), 
                new  Tail(), new  AsFixedPoint(), 
                new  IncP(), new  DecP(), new  SetP(), 
                new  Wrap(), new  Clip(), new  Squeeze()
            };

            StringToPrimOp = new Dictionary<string, PrimOp>();
            foreach (var primOp in allPrimOps)
            {
                StringToPrimOp.Add(primOp.Name, primOp);
            }
        }

        internal Visitor(InfoMode infoMode)
        {
            this.InfoM = infoMode;
        }

        private string RemoveSurroundingQuotes(string str)
        {
            return str.Length switch
            {
                0 => str,
                1 => string.Empty,
                2 => string.Empty,
                _ => str.Substring(1, str.Length - 2)
            };
        }

        private BigInteger StringToBigInteger(string str)
        {
            if (Regex.IsMatch(str, ZeroPattern, RegexOptions.Compiled))
            {
                return new BigInteger(0);
            }
            else if (Regex.IsMatch(str, HexPattern, RegexOptions.Compiled))
            {
                return BigInteger.Parse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            else if (Regex.IsMatch(str, OctalPattern, RegexOptions.Compiled))
            {
                throw new NotImplementedException("Parsing octal numbers is not supported yet.");
            }
            else if (Regex.IsMatch(str, BinaryPattern, RegexOptions.Compiled))
            {
                throw new NotImplementedException("Parsing binary numbers is not supported yet.");
            }
            else if (Regex.IsMatch(str, DecPattern, RegexOptions.Compiled))
            {
                return BigInteger.Parse(str, CultureInfo.InvariantCulture);
            }
            else
            {
                throw new Exception($"Invalid String for conversion to BigInteger: {str}");
            }
        }

        private int StringToInt(string str)
        {
            return (int)StringToBigInteger(str);
        }

        private bool LegalID(string id)
        {
            const string idPattern = "[A-Za-z0-9_$]+";
            return Regex.IsMatch(id, idPattern, RegexOptions.Compiled);
        }

        private IInfo VisitInfo(FIRRTLParser.InfoContext context, ParserRuleContext parentContext)
        {
            string GenInfo(string filename)
            {
                return $"{Path.GetFileName(filename)} {parentContext.Start.Line}:{parentContext.Start.Column}";
            }

            string useInfo = context == null ? string.Empty : new string(context.GetText().Skip(2).SkipLast(1).ToArray());

            if (InfoM is UseInfo)
            {
                if (useInfo.Length == 0)
                {
                    return new NoInfo();
                }
                else
                {
                    return new FileInfo(useInfo);
                }
            }
            else if (InfoM is AppendInfo aInfo)
            {
                if (useInfo.Length == 0)
                {
                    return new FileInfo(GenInfo(aInfo.Filename));
                }
                else
                {
                    return new MultiInfo(new List<IInfo>() { new FileInfo(useInfo), new FileInfo(GenInfo(aInfo.Filename)) });
                }
            }
            else if (InfoM is GenInfo gInfo)
            {
                return new FileInfo(GenInfo(gInfo.Filename));
            }
            else if (InfoM is IgnoreInfo)
            {
                return new NoInfo();
            }
            else
            {
                throw new Exception("Unhandled info type.");
            }
        }

        public Circuit VisitCircuit([NotNull] FIRRTLParser.CircuitContext context)
        {
            IInfo info = VisitInfo(context.info(), context);
            List<DefModule> modules = context.module().Select(VisitModule).ToList();
            string name = context.id().GetText();

            return new Circuit(info, modules, name);
        }

        private DefModule VisitModule([NotNull] FIRRTLParser.ModuleContext context)
        {
            IInfo info = VisitInfo(context.info(), context);
            if (context.GetChild(0).GetText() == "module")
            {
                string name = context.id().GetText();
                List<Port> ports = context.port().Select(VisitPort).ToList();
                Statement body = VisitBlock(context.moduleBlock());

                return new Module(info, name, ports, body);
            }
            else if (context.GetChild(0).GetText() == "extmodule")
            {
                string name = context.id().GetText();
                List<Port> ports = context.port().Select(VisitPort).ToList();
                string defName = context.defname().id().GetText() ?? name;
                List<Param> parameters = context.parameter().Select(VisitParameter).ToList();

                return new ExtModule(info, name, ports, defName, parameters);
            }
            else
            {
                throw new Exception("Invalid module type.");
            }
        }

        private Port VisitPort([NotNull] FIRRTLParser.PortContext context)
        {
            IInfo info = VisitInfo(context.info(), context);
            string name = context.id().GetText();
            Dir direction = VisitDir(context.dir());
            IFIRType type = VisitType(context.type());

            return new Port(info, name, direction, type);
        }

        private Param VisitParameter([NotNull] FIRRTLParser.ParameterContext context)
        {
            string name = context.id().GetText();

            return (context.intLit(), context.StringLit(), context.DoubleLit(), context.RawString()) switch
            {
                (var intLit, null, null, null) => new IntParam(name, StringToBigInteger(intLit.GetText())),
                (null, var str, null, null) => new StringParam(name, VisitStringLit(str)),
                (null, null, var dbl, null) => new DoubleParam(name, double.Parse(dbl.GetText())),
                (null, null, null, var raw) => new RawStringParam(name, RemoveSurroundingQuotes(raw.GetText()).Replace("\\", "'")),
                _ => throw new Exception($"Invalid parameter: {context.GetText()}")
            };
        }

        private Dir VisitDir([NotNull] FIRRTLParser.DirContext context)
        {
            return context.GetText() switch
            {
                "input" => Dir.Input,
                "output" => Dir.Output,
                var dir => throw new Exception($"Invalid direction: {dir}")
            };
        }

        private MPortDir VisitMdir([NotNull] FIRRTLParser.MdirContext context)
        {
            return context.GetText() switch
            {
                "infer" => MPortDir.MInfer,
                "read" => MPortDir.MRead,
                "write" => MPortDir.MWrite,
                "rdwr" => MPortDir.MReadWrite,
                var dir => throw new Exception($"Invalid memory port direction: {dir}")
            };
        }

        private IFIRType VisitType([NotNull] FIRRTLParser.TypeContext context)
        {
            int GetWidth(FIRRTLParser.IntLitContext lit)
            {
                return int.Parse(lit.GetText(), CultureInfo.InvariantCulture);
            }

            if (context.GetChild(0) is ITerminalNode term)
            {
                return term.GetText() switch
                {
                    "UInt" when context.ChildCount > 1 => new UIntType(GetWidth(context.intLit(0))),
                    "UInt" => new UIntType(GroundType.UnknownWidth),
                    "SInt" when context.ChildCount > 1 => new SIntType(GetWidth(context.intLit(0))),
                    "SInt" => new SIntType(GroundType.UnknownWidth),
                    "Fixed" => context.intLit().Length switch
                    {
                        0 => new FixedType(GroundType.UnknownWidth, GroundType.UnknownWidth),
                        1 => context.GetChild(2).GetText() switch
                        {
                            "<" => new FixedType(GroundType.UnknownWidth, GetWidth(context.intLit(0))),
                            _ => new FixedType(GetWidth(context.intLit(0)), GroundType.UnknownWidth)
                        },
                        2 => new FixedType(GetWidth(context.intLit(0)), GetWidth(context.intLit(1))),
                        _ => throw new Exception("Invalid Fixed type.")
                    },
                    "Interval" => throw new NotImplementedException("Intervals are currently not supported"),
                    "Clock" => new ClockType(),
                    "AsyncReset" => new AsyncResetType(),
                    "Reset" => new ResetType(),
                    "Analog" when context.ChildCount > 1 => new AnalogType(GetWidth(context.intLit(0))),
                    "Analog" => new AnalogType(GroundType.UnknownWidth),
                    "{" => new BundleType(context.field().Select(VisitField).ToList()),
                    _ => throw new Exception($"Uknown type: {term.GetText()}")
                };
            }
            else if (context.GetChild(0) is FIRRTLParser.TypeContext typeContext)
            {
                return new VectorType(VisitType(context.type()), GetWidth(context.intLit(0)));
            }
            else
            {
                throw new Exception($"Uknown type: {context.GetText()}");
            }
        }

        private (IFIRType Type, BigInteger Size) VisitCMemType([NotNull] FIRRTLParser.TypeContext context)
        {
            if (context.GetChild(0) is FIRRTLParser.TypeContext typeContext)
            {
                IFIRType type = VisitType(context.type());
                BigInteger size = StringToBigInteger(context.intLit(0).GetText());

                return (type, size);
            }
            else
            {
                throw new Exception($"Invalid memory type: {context.GetText()}");
            }
        }

        private Field VisitField([NotNull] FIRRTLParser.FieldContext context)
        {
            string name = context.fieldId().GetText();
            Orientation orien = context.GetChild(0).GetText() == "flip" ? Orientation.Flipped : Orientation.Normal;
            IFIRType type = VisitType(context.type());

            return new Field(name, orien, type);
        }

        private Statement VisitBlock(FIRRTLParser.ModuleBlockContext context)
        {
            if (context == null)
            {
                return new EmptyStmt();
            }

            List<Statement> statements = context.simple_stmt()
                .Select(x => x.stmt())
                .Where(x => x != null)
                .Select(VisitStmt).ToList();

            return new Block(statements);
        }

        private Statement VisitSuite([NotNull] FIRRTLParser.SuiteContext context)
        {
            List<Statement> statements = context.simple_stmt()
                .Select(x => x.stmt())
                .Where(x => x != null)
                .Select(VisitStmt).ToList();

            return new Block(statements);
        }

        private ReadUnderWrite VisitRuw(FIRRTLParser.RuwContext context)
        {
            if (context == null)
            {
                return ReadUnderWrite.Undefined;
            }

            return context.GetText() switch
            {
                "undefined" => ReadUnderWrite.Undefined,
                "old" => ReadUnderWrite.Old,
                "new" => ReadUnderWrite.New,
                _ => throw new Exception($"Invalid ruw: {context.GetText()}")
            };
        }

        private Statement VisitMem([NotNull] FIRRTLParser.StmtContext context)
        {
            List<string> readers = new List<string>();
            List<string> writers = new List<string>();
            List<string> readWriters = new List<string>();
            var fieldMap = new Dictionary<string, (IFIRType Type, int? Lit, ReadUnderWrite Ruv, bool Unique)>();
            string memName = context.id(0).GetText();
            IInfo info = VisitInfo(context.info(), context);

            foreach (var field in context.memField())
            {
                string fieldName = field.GetChild(0).GetText();
                switch (fieldName)
                {
                    case "reader":
                        readers.AddRange(field.id().Select(x => x.GetText()));
                        continue;
                    case "writer":
                        writers.AddRange(field.id().Select(x => x.GetText()));
                        continue;
                    case "readwriter":
                        readWriters.AddRange(field.id().Select(x => x.GetText()));
                        continue;
                    default:
                        break;
                }

                if (fieldMap.ContainsKey(fieldName))
                {
                    throw new Exception($"Redefinition of {fieldName} in FIRRTL line: {field.start.Line}");
                }

                fieldMap.Add(fieldName, fieldName switch
                {
                    "data-type" => (VisitType(field.type()), null, ReadUnderWrite.Undefined, true),
                    "read-under-write" => (null, null, VisitRuw(field.ruw()), true),
                    _ => (null, int.Parse(field.intLit().GetText(), CultureInfo.InvariantCulture), ReadUnderWrite.Undefined, true)
                });
            }

            string[] requiredFields = new string[]
            {
                "data-type",
                "depth",
                "read-latency",
                "write-latency"
            };

            foreach (var required in requiredFields)
            {
                if (!fieldMap.ContainsKey(required))
                {
                    throw new Exception($"Required mem field {required} not found.");
                }
            }

            ReadUnderWrite ruw = fieldMap.TryGetValue("read-under-write", out var fieldInfo) ? fieldInfo.Ruv : ReadUnderWrite.Undefined;

            return new DefMemory(
                info,
                memName,
                fieldMap["data-type"].Type,
                fieldMap["depth"].Lit.Value,
                fieldMap["write-latency"].Lit.Value,
                fieldMap["read-latency"].Lit.Value,
                readers,
                writers,
                readWriters,
                ruw);
        }

        private StringLit VisitStringLit(ITerminalNode node)
        {
            return new StringLit(RemoveSurroundingQuotes(node.GetText()));
        }

        private Statement VisitWhen([NotNull] FIRRTLParser.WhenContext context)
        {
            IInfo info = VisitInfo(context.info(0), context);

            Statement alt;
            if (context.when() != null)
            {
                alt = VisitWhen(context.when());
            }
            else if (context.suite().Length > 1)
            {
                alt = VisitSuite(context.suite(1));
            }
            else
            {
                alt = new EmptyStmt();
            }

            return new Conditionally(info, VisitExp(context.exp()), VisitSuite(context.suite(0)), alt);
        }

        private Statement VisitStmt([NotNull] FIRRTLParser.StmtContext context)
        {
            FIRRTLParser.ExpContext[] contextExprs = context.exp();
            IInfo info = VisitInfo(context.info(), context);

            if (context.GetChild(0) is FIRRTLParser.WhenContext when)
            {
                return VisitWhen(when);
            }
            else if (context.GetChild(0) is ITerminalNode term)
            {
                switch (term.GetText())
                {
                    case "wire":
                        return new DefWire(info, context.id(0).GetText(), VisitType(context.type()));
                    case "reg":
                        {
                            string name = context.id(0).GetText();
                            IFIRType type = VisitType(context.type());
                            Expression clock = VisitExp(contextExprs[0]);

                            var rb = context.reset_block();
                            if (rb != null)
                            {
                                var sr = rb.simple_reset().simple_reset0();
                                IInfo innerInfo = info is NoInfo ? VisitInfo(rb.info(), context) : info;
                                Expression reset = VisitExp(sr.exp(0));
                                Expression init = VisitExp(sr.exp(1));

                                return new DefRegister(innerInfo, name, type, clock, reset, init);
                            }
                            else
                            {
                                Expression reset = new UIntLiteral(new BigInteger(0), 1);
                                Expression init = new Reference(name, type, KindType.Unknown, FlowType.Unknown); ;
                                return new DefRegister(info, name, type, clock, reset, init);
                            }
                        }
                    case "mem":
                        return VisitMem(context);
                    case "cmem":
                        {
                            var cMemType = VisitCMemType(context.type());
                            return new CDefMemory(info, context.id(0).GetText(), cMemType.Type, cMemType.Size, false, ReadUnderWrite.Undefined);
                        }
                    case "smem":
                        {
                            var cMemType = VisitCMemType(context.type());
                            return new CDefMemory(info, context.id(0).GetText(), cMemType.Type, cMemType.Size, true, VisitRuw(context.ruw()));
                        }
                    case "inst":
                        return new DefInstance(info, context.id(0).GetText(), context.id(1).GetText(), new UnknownType());
                    case "node":
                        return new DefNode(info, context.id(0).GetText(), VisitExp(contextExprs[0]));
                    case "stop(":
                        return new Stop(info, StringToInt(context.intLit().GetText()), VisitExp(contextExprs[0]), VisitExp(contextExprs[1]));
                    case "attach":
                        return new Attach(info, contextExprs.Select(VisitExp).ToList());
                    case "printf(":
                        {
                            StringLit msg = VisitStringLit(context.StringLit());
                            var exprs = contextExprs.Select(VisitExp).ToList();
                            return new Print(info, msg, exprs.Skip(2).ToList(), exprs[0], exprs[1]);
                        }
                    case "assert":
                        {
                            StringLit msg = VisitStringLit(context.StringLit());
                            var exprs = contextExprs.Select(VisitExp).ToList();
                            return new Verification(Formal.Assert, info, exprs[0], exprs[1], exprs[2], msg);
                        }
                    case "assume":
                        {
                            StringLit msg = VisitStringLit(context.StringLit());
                            var exprs = contextExprs.Select(VisitExp).ToList();
                            return new Verification(Formal.Assume, info, exprs[0], exprs[1], exprs[2], msg);
                        }
                    case "cover":
                        {
                            StringLit msg = VisitStringLit(context.StringLit());
                            var exprs = contextExprs.Select(VisitExp).ToList();
                            return new Verification(Formal.Cover, info, exprs[0], exprs[1], exprs[2], msg);
                        }
                    case "skip":
                        return new EmptyStmt();
                    default:
                        throw new Exception($"Invalid statement: {context.GetText()}");
                }
            }
            else
            {
                switch (context.GetChild(1).GetText())
                {
                    case "<=":
                        return new Connect(info, VisitExp(contextExprs[0]), VisitExp(contextExprs[1]));
                    case "<-":
                        return new PartialConnect(info, VisitExp(contextExprs[0]), VisitExp(contextExprs[1]));
                    case "is":
                        return new IsInvalid(info, VisitExp(contextExprs[0]));
                    case "mport":
                        return new CDefMPort(info, context.id(0).GetText(), new UnknownType(), context.id(1).GetText(), new List<Expression>() { VisitExp(contextExprs[0]), VisitExp(contextExprs[1]) }, VisitMdir(context.mdir()));
                    default:
                        throw new Exception($"Invalid statement: {context.GetText()}");
                }
            }
        }

        private Expression VisitExp([NotNull] FIRRTLParser.ExpContext context)
        {
            FIRRTLParser.ExpContext[] contextExprs = context.exp();

            if (context.GetChild(0) is FIRRTLParser.IdContext)
            {
                return new Reference(context.GetText(), new UnknownType(), KindType.Unknown, FlowType.Unknown);
            }
            else if (context.GetChild(0) is FIRRTLParser.ExpContext)
            {
                if (context.GetChild(1).GetText() == ".")
                {
                    Expression expr1 = VisitExp(contextExprs[0]);
                    if (context.fieldId() == null)
                    {
                        string[] ids = context.DoubleLit().GetText().Split('.');
                        if (ids.Length != 2 || !LegalID(ids[0]) || !LegalID(ids[1]))
                        {
                            throw new Exception($"Invalid expression: {context.GetText()}");
                        }

                        SubField inner = new SubField(expr1, ids[0], new UnknownType(), FlowType.Unknown);
                        return new SubField(inner, ids[1], new UnknownType(), FlowType.Unknown);
                    }
                    else
                    {
                        return new SubField(expr1, context.fieldId().GetText(), new UnknownType(), FlowType.Unknown);
                    }
                }
                else if (context.GetChild(1).GetText() == "[")
                {
                    if (context.exp(1) == null)
                    {
                        return new SubIndex(VisitExp(contextExprs[0]), StringToInt(context.intLit(0).GetText()), new UnknownType(), FlowType.Unknown);
                    }
                    else
                    {
                        return new SubAccess(VisitExp(contextExprs[0]), VisitExp(contextExprs[1]), new UnknownType(), FlowType.Unknown);
                    }
                }
                else
                {
                    throw new Exception($"Invalid expression: {context.GetText()}");
                }
            }
            else if (context.GetChild(0) is FIRRTLParser.PrimopContext)
            {
                return new DoPrim(VisitPrimop(context.primop()), contextExprs.Select(VisitExp).ToList(), context.intLit().Select(x => StringToBigInteger(x.GetText())).ToList(), new UnknownType());
            }
            else
            {
                string exprStr = context.GetChild(0).GetText();
                if (exprStr == "UInt" || exprStr == "SInt")
                {
                    int width;
                    BigInteger value;
                    if (context.ChildCount > 4)
                    {
                        width = StringToInt(context.intLit(0).GetText());
                        value = StringToBigInteger(context.intLit(1).GetText());
                    }
                    else
                    {
                        width = GroundType.UnknownWidth;
                        value = StringToBigInteger(context.intLit(0).GetText());
                    }

                    if (exprStr == "UInt")
                    {
                        return new UIntLiteral(value, width);
                    }
                    else
                    {
                        return new SIntLiteral(value, width);
                    }
                }
                else if (exprStr == "validif(")
                {
                    return new ValidIf(VisitExp(contextExprs[0]), VisitExp(contextExprs[1]), new UnknownType());
                }
                else if (exprStr == "mux(")
                {
                    return new Mux(VisitExp(contextExprs[0]), VisitExp(contextExprs[1]), VisitExp(contextExprs[2]), new UnknownType());
                }
                else
                {
                    throw new Exception($"Invalid expression: {context.GetText()}");
                }
            }
        }

        private PrimOp VisitPrimop([NotNull] FIRRTLParser.PrimopContext context)
        {
            return StringToPrimOp[context.GetText().TrimEnd('(')];
        }
    }
}
