using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using VCDReader;
#nullable enable

namespace ChiselDebug.GraphFIR.Components
{
    public abstract class MonoArgMonoResPrimOp : FIRRTLPrimOP
    {
        public readonly string OpName;
        public readonly Sink A;

        public MonoArgMonoResPrimOp(string opName, Source aIn, IFIRType outType, IFirrtlNode defNode) : base(outType, defNode)
        {
            OpName = opName;
            A = new Sink(this, aIn.Type);

            aIn.ConnectToInput(A);
        }

        public override Sink[] GetSinks()
        {
            return new Sink[] { A };
        }

        public override IEnumerable<FIRIO> GetIO()
        {
            yield return A;
            yield return Result;
        }

        public override void Compute()
        {
            ref BinaryVarValue aVal = ref A.UpdateValueFromSourceFast();
            ref BinaryVarValue resultVal = ref Result.GetValue();

            Debug.Assert(aVal.IsValidBinary == aVal.Bits.IsAllBinary());

            resultVal.IsValidBinary = true;
            MonoArgCompute(ref aVal, ref resultVal);

            Debug.Assert(resultVal.IsValidBinary == resultVal.Bits.IsAllBinary());
        }
        protected abstract void MonoArgCompute(ref BinaryVarValue a, ref BinaryVarValue result);

        internal override void InferType()
        {
            if (Result.Type is GroundType ground && ground.IsTypeFullyKnown())
            {
                return;
            }

            A.InferType();

            Result.SetType(MonoArgInferType());
        }
        protected abstract IFIRType? MonoArgInferType();
    }

    public sealed class FIRAsUInt : MonoArgMonoResPrimOp
    {
        public FIRAsUInt(Source aIn, IFIRType outType, IFirrtlNode defNode) : base("asUInt", aIn, outType, defNode) { }

        protected override void MonoArgCompute(ref BinaryVarValue a, ref BinaryVarValue result)
        {
            a.Bits.CopyTo(result.Bits);

            result.IsValidBinary = a.IsValidBinary;
        }

