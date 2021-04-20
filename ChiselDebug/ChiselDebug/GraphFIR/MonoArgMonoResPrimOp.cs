using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Diagnostics;
using System.Numerics;
using VCDReader;

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

        public override void Compute()
        {
            Connection aCon = A.GetEnabledCon();
            Connection resultCon = Result.Con;

            BinaryVarValue aVal = (BinaryVarValue)aCon.Value.GetValue();
            BinaryVarValue resultVal = (BinaryVarValue)resultCon.Value.GetValue();

            if (!aVal.IsValidBinary())
            {
                Array.Fill(resultVal.Bits, BitState.X);
            }

            MonoArgCompute(aVal, resultVal);
        }
        protected abstract void MonoArgCompute(BinaryVarValue a, BinaryVarValue result);

        public override void InferType()
        {
            if (Result.Type is GroundType ground && ground.IsTypeFullyKnown())
            {
                return;
            }

            A.InferType();

            Result.SetType(MonoArgInferType());
        }
        protected abstract IFIRType MonoArgInferType();
    }

    public class FIRAsUInt : MonoArgMonoResPrimOp
    {
        public FIRAsUInt(Output aIn, IFIRType outType, FirrtlNode defNode) : base("asUInt", aIn, outType, defNode) { }

        protected override void MonoArgCompute(BinaryVarValue a, BinaryVarValue result)
        {
            Array.Copy(a.Bits, result.Bits, a.Bits.Length);
        }

        protected override IFIRType MonoArgInferType() => A.Type switch
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

        protected override void MonoArgCompute(BinaryVarValue a, BinaryVarValue result)
        {
            Array.Copy(a.Bits, result.Bits, a.Bits.Length);
        }

        protected override IFIRType MonoArgInferType() => A.Type switch
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

        protected override void MonoArgCompute(BinaryVarValue a, BinaryVarValue result)
        {
            result.Bits[0] = a.Bits[0];
        }

        protected override IFIRType MonoArgInferType() => A.Type switch
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

        protected override void MonoArgCompute(BinaryVarValue a, BinaryVarValue result)
        {
            if (A.Type is UIntType)
            {
                Array.Copy(a.Bits, result.Bits, a.Bits.Length);
                result.Bits[^1] = result.Bits[^2];
            }
            else if (A.Type is SIntType)
            {
                Array.Copy(a.Bits, result.Bits, a.Bits.Length);
            }
            else
            {
                throw new Exception($"Input to prim op cvt must be either uint or sint. Type: {A.Type}");
            }
        }

        protected override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new SIntType(a.Width + 1),
            SIntType a => new SIntType(a.Width),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRNeg : MonoArgMonoResPrimOp
    {
        public FIRNeg(Output aIn, IFIRType outType, FirrtlNode defNode) : base("-", aIn, outType, defNode) { }

        protected override void MonoArgCompute(BinaryVarValue a, BinaryVarValue result)
        {
            const int bitsInLong = 64;
            if (A.Type is UIntType)
            {
                if (a.Bits.Length <= bitsInLong && result.Bits.Length <= 64)
                {
                    long aVal = (long)a.AsULong();
                    result.SetBits(-aVal);
                }
                else
                {
                    BigInteger aVal = a.AsUnsignedBigInteger();
                    result.SetBitsZeroExtend(-aVal);
                }
            }
            else
            {
                Debug.Assert(A.Type is SIntType);
                if (a.Bits.Length <= bitsInLong && result.Bits.Length <= 64)
                {
                    long aVal = a.AsLong();
                    result.SetBits(-aVal);
                }
                else
                {
                    BigInteger aVal = a.AsSignedBigInteger();
                    result.SetBitsZeroExtend(-aVal);
                }
            }
        }

        protected override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new SIntType(a.Width + 1),
            SIntType a => new SIntType(a.Width + 1),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRNot : MonoArgMonoResPrimOp
    {
        public FIRNot(Output aIn, IFIRType outType, FirrtlNode defNode) : base("~", aIn, outType, defNode) { }

        protected override void MonoArgCompute(BinaryVarValue a, BinaryVarValue result)
        {
            for (int i = 0; i < a.Bits.Length; i++)
            {
                result.Bits[i] = (BitState)(((int)a.Bits[i] & 1) ^ 1);
            }
        }

        protected override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(a.Width),
            SIntType a => new UIntType(a.Width),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRAndr : MonoArgMonoResPrimOp
    {
        public FIRAndr(Output aIn, IFIRType outType, FirrtlNode defNode) : base("andr", aIn, outType, defNode) { }

        protected override void MonoArgCompute(BinaryVarValue a, BinaryVarValue result)
        {
            int value = (int)a.Bits[0];
            for (int i = 1; i < a.Bits.Length; i++)
            {
                value &= (int)a.Bits[i];
            }

            result.Bits[0] = (BitState)value;
        }

        protected override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(1),
            SIntType a => new UIntType(1),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIROrr : MonoArgMonoResPrimOp
    {
        public FIROrr(Output aIn, IFIRType outType, FirrtlNode defNode) : base("orr", aIn, outType, defNode) { }

        protected override void MonoArgCompute(BinaryVarValue a, BinaryVarValue result)
        {
            int value = (int)a.Bits[0];
            for (int i = 1; i < a.Bits.Length; i++)
            {
                value &= (int)a.Bits[i];
            }

            result.Bits[0] = (BitState)value;
        }

        protected override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(1),
            SIntType a => new UIntType(1),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRXorr : MonoArgMonoResPrimOp
    {
        public FIRXorr(Output aIn, IFIRType outType, FirrtlNode defNode) : base("xorr", aIn, outType, defNode) { }

        protected override void MonoArgCompute(BinaryVarValue a, BinaryVarValue result)
        {
            int value = (int)a.Bits[0];
            for (int i = 1; i < a.Bits.Length; i++)
            {
                value ^= (int)a.Bits[i];
            }

            result.Bits[0] = (BitState)value;
        }

        protected override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(1),
            SIntType a => new UIntType(1),
            _ => throw new Exception("Failed to infer type.")
        };
    }
}
