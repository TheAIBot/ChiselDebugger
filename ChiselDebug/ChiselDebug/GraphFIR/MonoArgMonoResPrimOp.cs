using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;

namespace ChiselDebug.GraphFIR
{
    public abstract class MonoArgMonoResPrimOp : FIRRTLPrimOP
    {
        public readonly string OpName;
        public readonly Input A;

        public MonoArgMonoResPrimOp(string opName, Output aIn, IFIRType outType) : base(outType)
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
        public FIRAsUInt(Output aIn, IFIRType outType) : base("asUInt", aIn, outType) { }

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
        public FIRAsSInt(Output aIn, IFIRType outType) : base("asSInt", aIn, outType) { }

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
        public FIRAsClock(Output aIn, IFIRType outType) : base("asClock", aIn, outType) { }

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
        public FIRCvt(Output aIn, IFIRType outType) : base("cvt", aIn, outType) { }

        public override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new SIntType(a.Width + 1),
            SIntType a => new SIntType(a.Width),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRNeg : MonoArgMonoResPrimOp
    {
        public FIRNeg(Output aIn, IFIRType outType) : base("-", aIn, outType) { }

        public override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new SIntType(a.Width + 1),
            SIntType a => new SIntType(a.Width + 1),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRNot : MonoArgMonoResPrimOp
    {
        public FIRNot(Output aIn, IFIRType outType) : base("~", aIn, outType) { }

        public override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(a.Width),
            SIntType a => new UIntType(a.Width),
            _ => throw new Exception("Failed to infer type.")
        };
    }
}
