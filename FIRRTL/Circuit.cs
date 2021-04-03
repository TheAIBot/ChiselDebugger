using System.Collections.Generic;
using System.Numerics;
using System.Text;

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

    public abstract record Literal(BigInteger Value, int Width) : Expression
    {
        public abstract IFIRType GetFIRType();
    }
    public record UIntLiteral(BigInteger Value, int Width) : Literal(Value, Width)
    {
        public override IFIRType GetFIRType()
        {
            return new UIntType(Width);
        }
    }
    public record SIntLiteral(BigInteger Value, int Width) : Literal(Value, Width)
    {
        public override IFIRType GetFIRType()
        {
            return new SIntType(Width);
        }
    }

    public record Statement() : FirrtlNode;
    public record EmptyStmt() : Statement;
    public record Block(List<Statement> Statements) : Statement;
    public record Conditionally(IInfo Info,  Expression Pred, Statement WhenTrue, Statement Alt) : Statement
    {
        public bool HasIf()
        {
            return WhenTrue is not EmptyStmt;
        }
        public bool HasElse()
        {
            return Alt is not EmptyStmt;
        }
    }

    public record Stop(IInfo Info, int Ret, Expression Clk, Expression Enabled) : Statement;
    public record Attach(IInfo Info, List<Expression> Exprs) : Statement;
    public record Print(IInfo Info, StringLit MsgFormat, List<Expression> Args, Expression Clk, Expression Enabled) : Statement;
    public record Verification(Formal Op, IInfo Info, Expression Clk, Expression Pred, Expression Enabled, StringLit MsgFormat) : Statement;
    public record Connect(IInfo Info, Expression Loc, Expression Expr) : Statement;
    public record PartialConnect(IInfo Info, Expression Loc, Expression Expr) : Statement;
    public record IsInvalid(IInfo Info, Expression Expr) : Statement;
    public record CDefMemory(IInfo Info, string Name, IFIRType Type, ulong Size, bool Sequence, ReadUnderWrite Ruw) : Statement;
    public record CDefMPort(IInfo Info, string Name, IFIRType Type, string Mem, List<Expression> Exps, MPortDir Direction) : Statement;

    public interface IsDeclaration { }
    public record DefWire(IInfo Info, string Name, IFIRType Type) : Statement, IsDeclaration;
    public record DefRegister(IInfo Info, string Name, IFIRType Type, Expression Clock, Expression Reset, Expression Init) : Statement, IsDeclaration
    {
        public bool HasResetAndInit()
        {
            bool hasDefaultReset = Reset is UIntLiteral res && res.Value.IsZero;
            bool hasDefaultInit = Init is Reference reff && reff.Name == Name;

            return !(hasDefaultReset && hasDefaultInit);
        }
    }
    public record DefInstance(IInfo Info, string Name, string Module, IFIRType Type) : Statement, IsDeclaration;
    public record DefNode(IInfo Info, string Name, Expression Value) : Statement, IsDeclaration;
    public record DefMemory(
        IInfo Info,
        string Name,
        IFIRType Type,
        ulong Depth,
        int WriteLatency,
        int ReadLatency,
        List<string> Readers,
        List<string> Writers,
        List<string> ReadWriters,
        ReadUnderWrite Ruw) : Statement, IsDeclaration;



    public record DefModule(string Name) : FirrtlNode, IsDeclaration;
    public record Port(IInfo Info, string Name, Dir Direction, IFIRType Type) : FirrtlNode, IsDeclaration;
    public record Module(IInfo Info, string Name, List<Port> Ports, Statement Body) : DefModule(Name);
    public record ExtModule(IInfo Info, string Name, List<Port> Ports, string DefName, List<Param> Params) : DefModule(Name);
    public record Circuit(IInfo Info, List<DefModule> Modules, string Main) : FirrtlNode;
}
