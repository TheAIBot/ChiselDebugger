using System.Collections.Generic;
using System.Numerics;

namespace FIRRTL
{
    public interface IFirrtlNode { }
    public interface IInfo { };
    public sealed record NoInfo() : IInfo;
    public sealed record FileInfo(string Info) : IInfo;
    public sealed record MultiInfo(List<IInfo> Infos) : IInfo;

    public interface IInfoMode { }
    public sealed record IgnoreInfo() : IInfoMode;
    public sealed record UseInfo() : IInfoMode;
    public sealed record GenInfo(string Filename) : IInfoMode;
    public sealed record AppendInfo(string Filename) : IInfoMode;

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
    public sealed record UnknownType() : IFIRType;
    public record GroundType(int Width) : IFIRType
    {
        public const int UnknownWidth = -1;
        public bool IsWidthKnown => Width != UnknownWidth;

        public bool IsTypeFullyKnown()
        {
            return IsWidthKnown;
        }
    }
    public sealed record UIntType(int Width) : GroundType(Width);
    public sealed record SIntType(int Width) : GroundType(Width);
    public sealed record FixedType(int Width, int Point) : GroundType(Width);


    public sealed record ClockType() : GroundType(1);
    public sealed record AsyncResetType() : GroundType(1);
    public sealed record ResetType() : GroundType(1);
    public sealed record AnalogType(int Width) : GroundType(Width);

    public interface IAggregateType : IFIRType { }
    public sealed record Field(string Name, Orientation Flip, IFIRType Type);
    public sealed record BundleType(List<Field> Fields) : IAggregateType;
    public sealed record VectorType(IFIRType Type, int Size) : IAggregateType;

    public sealed record StringLit(string Str) : IFirrtlNode;

    public interface IParam : IFirrtlNode
    {
        string Name { get; }
    }
    public sealed record IntParam(string Name, BigInteger Value) : IParam;
    public sealed record StringParam(string Name, StringLit Value) : IParam;
    public sealed record DoubleParam(string Name, double Value) : IParam;
    public sealed record RawStringParam(string Name, string Value) : IParam;

    public record PrimOp(string Name) : IFirrtlNode
    {
        public override string ToString()
        {
            return Name;
        }
    }
    public sealed record Add() : PrimOp("add");
    public sealed record Sub() : PrimOp("sub");
    public sealed record Mul() : PrimOp("mul");
    public sealed record Div() : PrimOp("div");
    public sealed record Rem() : PrimOp("rem");
    public sealed record Lt() : PrimOp("lt");
    public sealed record Leq() : PrimOp("leq");
    public sealed record Gt() : PrimOp("gt");
    public sealed record Geq() : PrimOp("geq");
    public sealed record Eq() : PrimOp("eq");
    public sealed record Neq() : PrimOp("neq");
    public sealed record Pad() : PrimOp("pad");
    public sealed record Shl() : PrimOp("shl");
    public sealed record Shr() : PrimOp("shr");
    public sealed record Dshl() : PrimOp("dshl");
    public sealed record Dshr() : PrimOp("dshr");
    public sealed record Cvt() : PrimOp("cvt");
    public sealed record Neg() : PrimOp("neg");
    public sealed record Not() : PrimOp("not");
    public sealed record And() : PrimOp("and");
    public sealed record Or() : PrimOp("or");
    public sealed record Xor() : PrimOp("xor");
    public sealed record Andr() : PrimOp("andr");
    public sealed record Orr() : PrimOp("orr");
    public sealed record Xorr() : PrimOp("xorr");
    public sealed record Cat() : PrimOp("cat");
    public sealed record Bits() : PrimOp("bits");
    public sealed record Head() : PrimOp("head");
    public sealed record Tail() : PrimOp("tail");
    public sealed record IncP() : PrimOp("incp");
    public sealed record DecP() : PrimOp("decp");
    public sealed record SetP() : PrimOp("setp");
    public sealed record AsUInt() : PrimOp("asUInt");
    public sealed record AsSInt() : PrimOp("asSInt");
    public sealed record AsClock() : PrimOp("asClock");
    public sealed record AsAsyncReset() : PrimOp("asAsyncReset");
    public sealed record AsFixedPoint() : PrimOp("asFixedPoint");
    public sealed record AsInterval() : PrimOp("asInterval");
    public sealed record Squeeze() : PrimOp("squz");
    public sealed record Wrap() : PrimOp("wrap");
    public sealed record Clip() : PrimOp("clip");

