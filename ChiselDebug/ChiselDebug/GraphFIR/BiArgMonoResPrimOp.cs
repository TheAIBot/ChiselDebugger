using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
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

        public override Input[] GetInputs()
        {
            return new Input[] { A, B };
        }

        public override IEnumerable<FIRIO> GetIO()
        {
            yield return A;
            yield return B;
            yield return Result;
        }

        public override void Compute()
        {
            ref BinaryVarValue aVal = ref A.UpdateValueFromSourceFast();
            ref BinaryVarValue bVal = ref B.UpdateValueFromSourceFast();
            ref BinaryVarValue resultVal = ref Result.GetValue();

            Debug.Assert(aVal.IsValidBinary == aVal.Bits.IsAllBinary());
            Debug.Assert(bVal.IsValidBinary == bVal.Bits.IsAllBinary());

            resultVal.IsValidBinary = true;
            BiArgPropCompute(ref aVal, ref bVal, ref resultVal);

            Debug.Assert(resultVal.IsValidBinary == resultVal.Bits.IsAllBinary(), $"A: {aVal.BitsToString()}\nB: {bVal.BitsToString()}\nR: {resultVal.BitsToString()}");
        }
        protected abstract void BiArgPropCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result);

        protected abstract void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result);

        internal override void InferType()
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

    public abstract class BiArgPrimOpAlwaysPropUnknown : BiArgMonoResPrimOp
    {
        public BiArgPrimOpAlwaysPropUnknown(string opName, Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base(opName, aIn, bIn, outType, defNode)
        { }

        protected override void BiArgPropCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            if (!a.IsValidBinary || !b.IsValidBinary)
            {
                result.SetAllUnknown();
                return;
            }

            BiArgCompute(ref a, ref b, ref result);
        }
    }

    public abstract class BiArgPrimOpCheckPropUnknown : BiArgMonoResPrimOp
    {
        public BiArgPrimOpCheckPropUnknown(string opName, Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base(opName, aIn, bIn, outType, defNode)
        { }

        protected override void BiArgPropCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            BiArgCompute(ref a, ref b, ref result);
        }
    }

    public class FIRAdd : BiArgPrimOpAlwaysPropUnknown
    {
        public FIRAdd(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("+", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            BigInteger aVal = a.AsBigInteger(A.Type is SIntType);
            BigInteger bVal = b.AsBigInteger(A.Type is SIntType);
            result.SetBitsAndExtend(aVal + bVal, Result.Type is SIntType);
        }

        protected override IFIRType BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(Math.Max(a.Width, b.Width) + 1),
            (UIntType a, SIntType b) => new SIntType(Math.Max(a.Width, b.Width) + 1),
            (SIntType a, UIntType b) => new SIntType(Math.Max(a.Width, b.Width) + 1),
            (SIntType a, SIntType b) => new SIntType(Math.Max(a.Width, b.Width) + 1),
            _ => null
        };
    }

    public class FIRSub : BiArgPrimOpAlwaysPropUnknown
    {
        public FIRSub(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("-", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            BigInteger aVal = a.AsBigInteger(A.Type is SIntType);
            BigInteger bVal = b.AsBigInteger(A.Type is SIntType);
            result.SetBitsAndExtend(aVal - bVal, Result.Type is SIntType);
        }

        protected override IFIRType BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new SIntType(Math.Max(a.Width, b.Width) + 1),
            (UIntType a, SIntType b) => new SIntType(Math.Max(a.Width, b.Width) + 1),
            (SIntType a, UIntType b) => new SIntType(Math.Max(a.Width, b.Width) + 1),
            (SIntType a, SIntType b) => new SIntType(Math.Max(a.Width, b.Width) + 1),
            _ => null
        };
    }

    public class FIRMul : BiArgPrimOpAlwaysPropUnknown
    {
        public FIRMul(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("*", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            BigInteger aVal = a.AsBigInteger(A.Type is SIntType);
            BigInteger bVal = b.AsBigInteger(A.Type is SIntType);
            result.SetBitsAndExtend(aVal * bVal, Result.Type is SIntType);
        }

        protected override IFIRType BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(a.Width + b.Width),
            (UIntType a, SIntType b) => new SIntType(a.Width + b.Width),
            (SIntType a, UIntType b) => new SIntType(a.Width + b.Width),
            (SIntType a, SIntType b) => new SIntType(a.Width + b.Width),
            _ => null
        };
    }

    public class FIRDiv : BiArgPrimOpAlwaysPropUnknown
    {
        public FIRDiv(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("/", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            BigInteger aVal = a.AsBigInteger(A.Type is SIntType);
            BigInteger bVal = b.AsBigInteger(B.Type is SIntType);
            //Handle divide by zero
            if (bVal == 0)
            {
                result.SetAllUnknown();
            }
            else
            {
                result.SetBitsAndExtend(aVal / bVal, Result.Type is SIntType);
            }
        }

        protected override IFIRType BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(a.Width),
            (UIntType a, SIntType b) => new SIntType(a.Width + 1),
            (SIntType a, UIntType b) => new SIntType(a.Width),
            (SIntType a, SIntType b) => new SIntType(a.Width + 1),
            _ => null
        };
    }

    public class FIRRem : BiArgPrimOpAlwaysPropUnknown
    {
        public FIRRem(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("%", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            BigInteger aVal = a.AsBigInteger(A.Type is SIntType);
            BigInteger bVal = b.AsBigInteger(B.Type is SIntType);
            if (bVal == 0)
            {
                result.SetAllUnknown();
            }
            else
            {
                result.SetBitsAndExtend(aVal % bVal, Result.Type is SIntType);
            }
        }

        protected override IFIRType BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(Math.Min(a.Width, b.Width)),
            (UIntType a, SIntType b) => new UIntType(Math.Min(a.Width, b.Width)),
            (SIntType a, UIntType b) => new SIntType(Math.Min(a.Width, b.Width) + 1),
            (SIntType a, SIntType b) => new SIntType(Math.Min(a.Width, b.Width)),
            _ => null
        };
    }

    public class FIRDshl : BiArgPrimOpCheckPropUnknown
    {
        public FIRDshl(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("<<", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            if (!b.IsValidBinary)
            {
                result.SetAllUnknown();
                return;
            }

            int shift = b.AsInt();
            result.Bits.Slice(0, shift).Fill(BitState.Zero);

            int copyLength = Math.Min(result.Bits.Length - shift, a.Bits.Length);
            a.Bits.CopyTo(result.Bits.Slice(shift, copyLength));

            BitState signFill = A.Type is SIntType ? a.Bits[^1] : BitState.Zero;
            result.Bits.Slice(shift + copyLength).Fill(signFill);

            if (!a.IsValidBinary)
            {
                result.IsValidBinary = result.Bits.IsAllBinary();
            }
        }

        protected override IFIRType BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(a.Width + (1 << b.Width) - 1),
            (SIntType a, UIntType b) => new SIntType(a.Width + (1 << b.Width) - 1),
            _ => null
        };
    }

    public class FIRDshr : BiArgPrimOpCheckPropUnknown
    {
        public FIRDshr(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base(">>", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            if (!b.IsValidBinary)
            {
                result.SetAllUnknown();
                return;
            }

            int shift = b.AsInt();
            result.Bits.Fill(A.Type is SIntType ? a.Bits[^1] : BitState.Zero);
            a.Bits.Slice(Math.Min(a.Bits.Length - 1, shift), Math.Max(0, a.Bits.Length - shift)).CopyTo(result.Bits);

            if (!a.IsValidBinary)
            {
                result.IsValidBinary = result.Bits.IsAllBinary();
            }
        }

        protected override IFIRType BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(a.Width),
            (SIntType a, UIntType b) => new SIntType(a.Width),
            _ => null
        };
    }

    public class FIRCat : BiArgPrimOpCheckPropUnknown
    {
        public FIRCat(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("cat", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            Span<BitState> aCopy = result.Bits.Slice(b.Bits.Length);
            Span<BitState> bCopy = result.Bits.Slice(0);

            a.Bits.CopyTo(aCopy);
            b.Bits.CopyTo(bCopy);

            result.IsValidBinary = a.IsValidBinary & b.IsValidBinary;
        }

        protected override IFIRType BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(a.Width + b.Width),
            (UIntType a, SIntType b) => new UIntType(a.Width + b.Width),
            (SIntType a, UIntType b) => new UIntType(a.Width + b.Width),
            (SIntType a, SIntType b) => new UIntType(a.Width + b.Width),
            _ => null
        };
    }

    public abstract class FIRCompOp : BiArgPrimOpAlwaysPropUnknown
    {
        public FIRCompOp(string opName, Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base(opName, aIn, bIn, outType, defNode) { }

        protected override IFIRType BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(1),
            (UIntType a, SIntType b) => new UIntType(1),
            (SIntType a, UIntType b) => new UIntType(1),
            (SIntType a, SIntType b) => new UIntType(1),
            _ => null
        };
    }

    public class FIREq : FIRCompOp
    {
        public FIREq(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("=", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            bool value = a.SameValue(ref b, A.Type is SIntType);
            result.Bits[0] = value ? BitState.One : BitState.Zero;
        }
    }

    public class FIRNeq : FIRCompOp
    {
        public FIRNeq(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("≠", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            bool value = a.SameValue(ref b, A.Type is SIntType);
            result.Bits[0] = !value ? BitState.One : BitState.Zero;
        }
    }

    public class FIRGeq : FIRCompOp
    {
        public FIRGeq(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("≥", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            BigInteger aVal = a.AsBigInteger(A.Type is SIntType);
            BigInteger bVal = b.AsBigInteger(B.Type is SIntType);
            result.SetBits(aVal >= bVal ? 1 : 0);
        }
    }

    public class FIRLeq : FIRCompOp
    {
        public FIRLeq(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("≤", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            BigInteger aVal = a.AsBigInteger(A.Type is SIntType);
            BigInteger bVal = b.AsBigInteger(B.Type is SIntType);
            result.SetBits(aVal <= bVal ? 1 : 0);
        }
    }

    public class FIRGt : FIRCompOp
    {
        public FIRGt(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base(">", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            BigInteger aVal = a.AsBigInteger(A.Type is SIntType);
            BigInteger bVal = b.AsBigInteger(B.Type is SIntType);
            result.SetBits(aVal > bVal ? 1 : 0);
        }
    }

    public class FIRLt : FIRCompOp
    {
        public FIRLt(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("<", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            BigInteger aVal = a.AsBigInteger(A.Type is SIntType);
            BigInteger bVal = b.AsBigInteger(B.Type is SIntType);
            result.SetBits(aVal < bVal ? 1 : 0);
        }
    }

    public abstract class FIRBitwise : BiArgPrimOpCheckPropUnknown
    {
        public FIRBitwise(string opName, Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base(opName, aIn, bIn, outType, defNode) { }

        protected override IFIRType BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(Math.Max(a.Width, b.Width)),
            (UIntType a, SIntType b) => new UIntType(Math.Max(a.Width, b.Width)),
            (SIntType a, UIntType b) => new UIntType(Math.Max(a.Width, b.Width)),
            (SIntType a, SIntType b) => new UIntType(Math.Max(a.Width, b.Width)),
            _ => null
        };

        public static BitState CompOpPropX(BitState a, BitState b, BitState computed)
        {
            return CompOpPropX(a | b, computed);
        }

        public static BitState CompOpPropX(BitState a, BitState computed)
        {
            BitState isNotBinary = a & BitState.X;
            return isNotBinary | ((BitState)(((int)isNotBinary >> 1) ^ 1) & computed);
        }
    }

    public class FIRAnd : FIRBitwise
    {
        public FIRAnd(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("&", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            int length = Math.Min(a.Bits.Length, b.Bits.Length);
            for (int i = 0; i < length; i++)
            {
                result.Bits[i] = CompOpPropX(a.Bits[i], b.Bits[i], a.Bits[i] & b.Bits[i]);
            }

            if (a.Bits.Length > b.Bits.Length)
            {
                BitState bEnd = B.Type is SIntType ? b.Bits[^1] : BitState.Zero;
                for (int i = length; i < a.Bits.Length; i++)
                {
                    result.Bits[i] = CompOpPropX(a.Bits[i], bEnd, a.Bits[i] & bEnd);
                }
            }
            else if (a.Bits.Length < b.Bits.Length)
            {
                BitState aEnd = A.Type is SIntType ? a.Bits[^1] : BitState.Zero;
                for (int i = length; i < b.Bits.Length; i++)
                {
                    result.Bits[i] = CompOpPropX(aEnd, b.Bits[i], aEnd & b.Bits[i]);
                }
            }

            result.IsValidBinary = a.IsValidBinary & b.IsValidBinary;
        }
    }

    public class FIROr : FIRBitwise
    {
        public FIROr(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("|", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            int length = Math.Min(a.Bits.Length, b.Bits.Length);
            for (int i = 0; i < length; i++)
            {
                result.Bits[i] = CompOpPropX(a.Bits[i], b.Bits[i], a.Bits[i] | b.Bits[i]);
            }

            if (a.Bits.Length > b.Bits.Length)
            {
                BitState bEnd = B.Type is SIntType ? b.Bits[^1] : BitState.Zero;
                for (int i = length; i < a.Bits.Length; i++)
                {
                    result.Bits[i] = CompOpPropX(a.Bits[i], bEnd, a.Bits[i] | bEnd);
                }
            }
            else if (a.Bits.Length < b.Bits.Length)
            {
                BitState aEnd = A.Type is SIntType ? a.Bits[^1] : BitState.Zero;
                for (int i = length; i < b.Bits.Length; i++)
                {
                    result.Bits[i] = CompOpPropX(aEnd, b.Bits[i], aEnd | b.Bits[i]);
                }
            }

            result.IsValidBinary = a.IsValidBinary & b.IsValidBinary;
        }
    }

    public class FIRXor : FIRBitwise
    {
        public FIRXor(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("^", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            int length = Math.Min(a.Bits.Length, b.Bits.Length);
            for (int i = 0; i < length; i++)
            {
                result.Bits[i] = CompOpPropX(a.Bits[i], b.Bits[i], a.Bits[i] ^ b.Bits[i]);
            }

            if (a.Bits.Length > b.Bits.Length)
            {
                BitState bEnd = B.Type is SIntType ? b.Bits[^1] : BitState.Zero;
                for (int i = length; i < a.Bits.Length; i++)
                {
                    result.Bits[i] = CompOpPropX(a.Bits[i], bEnd, a.Bits[i] ^ bEnd);
                }
            }
            else if (a.Bits.Length < b.Bits.Length)
            {
                BitState aEnd = A.Type is SIntType ? a.Bits[^1] : BitState.Zero;
                for (int i = length; i < b.Bits.Length; i++)
                {
                    result.Bits[i] = CompOpPropX(aEnd, b.Bits[i], aEnd ^ b.Bits[i]);
                }
            }

            result.IsValidBinary = a.IsValidBinary & b.IsValidBinary;
        }
    }

    public class FIRShl : BiArgPrimOpCheckPropUnknown
    {
        private readonly int ShiftBy;
        public FIRShl(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base("<<", aIn, bIn, outType, defNode)
        {
            this.ShiftBy = (int)((ConstValue)bIn.Node).Value.Value;
        }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            for (int i = 0; i < a.Bits.Length; i++)
            {
                result.Bits[i + ShiftBy] = a.Bits[i];
            }
            result.Bits.Slice(0, ShiftBy).Fill(BitState.Zero);

            if (!a.IsValidBinary)
            {
                result.IsValidBinary = result.Bits.IsAllBinary();
            }
        }

        protected override IFIRType BiArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(a.Width + ShiftBy),
            SIntType a => new SIntType(a.Width + ShiftBy),
            _ => null
        };
    }

    public class FIRShr : BiArgPrimOpCheckPropUnknown
    {
        private readonly int ShiftBy;
        public FIRShr(Output aIn, Output bIn, IFIRType outType, FirrtlNode defNode) : base(">>", aIn, bIn, outType, defNode)
        {
            this.ShiftBy = (int)((ConstValue)bIn.Node).Value.Value;
        }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            //Special logic for when shifting by more than the wire length.
            //In that case the result should be equal to zero if unsigned and
            //equal to sign bit if signed.
            if (ShiftBy >= a.Bits.Length)
            {
                if (A.Type is UIntType)
                {
                    result.Bits[0] = BitState.Zero;
                }
                else
                {
                    result.Bits[0] = a.Bits[^1];
                }
            }
            else
            {
                for (int i = 0; i < result.Bits.Length; i++)
                {
                    result.Bits[i] = a.Bits[i + ShiftBy];
                }
            }

            if (!a.IsValidBinary)
            {
                result.IsValidBinary = result.Bits.IsAllBinary();
            }
        }

        protected override IFIRType BiArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(Math.Max(a.Width - ShiftBy, 1)),
            SIntType a => new SIntType(Math.Max(a.Width - ShiftBy, 1)),
            _ => null
        };
    }
}