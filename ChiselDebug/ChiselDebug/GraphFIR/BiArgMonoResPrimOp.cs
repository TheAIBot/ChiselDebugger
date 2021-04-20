using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Diagnostics;
using System.Numerics;
using VCDReader;

namespace ChiselDebug.GraphFIR
{
    public abstract class BiArgMonoResPrimOp : FIRRTLPrimOP
    {
        public readonly string OpName;
        public readonly Input A;
        public readonly Input B;

        public BiArgMonoResPrimOp(string opName, Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base(outType, defNode)
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

        public override void Compute()
        {
            Connection aCon = A.GetEnabledCon();
            Connection bCon = B.GetEnabledCon();
            Connection resultCon = Result.Con;

            BinaryVarValue aVal = (BinaryVarValue)aCon.Value.GetValue();
            BinaryVarValue bVal = (BinaryVarValue)bCon.Value.GetValue();
            BinaryVarValue resultVal = (BinaryVarValue)resultCon.Value.GetValue();

            if (!aVal.IsValidBinary() || !bVal.IsValidBinary())
            {
                Array.Fill(resultVal.Bits, BitState.X);
            }

            BiArgCompute(aVal, bVal, resultVal);
        }
        protected abstract void BiArgCompute(BinaryVarValue a, BinaryVarValue b, BinaryVarValue result);

        public override void InferType()
        {
            if (Result.Type is GroundType ground && ground.IsTypeFullyKnown())
            {
                return;
            }

            A.InferType();
            B.InferType();

            Result.SetType(BiArgInferType());
        }
        protected abstract IFIRType BiArgInferType();
    }

