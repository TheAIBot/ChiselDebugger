using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;

namespace ChiselDebug.GraphFIR
{
    public abstract class BiArgMonoResPrimOp : FIRRTLPrimOP
    {
        public readonly string OpName;
        public readonly Input A;
        public readonly Input B;

        public BiArgMonoResPrimOp(string opName, Output aIn, Output bIn, IFIRType outType) : base(outType)
        {
            this.OpName = opName;
            this.A = new Input(this, aIn.Type);
            this.B = new Input(this, bIn.Type);

            aIn.ConnectToInput(A);
            bIn.ConnectToInput(B);
        }

        public override ScalarIO[] GetInputs()
        {
            return new ScalarIO[] { A, B };
        }

        public override FIRIO[] GetIO()
        {
            return new FIRIO[]
            {
                A,
                B,
                Result
            };
        }

        public override void InferType()
        {
            if (Result.Type is not UnknownType)
            {
                return;
            }

            A.InferType();
            B.InferType();

            Result.SetType(BiArgInferType());
        }
        public abstract IFIRType BiArgInferType();
    }

    public class FIRAdd : BiArgMonoResPrimOp
    {
        public FIRAdd(Output aIn, Output bIn, IFIRType outType) : base("+", aIn, bIn, outType) { }

        public override IFIRType BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(Math.Max(a.Width, b.Width) + 1),
            (UIntType a, SIntType b) => new SIntType(Math.Max(a.Width, b.Width) + 1),
            (SIntType a, UIntType b) => new SIntType(Math.Max(a.Width, b.Width) + 1),
            (SIntType a, SIntType b) => new SIntType(Math.Max(a.Width, b.Width) + 1),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRSub : BiArgMonoResPrimOp
    {
        public FIRSub(Output aIn, Output bIn, IFIRType outType) : base("-", aIn, bIn, outType) { }

        public override IFIRType BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new SIntType(Math.Max(a.Width, b.Width) + 1),
            (UIntType a, SIntType b) => new SIntType(Math.Max(a.Width, b.Width) + 1),
            (SIntType a, UIntType b) => new SIntType(Math.Max(a.Width, b.Width) + 1),
            (SIntType a, SIntType b) => new SIntType(Math.Max(a.Width, b.Width) + 1),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRMul : BiArgMonoResPrimOp
    {
        public FIRMul(Output aIn, Output bIn, IFIRType outType) : base("*", aIn, bIn, outType) { }

        public override IFIRType BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(a.Width + b.Width),
            (UIntType a, SIntType b) => new SIntType(a.Width + b.Width),
            (SIntType a, UIntType b) => new SIntType(a.Width + b.Width),
            (SIntType a, SIntType b) => new SIntType(a.Width + b.Width),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRDiv : BiArgMonoResPrimOp
    {
        public FIRDiv(Output aIn, Output bIn, IFIRType outType) : base("/", aIn, bIn, outType) { }

        public override IFIRType BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(a.Width),
            (UIntType a, SIntType b) => new SIntType(a.Width + 1),
            (SIntType a, UIntType b) => new SIntType(a.Width),
            (SIntType a, SIntType b) => new SIntType(a.Width + 1),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRRem : BiArgMonoResPrimOp
    {
        public FIRRem(Output aIn, Output bIn, IFIRType outType) : base("%", aIn, bIn, outType) { }

        public override IFIRType BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(Math.Min(a.Width, b.Width)),
            (UIntType a, SIntType b) => new UIntType(Math.Min(a.Width, b.Width)),
            (SIntType a, UIntType b) => new SIntType(Math.Min(a.Width, b.Width) + 1),
            (SIntType a, SIntType b) => new SIntType(Math.Min(a.Width, b.Width)),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRDshl : BiArgMonoResPrimOp
    {
        public FIRDshl(Output aIn, Output bIn, IFIRType outType) : base("<<", aIn, bIn, outType) { }

        public override IFIRType BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(a.Width + (1 << b.Width)),
            (SIntType a, UIntType b) => new SIntType(a.Width + (1 << b.Width)),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRDshr : BiArgMonoResPrimOp
    {
        public FIRDshr(Output aIn, Output bIn, IFIRType outType) : base(">>", aIn, bIn, outType) { }

        public override IFIRType BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(a.Width),
            (SIntType a, UIntType b) => new SIntType(a.Width),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRCat : BiArgMonoResPrimOp
    {
        public FIRCat(Output aIn, Output bIn, IFIRType outType) : base("cat", aIn, bIn, outType) { }

        public override IFIRType BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(a.Width + b.Width),
            (UIntType a, SIntType b) => new UIntType(a.Width + b.Width),
            (SIntType a, UIntType b) => new UIntType(a.Width + b.Width),
            (SIntType a, SIntType b) => new UIntType(a.Width + b.Width),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public abstract class FIRCompOp : BiArgMonoResPrimOp
    {
        public FIRCompOp(string opName, Output aIn, Output bIn, IFIRType outType) : base(opName, aIn, bIn, outType) { }

        public override IFIRType BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(1),
            (UIntType a, SIntType b) => new UIntType(1),
            (SIntType a, UIntType b) => new UIntType(1),
            (SIntType a, SIntType b) => new UIntType(1),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIREq : FIRCompOp
    {
        public FIREq(Output aIn, Output bIn, IFIRType outType) : base("=", aIn, bIn, outType){ }
    }
    
    public class FIRNeq : FIRCompOp
    {
        public FIRNeq(Output aIn, Output bIn, IFIRType outType) : base("≠", aIn, bIn, outType) { }
    }
    
    public class FIRGeq : FIRCompOp
    {
        public FIRGeq(Output aIn, Output bIn, IFIRType outType) : base("≥", aIn, bIn, outType) { }
    }
    
    public class FIRLeq : FIRCompOp
    {
        public FIRLeq(Output aIn, Output bIn, IFIRType outType) : base("≤", aIn, bIn, outType) { }
    }
    
    public class FIRGt : FIRCompOp
    {
        public FIRGt(Output aIn, Output bIn, IFIRType outType) : base(">", aIn, bIn, outType) { }
    }
    
    public class FIRLt : FIRCompOp
    {
        public FIRLt(Output aIn, Output bIn, IFIRType outType) : base("<", aIn, bIn, outType) { }
    }

    public abstract class FIRBitwise : BiArgMonoResPrimOp
    {
        public FIRBitwise(string opName, Output aIn, Output bIn, IFIRType outType) : base(opName, aIn, bIn, outType) { }

        public override IFIRType BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(Math.Max(a.Width, b.Width)),
            (UIntType a, SIntType b) => new UIntType(Math.Max(a.Width, b.Width)),
            (SIntType a, UIntType b) => new UIntType(Math.Max(a.Width, b.Width)),
            (SIntType a, SIntType b) => new UIntType(Math.Max(a.Width, b.Width)),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRAnd : FIRBitwise
    {
        public FIRAnd(Output aIn, Output bIn, IFIRType outType) : base("&", aIn, bIn, outType) { }
    }

    public class FIROr : FIRBitwise
    {
        public FIROr(Output aIn, Output bIn, IFIRType outType) : base("|", aIn, bIn, outType) { }
    }

    public class FIRXor : FIRBitwise
    {
        public FIRXor(Output aIn, Output bIn, IFIRType outType) : base("^", aIn, bIn, outType) { }
    }
}