        protected override IFIRType? MonoArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(a.Width),
            SIntType a => new UIntType(a.Width),
            ClockType a => new UIntType(1),
            _ => null
        };
    }

    public sealed class FIRAsSInt : MonoArgMonoResPrimOp
    {
        public FIRAsSInt(Source aIn, IFIRType outType, IFirrtlNode defNode) : base("asSInt", aIn, outType, defNode) { }

        protected override void MonoArgCompute(ref BinaryVarValue a, ref BinaryVarValue result)
        {
            a.Bits.CopyTo(result.Bits);

            result.IsValidBinary = a.IsValidBinary;
        }

        protected override IFIRType? MonoArgInferType() => A.Type switch
        {
            UIntType a => new SIntType(a.Width),
            SIntType a => new SIntType(a.Width),
            ClockType a => new SIntType(1),
            _ => null
        };
    }

    public sealed class FIRAsClock : MonoArgMonoResPrimOp
    {
        public FIRAsClock(Source aIn, IFIRType outType, IFirrtlNode defNode) : base("asClock", aIn, outType, defNode) { }

        protected override void MonoArgCompute(ref BinaryVarValue a, ref BinaryVarValue result)
        {
            result.Bits[0] = a.Bits[0];

            result.IsValidBinary = result.Bits[0].IsBinary();
        }

        protected override IFIRType? MonoArgInferType() => A.Type switch
        {
            UIntType a => new ClockType(),
            SIntType a => new ClockType(),
            ClockType a => new ClockType(),
            _ => null
        };
    }

    public sealed class FIRCvt : MonoArgMonoResPrimOp
    {
        public FIRCvt(Source aIn, IFIRType outType, IFirrtlNode defNode) : base("cvt", aIn, outType, defNode) { }

        protected override void MonoArgCompute(ref BinaryVarValue a, ref BinaryVarValue result)
        {
            if (A.Value == null)
            {
                throw new InvalidOperationException($"{nameof(A)} was not initialized.");
            }

            if (!A.Value.IsSigned)
            {
                a.Bits.CopyTo(result.Bits);
                result.Bits[^1] = BitState.Zero;
            }
            else
            {
                a.Bits.CopyTo(result.Bits);
            }

            result.IsValidBinary = a.IsValidBinary;
        }

        protected override IFIRType? MonoArgInferType() => A.Type switch
        {
            UIntType a => new SIntType(a.Width + 1),
            SIntType a => new SIntType(a.Width),
            _ => null
        };
    }

    public sealed class FIRNeg : MonoArgMonoResPrimOp
    {
        public FIRNeg(Source aIn, IFIRType outType, IFirrtlNode defNode) : base("-", aIn, outType, defNode) { }

        protected override void MonoArgCompute(ref BinaryVarValue a, ref BinaryVarValue result)
        {
            if (!a.IsValidBinary)
            {
                result.SetAllUnknown();
                return;
            }

            if (A.Value == null)
            {
                throw new InvalidOperationException($"{nameof(A)} was not initialized.");
            }

            BigInteger aVal = a.AsBigInteger(A.Value.IsSigned);
            result.SetBitsAndExtend(-aVal);
        }

        protected override IFIRType? MonoArgInferType() => A.Type switch
        {
            UIntType a => new SIntType(a.Width + 1),
            SIntType a => new SIntType(a.Width + 1),
            _ => null
        };
    }

    public sealed class FIRNot : MonoArgMonoResPrimOp
    {
        public FIRNot(Source aIn, IFIRType outType, IFirrtlNode defNode) : base("~", aIn, outType, defNode) { }

        protected override void MonoArgCompute(ref BinaryVarValue a, ref BinaryVarValue result)
        {
            for (int i = 0; i < a.Bits.Length; i++)
            {
                result.Bits[i] = FIRBitwise.CompOpPropX(a.Bits[i], (BitState)((int)a.Bits[i] & 1 ^ 1));
            }

            result.IsValidBinary = a.IsValidBinary;
        }

        protected override IFIRType? MonoArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(a.Width),
            SIntType a => new UIntType(a.Width),
            _ => null
        };
    }

    public sealed class FIRAndr : MonoArgMonoResPrimOp
    {
        public FIRAndr(Source aIn, IFIRType outType, IFirrtlNode defNode) : base("andr", aIn, outType, defNode) { }

        protected override void MonoArgCompute(ref BinaryVarValue a, ref BinaryVarValue result)
        {
            if (!a.IsValidBinary)
            {
                result.SetAllUnknown();
                return;
            }

            int value = 1;
            for (int i = 0; i < a.Bits.Length; i++)
            {
                value &= (int)a.Bits[i];
            }

            result.Bits[0] = (BitState)value;
        }

        protected override IFIRType? MonoArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(1),
            SIntType a => new UIntType(1),
            _ => null
        };
    }

    public sealed class FIROrr : MonoArgMonoResPrimOp
    {
        public FIROrr(Source aIn, IFIRType outType, IFirrtlNode defNode) : base("orr", aIn, outType, defNode) { }

        protected override void MonoArgCompute(ref BinaryVarValue a, ref BinaryVarValue result)
        {
            if (!a.IsValidBinary)
            {
                result.SetAllUnknown();
                return;
            }

            int value = 0;
            for (int i = 0; i < a.Bits.Length; i++)
            {
                value |= (int)a.Bits[i];
            }

            result.Bits[0] = (BitState)value;
        }

        protected override IFIRType? MonoArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(1),
            SIntType a => new UIntType(1),
            _ => null
        };
    }

    public sealed class FIRXorr : MonoArgMonoResPrimOp
    {
        public FIRXorr(Source aIn, IFIRType outType, IFirrtlNode defNode) : base("xorr", aIn, outType, defNode) { }

        protected override void MonoArgCompute(ref BinaryVarValue a, ref BinaryVarValue result)
        {
            if (!a.IsValidBinary)
            {
                result.SetAllUnknown();
                return;
            }

            int value = 0;
            for (int i = 0; i < a.Bits.Length; i++)
            {
                value ^= (int)a.Bits[i];
            }

            result.Bits[0] = (BitState)value;
        }

        protected override IFIRType? MonoArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(1),
            SIntType a => new UIntType(1),
            _ => null
        };
    }
}