    public class FIRAdd : BiArgMonoResPrimOp
    {
        public FIRAdd(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("+", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(BinaryVarValue a, BinaryVarValue b, BinaryVarValue result)
        {
            const int bitsInLong = 64;
            if (a.Bits.Length <= bitsInLong && 
                b.Bits.Length <= bitsInLong && 
                result.Bits.Length <= bitsInLong)
            {
                ulong aVal = a.AsULong();
                ulong bVal = b.AsULong();
                result.SetBits(aVal + bVal);
            }
            else
            {
                BigInteger aVal = a.AsUnsignedBigInteger();
                BigInteger bVal = b.AsUnsignedBigInteger();
                result.SetBitsAndExtend(aVal + bVal, Result.Type is SIntType);
            }
        }

        protected override IFIRType BiArgInferType() => (A.Type, B.Type) switch
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
        public FIRSub(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("-", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(BinaryVarValue a, BinaryVarValue b, BinaryVarValue result)
        {
            const int bitsInLong = 64;
            if (a.Bits.Length <= bitsInLong &&
                b.Bits.Length <= bitsInLong &&
                result.Bits.Length <= bitsInLong)
            {
                ulong aVal = a.AsULong();
                ulong bVal = b.AsULong();
                result.SetBits(aVal - bVal);
            }
            else
            {
                BigInteger aVal = a.AsUnsignedBigInteger();
                BigInteger bVal = b.AsUnsignedBigInteger();
                result.SetBitsAndExtend(aVal - bVal, Result.Type is SIntType);
            }
        }

        protected override IFIRType BiArgInferType() => (A.Type, B.Type) switch
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
        public FIRMul(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("*", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(BinaryVarValue a, BinaryVarValue b, BinaryVarValue result)
        {
            const int bitsInLong = 64;
            if (a.Bits.Length <= bitsInLong &&
                b.Bits.Length <= bitsInLong &&
                result.Bits.Length <= bitsInLong)
            {
                ulong aVal = a.AsULong();
                ulong bVal = b.AsULong();
                result.SetBits(aVal * bVal);
            }
            else
            {
                BigInteger aVal = a.AsUnsignedBigInteger();
                BigInteger bVal = b.AsUnsignedBigInteger();
                result.SetBitsAndExtend(aVal * bVal, Result.Type is SIntType);
            }
        }

        protected override IFIRType BiArgInferType() => (A.Type, B.Type) switch
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
        public FIRDiv(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("/", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(BinaryVarValue a, BinaryVarValue b, BinaryVarValue result)
        {
            const int bitsInLong = 64;
            if (a.Bits.Length <= bitsInLong &&
                b.Bits.Length <= bitsInLong &&
                result.Bits.Length <= bitsInLong)
            {
                ulong aVal = a.AsULong();
                ulong bVal = b.AsULong();
                result.SetBits(aVal / bVal);
            }
            else
            {
                BigInteger aVal = a.AsBigInteger(A.Type is SIntType);
                BigInteger bVal = b.AsBigInteger(B.Type is SIntType);
                result.SetBitsAndExtend(aVal / bVal, Result.Type is SIntType);
            }
        }

        protected override IFIRType BiArgInferType() => (A.Type, B.Type) switch
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
        public FIRRem(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("%", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(BinaryVarValue a, BinaryVarValue b, BinaryVarValue result)
        {
            const int bitsInLong = 64;
            if (a.Bits.Length <= bitsInLong &&
                b.Bits.Length <= bitsInLong &&
                result.Bits.Length <= bitsInLong)
            {
                ulong aVal = a.AsULong();
                ulong bVal = b.AsULong();
                result.SetBits(aVal % bVal);
            }
            else
            {
                BigInteger aVal = a.AsBigInteger(A.Type is SIntType);
                BigInteger bVal = b.AsBigInteger(B.Type is SIntType);
                result.SetBitsAndExtend(aVal % bVal, Result.Type is SIntType);
            }
        }

        protected override IFIRType BiArgInferType() => (A.Type, B.Type) switch
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
        public FIRDshl(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("<<", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(BinaryVarValue a, BinaryVarValue b, BinaryVarValue result)
        {
            int shift = b.AsInt();
            for (int i = 0; i < a.Bits.Length; i++)
            {
                result.Bits[i + shift] = a.Bits[i];
            }
            Array.Fill(result.Bits, BitState.Zero, 0, shift);
        }

        protected override IFIRType BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(a.Width + (1 << b.Width)),
            (SIntType a, UIntType b) => new SIntType(a.Width + (1 << b.Width)),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRDshr : BiArgMonoResPrimOp
    {
        public FIRDshr(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base(">>", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(BinaryVarValue a, BinaryVarValue b, BinaryVarValue result)
        {
            int shift = b.AsInt();
            for (int i = 0; i < a.Bits.Length; i++)
            {
                result.Bits[i] = a.Bits[i + shift];
            }
            Array.Fill(result.Bits, a.Bits[^1], result.Bits.Length - 1 - shift, shift);
        }

        protected override IFIRType BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(a.Width),
            (SIntType a, UIntType b) => new SIntType(a.Width),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRCat : BiArgMonoResPrimOp
    {
        public FIRCat(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("cat", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(BinaryVarValue a, BinaryVarValue b, BinaryVarValue result)
        {
            Array.Copy(a.Bits, 0, result.Bits, b.Bits.Length, a.Bits.Length);
            Array.Copy(b.Bits, result.Bits, b.Bits.Length);
        }

        protected override IFIRType BiArgInferType() => (A.Type, B.Type) switch
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
        public FIRCompOp(string opName, Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base(opName, aIn, bIn, outType, defNode) { }

        protected override IFIRType BiArgInferType() => (A.Type, B.Type) switch
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
        public FIREq(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("=", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(BinaryVarValue a, BinaryVarValue b, BinaryVarValue result)
        {
            bool value = a.SameValueZeroExtend(b);
            result.Bits[0] = value ? BitState.One : BitState.Zero;
        }
    }
    
    public class FIRNeq : FIRCompOp
    {
        public FIRNeq(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("≠", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(BinaryVarValue a, BinaryVarValue b, BinaryVarValue result)
        {
            bool value = a.SameValueZeroExtend(b);
            result.Bits[0] = !value ? BitState.One : BitState.Zero;
        }
    }
    
    public class FIRGeq : FIRCompOp
    {
        public FIRGeq(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("≥", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(BinaryVarValue a, BinaryVarValue b, BinaryVarValue result)
        {
            const int bitsInLong = 64;
            if (a.Bits.Length <= bitsInLong &&
                b.Bits.Length <= bitsInLong)
            {
                if (A.Type is UIntType && B.Type is UIntType)
                {
                    ulong aVal = a.AsULong();
                    ulong bVal = b.AsULong();
                    result.SetBits(aVal >= bVal ? 1 : 0);
                }
                else
                {
                    Debug.Assert(A.Type is SIntType && B.Type is SIntType);
                    long aVal = a.AsLong();
                    long bVal = b.AsLong();
                    result.SetBits(aVal >= bVal ? 1 : 0);
                }

            }
            else
            {
                BigInteger aVal = a.AsBigInteger(A.Type is SIntType);
                BigInteger bVal = b.AsBigInteger(B.Type is SIntType);
                result.SetBits(aVal >= bVal ? 1 : 0);
            }
        }
    }
    
    public class FIRLeq : FIRCompOp
    {
        public FIRLeq(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("≤", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(BinaryVarValue a, BinaryVarValue b, BinaryVarValue result)
        {
            const int bitsInLong = 64;
            if (a.Bits.Length <= bitsInLong &&
                b.Bits.Length <= bitsInLong)
            {
                if (A.Type is UIntType && B.Type is UIntType)
                {
                    ulong aVal = a.AsULong();
                    ulong bVal = b.AsULong();
                    result.SetBits(aVal <= bVal ? 1 : 0);
                }
                else
                {
                    Debug.Assert(A.Type is SIntType && B.Type is SIntType);
                    long aVal = a.AsLong();
                    long bVal = b.AsLong();
                    result.SetBits(aVal <= bVal ? 1 : 0);
                }

            }
            else
            {
                BigInteger aVal = a.AsBigInteger(A.Type is SIntType);
                BigInteger bVal = b.AsBigInteger(B.Type is SIntType);
                result.SetBits(aVal <= bVal ? 1 : 0);
            }
        }
    }
    
    public class FIRGt : FIRCompOp
    {
        public FIRGt(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base(">", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(BinaryVarValue a, BinaryVarValue b, BinaryVarValue result)
        {
            const int bitsInLong = 64;
            if (a.Bits.Length <= bitsInLong &&
                b.Bits.Length <= bitsInLong)
            {
                if (A.Type is UIntType && B.Type is UIntType)
                {
                    ulong aVal = a.AsULong();
                    ulong bVal = b.AsULong();
                    result.SetBits(aVal > bVal ? 1 : 0);
                }
                else
                {
                    Debug.Assert(A.Type is SIntType && B.Type is SIntType);
                    long aVal = a.AsLong();
                    long bVal = b.AsLong();
                    result.SetBits(aVal > bVal ? 1 : 0);
                }

            }
            else
            {
                BigInteger aVal = a.AsBigInteger(A.Type is SIntType);
                BigInteger bVal = b.AsBigInteger(B.Type is SIntType);
                result.SetBits(aVal > bVal ? 1 : 0);
            }
        }
    }
    
    public class FIRLt : FIRCompOp
    {
        public FIRLt(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("<", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(BinaryVarValue a, BinaryVarValue b, BinaryVarValue result)
        {
            const int bitsInLong = 64;
            if (a.Bits.Length <= bitsInLong &&
                b.Bits.Length <= bitsInLong)
            {
                if (A.Type is UIntType && B.Type is UIntType)
                {
                    ulong aVal = a.AsULong();
                    ulong bVal = b.AsULong();
                    result.SetBits(aVal < bVal ? 1 : 0);
                }
                else
                {
                    Debug.Assert(A.Type is SIntType && B.Type is SIntType);
                    long aVal = a.AsLong();
                    long bVal = b.AsLong();
                    result.SetBits(aVal < bVal ? 1 : 0);
                }

            }
            else
            {
                BigInteger aVal = a.AsBigInteger(A.Type is SIntType);
                BigInteger bVal = b.AsBigInteger(B.Type is SIntType);
                result.SetBits(aVal < bVal ? 1 : 0);
            }
        }
    }

    public abstract class FIRBitwise : BiArgMonoResPrimOp
    {
        public FIRBitwise(string opName, Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base(opName, aIn, bIn, outType, defNode) { }

        protected override IFIRType BiArgInferType() => (A.Type, B.Type) switch
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
        public FIRAnd(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("&", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(BinaryVarValue a, BinaryVarValue b, BinaryVarValue result)
        {
            for (int i = 0; i < a.Bits.Length; i++)
            {
                result.Bits[i] = (BitState)((int)a.Bits[i] & (int)b.Bits[i]);
            }
        }
    }

    public class FIROr : FIRBitwise
    {
        public FIROr(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("|", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(BinaryVarValue a, BinaryVarValue b, BinaryVarValue result)
        {
            for (int i = 0; i < a.Bits.Length; i++)
            {
                result.Bits[i] = (BitState)((int)a.Bits[i] | (int)b.Bits[i]);
            }
        }
    }

    public class FIRXor : FIRBitwise
    {
        public FIRXor(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("^", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(BinaryVarValue a, BinaryVarValue b, BinaryVarValue result)
        {
            for (int i = 0; i < a.Bits.Length; i++)
            {
                result.Bits[i] |= (BitState)((int)a.Bits[i] ^ (int)b.Bits[i]);
            }
        }
    }

    public class FIRShl : BiArgMonoResPrimOp
    {
        private readonly int ShiftBy;
        public FIRShl(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("<<", aIn, bIn, outType, defNode) 
        {
            this.ShiftBy = (int)((ConstValue)bIn.Node).Value.Value;
        }

        protected override void BiArgCompute(BinaryVarValue a, BinaryVarValue b, BinaryVarValue result)
        {
            for (int i = 0; i < a.Bits.Length; i++)
            {
                result.Bits[i + ShiftBy] = a.Bits[i];
            }
            Array.Fill(result.Bits, BitState.Zero, 0, ShiftBy);
        }

        protected override IFIRType BiArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(a.Width + ShiftBy),
            SIntType a => new SIntType(a.Width + ShiftBy),
            _ => throw new Exception("Failed to infer type.")
        };
    }

    public class FIRShr : BiArgMonoResPrimOp
    {
        private readonly int ShiftBy;
        public FIRShr(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base(">>", aIn, bIn, outType, defNode)
        {
            this.ShiftBy = (int)((ConstValue)bIn.Node).Value.Value;
        }

        protected override void BiArgCompute(BinaryVarValue a, BinaryVarValue b, BinaryVarValue result)
        {
            for (int i = 0; i < a.Bits.Length; i++)
            {
                result.Bits[i] = a.Bits[i + ShiftBy];
            }
            Array.Fill(result.Bits, a.Bits[^1], result.Bits.Length - 1 - ShiftBy, ShiftBy);
        }

        protected override IFIRType BiArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(a.Width - ShiftBy),
            SIntType a => new SIntType(a.Width - ShiftBy),
            _ => throw new Exception("Failed to infer type.")
        };
    }
}
