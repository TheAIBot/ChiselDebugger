using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
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

        public override Input[] GetInputs()
        {
            return new Input[] { A };
        }

        public override IEnumerable<FIRIO> GetIO()
        {
            yield return A;
            yield return Result;
        }

        public override void Compute()
        {
            ref readonly BinaryVarValue aVal = ref A.UpdateValueFromSourceFast();
            ref BinaryVarValue resultVal = ref Result.GetValue();

            Debug.Assert(aVal.IsValidBinary == aVal.Bits.IsAllBinary());
            if (!aVal.IsValidBinary)
            {
                resultVal.Bits.Fill(BitState.X);
                return;
            }

            resultVal.IsValidBinary = true;
            MonoArgCompute(in aVal, ref resultVal);
            Debug.Assert(resultVal.IsValidBinary == resultVal.Bits.IsAllBinary());
        }
        protected abstract void MonoArgCompute(in BinaryVarValue a, ref BinaryVarValue result);

        internal override void InferType()
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

        protected override void MonoArgCompute(in BinaryVarValue a, ref BinaryVarValue result)
        {
            a.Bits.CopyTo(result.Bits);
        }

        protected override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(a.Width),
            SIntType a => new UIntType(a.Width),
            ClockType a => new UIntType(1),
            _ => null
        };
    }

    public class FIRAsSInt : MonoArgMonoResPrimOp
    {
        public FIRAsSInt(Output aIn, IFIRType outType, FirrtlNode defNode) : base("asSInt", aIn, outType, defNode) { }

        protected override void MonoArgCompute(in BinaryVarValue a, ref BinaryVarValue result)
        {
            a.Bits.CopyTo(result.Bits);
        }

        protected override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new SIntType(a.Width),
            SIntType a => new SIntType(a.Width),
            ClockType a => new SIntType(1),
            _ => null
        };
    }

    public class FIRAsClock : MonoArgMonoResPrimOp
    {
        public FIRAsClock(Output aIn, IFIRType outType, FirrtlNode defNode) : base("asClock", aIn, outType, defNode) { }

        protected override void MonoArgCompute(in BinaryVarValue a, ref BinaryVarValue result)
        {
            result.Bits[0] = a.Bits[0];
        }

        protected override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new ClockType(),
            SIntType a => new ClockType(),
            ClockType a => new ClockType(),
            _ => null
        };
    }

    public class FIRCvt : MonoArgMonoResPrimOp
    {
        public FIRCvt(Output aIn, IFIRType outType, FirrtlNode defNode) : base("cvt", aIn, outType, defNode) { }

        protected override void MonoArgCompute(in BinaryVarValue a, ref BinaryVarValue result)
        {
            if (A.Type is UIntType)
            {
                a.Bits.CopyTo(result.Bits);
                result.Bits[^1] = BitState.Zero;
            }
            else if (A.Type is SIntType)
            {
                a.Bits.CopyTo(result.Bits);
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
            _ => null
        };
    }

    public class FIRNeg : MonoArgMonoResPrimOp
    {
        public FIRNeg(Output aIn, IFIRType outType, FirrtlNode defNode) : base("-", aIn, outType, defNode) { }

        protected override void MonoArgCompute(in BinaryVarValue a, ref BinaryVarValue result)
        {
            const int bitsInLong = 64;
            if (A.Type is UIntType)
            {
                //if (a.Bits.Length <= bitsInLong && result.Bits.Length <= 64)
                //{
                //    long aVal = (long)a.AsULong();
                //    result.SetBits(-aVal);
                //}
                //else
                //{
                    BigInteger aVal = a.AsUnsignedBigInteger();
                    result.SetBitsAndExtend(-aVal, Result.Type is SIntType);
                //}
            }
            else
            {
                //Debug.Assert(A.Type is SIntType);
                //if (a.Bits.Length <= bitsInLong && result.Bits.Length <= 64)
                //{
                //    long aVal = a.AsLong();
                //    result.SetBits(-aVal);
                //}
                //else
                //{
                    BigInteger aVal = a.AsSignedBigInteger();
                    result.SetBitsAndExtend(-aVal, Result.Type is SIntType);
                //}
            }
        }

        protected override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new SIntType(a.Width + 1),
            SIntType a => new SIntType(a.Width + 1),
            _ => null
        };
    }

    public class FIRNot : MonoArgMonoResPrimOp
    {
        public FIRNot(Output aIn, IFIRType outType, FirrtlNode defNode) : base("~", aIn, outType, defNode) { }

        protected override void MonoArgCompute(in BinaryVarValue a, ref BinaryVarValue result)
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
            _ => null
        };
    }

    public class FIRAndr : MonoArgMonoResPrimOp
    {
        public FIRAndr(Output aIn, IFIRType outType, FirrtlNode defNode) : base("andr", aIn, outType, defNode) { }

        protected override void MonoArgCompute(in BinaryVarValue a, ref BinaryVarValue result)
        {
            int value = 1;
            for (int i = 0; i < a.Bits.Length; i++)
            {
                value &= (int)a.Bits[i];
            }

            result.Bits[0] = (BitState)value;
        }

        protected override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(1),
            SIntType a => new UIntType(1),
            _ => null
        };
    }

    public class FIROrr : MonoArgMonoResPrimOp
    {
        public FIROrr(Output aIn, IFIRType outType, FirrtlNode defNode) : base("orr", aIn, outType, defNode) { }

        protected override void MonoArgCompute(in BinaryVarValue a, ref BinaryVarValue result)
        {
            int value = 0;
            for (int i = 0; i < a.Bits.Length; i++)
            {
                value |= (int)a.Bits[i];
            }

            result.Bits[0] = (BitState)value;
        }

        protected override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(1),
            SIntType a => new UIntType(1),
            _ => null
        };
    }

    public class FIRXorr : MonoArgMonoResPrimOp
    {
        public FIRXorr(Output aIn, IFIRType outType, FirrtlNode defNode) : base("xorr", aIn, outType, defNode) { }

        protected override void MonoArgCompute(in BinaryVarValue a, ref BinaryVarValue result)
        {
            int value = 0;
            for (int i = 0; i < a.Bits.Length; i++)
            {
                value ^= (int)a.Bits[i];
            }

            result.Bits[0] = (BitState)value;
        }

        protected override IFIRType MonoArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(1),
            SIntType a => new UIntType(1),
            _ => null
        };
    }
}