    public interface IExpression : IFirrtlNode { }
    public sealed record DoPrim(PrimOp Op, List<IExpression> Args, List<BigInteger> Consts, IFIRType Type) : IExpression;
    public sealed record ValidIf(IExpression Cond, IExpression Value, IFIRType Type) : IExpression;
    public sealed record Mux(IExpression Cond, IExpression TrueValue, IExpression FalseValue, IFIRType Type) : IExpression;

    public interface IRefLikeExpression { }
    public sealed record Reference(string Name, IFIRType Type, KindType Kind, FlowType Flow) : IExpression, IRefLikeExpression;
    public sealed record SubField(IExpression Expr, string Name, IFIRType Type, FlowType Flow) : IExpression, IRefLikeExpression;
    public sealed record SubIndex(IExpression Expr, int Value, IFIRType Type, FlowType Flow) : IExpression, IRefLikeExpression;
    public sealed record SubAccess(IExpression Expr, IExpression Index, IFIRType Type, FlowType Flow) : IExpression, IRefLikeExpression;

    public interface ILiteral : IExpression
    {
        BigInteger Value { get; }
        int Width { get; }
        IFIRType GetFIRType();
    }
    public sealed record UIntLiteral(BigInteger Value, int Width) : ILiteral
    {
        public IFIRType GetFIRType()
        {
            return new UIntType(Width);
        }
    }
    public sealed record SIntLiteral(BigInteger Value, int Width) : ILiteral
    {
        public IFIRType GetFIRType()
        {
            return new SIntType(Width);
        }
    }

    public interface IStatement : IFirrtlNode { }
    public sealed record EmptyStmt() : IStatement;
    public sealed record Block(List<IStatement> Statements) : IStatement;
    public sealed record Conditionally(IInfo Info, IExpression Pred, IStatement WhenTrue, IStatement Alt) : IStatement
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

    public sealed record Stop(IInfo Info, int Ret, IExpression Clk, IExpression Enabled) : IStatement;
    public sealed record Attach(IInfo Info, List<IExpression> Exprs) : IStatement;
    public sealed record Print(IInfo Info, StringLit MsgFormat, List<IExpression> Args, IExpression Clk, IExpression Enabled) : IStatement;
    public sealed record Verification(Formal Op, IInfo Info, IExpression Clk, IExpression Pred, IExpression Enabled, StringLit MsgFormat) : IStatement;
    public sealed record Connect(IInfo Info, IExpression Loc, IExpression Expr) : IStatement;
    public sealed record PartialConnect(IInfo Info, IExpression Loc, IExpression Expr) : IStatement;
    public sealed record IsInvalid(IInfo Info, IExpression Expr) : IStatement;
    public sealed record CDefMemory(IInfo Info, string Name, IFIRType Type, ulong Size, bool Sequence, ReadUnderWrite Ruw) : IStatement;
    public sealed record CDefMPort(IInfo Info, string Name, IFIRType Type, string Mem, List<IExpression> Exps, MPortDir Direction) : IStatement;

    public interface IIsDeclaration { }
    public sealed record DefWire(IInfo Info, string Name, IFIRType Type) : IStatement, IIsDeclaration;
    public sealed record DefRegister(IInfo Info, string Name, IFIRType Type, IExpression Clock, IExpression Reset, IExpression Init) : IStatement, IIsDeclaration
    {
        public bool HasResetAndInit()
        {
            bool hasDefaultReset = Reset is UIntLiteral res && res.Value.IsZero;
            bool hasDefaultInit = Init is Reference reff && reff.Name == Name;

            return !(hasDefaultReset && hasDefaultInit);
        }
    }
    public sealed record DefInstance(IInfo Info, string Name, string Module, IFIRType Type) : IStatement, IIsDeclaration;
    public sealed record DefNode(IInfo Info, string Name, IExpression Value) : IStatement, IIsDeclaration;
    public sealed record DefMemory(
        IInfo Info,
        string Name,
        IFIRType Type,
        ulong Depth,
        int WriteLatency,
        int ReadLatency,
        List<string> Readers,
        List<string> Writers,
        List<string> ReadWriters,
        ReadUnderWrite Ruw) : IStatement, IIsDeclaration;



    public interface IDefModule : IFirrtlNode, IIsDeclaration
    {
        string Name { get; }
    }
    public sealed record Port(IInfo Info, string Name, Dir Direction, IFIRType Type) : IFirrtlNode, IIsDeclaration;
    public sealed record Module(IInfo Info, string Name, List<Port> Ports, IStatement Body) : IDefModule;
    public sealed record ExtModule(IInfo Info, string Name, List<Port> Ports, string DefName, List<IParam> Params) : IDefModule;
    public sealed record Circuit(IInfo Info, List<IDefModule> Modules, string Main) : IFirrtlNode;
}
