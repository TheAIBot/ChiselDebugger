using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;

namespace ChiselDebug.GraphFIR
{
    public abstract class ShiftPrimOp : FIRRTLPrimOP
    {
        public readonly string OpName;
        public readonly Input A;
        public readonly int ShiftBy;

        public ShiftPrimOp(string opName, Output aIn, int shiftBy, IFIRType outType) : base(outType)
        {
            this.OpName = opName;
            this.A = new Input(this, aIn.Type);
            this.ShiftBy = shiftBy;

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

            Result.SetType(ShiftInferType());
        }
        public abstract IFIRType ShiftInferType();
    }

    public class FIRShl : ShiftPrimOp
    {
        public FIRShl(Output aIn, int shiftBy, IFIRType outType) : base("<<", aIn, shiftBy, outType) { }

        public override IFIRType ShiftInferType() => A.Type switch
        {
            UIntType a => new UIntType(a.Width + ShiftBy),
            SIntType a => new SIntType(a.Width + ShiftBy),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRShr : ShiftPrimOp
    {
        public FIRShr(Output aIn, int shiftBy, IFIRType outType) : base(">>", aIn, shiftBy, outType) { }

        public override IFIRType ShiftInferType() => A.Type switch
        {
            UIntType a => new UIntType(a.Width - ShiftBy),
            SIntType a => new SIntType(a.Width - ShiftBy),
            _ => throw new Exception("Failed to infer type.")
        };
    }
}
