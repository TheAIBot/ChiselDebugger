using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;

namespace ChiselDebug.GraphFIR
{
    public abstract class MonoArgMonoResPrimOp : FIRRTLPrimOP
    {
        public readonly string OpName;
        public readonly Input A;

        public MonoArgMonoResPrimOp(string opName, Output aIn, IFIRType outType, FirrtlNode defNode) : base(outType, defNode)
        {
            this.OpName = opName;
            this.A = new Input(this, aIn.Type);

            aIn.ConnectToInput(A);
        }

        public override ScalarIO[] GetInputs()
        {
            return new ScalarIO[] { A };
        }

        public override FIRIO[] GetIO()
        {
            return new FIRIO[]
            {
                A,
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

            Result.SetType(MonoArgInferType());
        }
        public abstract IFIRType MonoArgInferType();
    }

    public class FIRAsUInt : MonoArgMonoResPrimOp
    {
        public FIRAsUInt(Output aIn, IFIRType outType, FirrtlNode defNode) : base("asUInt", aIn, outType, defNode) { }

        public override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(a.Width),
            SIntType a => new UIntType(a.Width),
            ClockType a => new UIntType(1),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRAsSInt : MonoArgMonoResPrimOp
    {
        public FIRAsSInt(Output aIn, IFIRType outType, FirrtlNode defNode) : base("asSInt", aIn, outType, defNode) { }

        public override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new SIntType(a.Width),
            SIntType a => new SIntType(a.Width),
            ClockType a => new SIntType(1),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRAsClock : MonoArgMonoResPrimOp
    {
        public FIRAsClock(Output aIn, IFIRType outType, FirrtlNode defNode) : base("asClock", aIn, outType, defNode) { }

        public override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new ClockType(),
            SIntType a => new ClockType(),
            ClockType a => new ClockType(),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRCvt : MonoArgMonoResPrimOp
    {
        public FIRCvt(Output aIn, IFIRType outType, FirrtlNode defNode) : base("cvt", aIn, outType, defNode) { }

        public override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new SIntType(a.Width + 1),
            SIntType a => new SIntType(a.Width),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRNeg : MonoArgMonoResPrimOp
    {
        public FIRNeg(Output aIn, IFIRType outType, FirrtlNode defNode) : base("-", aIn, outType, defNode) { }

        public override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new SIntType(a.Width + 1),
            SIntType a => new SIntType(a.Width + 1),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRNot : MonoArgMonoResPrimOp
    {
        public FIRNot(Output aIn, IFIRType outType, FirrtlNode defNode) : base("~", aIn, outType, defNode) { }

        public override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(a.Width),
            SIntType a => new UIntType(a.Width),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRAndr : MonoArgMonoResPrimOp
    {
        public FIRAndr(Output aIn, IFIRType outType, FirrtlNode defNode) : base("andr", aIn, outType, defNode) { }

        public override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(1),
            SIntType a => new UIntType(1),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIROrr : MonoArgMonoResPrimOp
    {
        public FIROrr(Output aIn, IFIRType outType, FirrtlNode defNode) : base("orr", aIn, outType, defNode) { }

        public override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(1),
            SIntType a => new UIntType(1),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRXorr : MonoArgMonoResPrimOp
    {
        public FIRXorr(Output aIn, IFIRType outType, FirrtlNode defNode) : base("xorr", aIn, outType, defNode) { }

        public override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(1),
            SIntType a => new UIntType(1),
            _ => throw new Exception("Failed to infer type.")
        };
    }
}
