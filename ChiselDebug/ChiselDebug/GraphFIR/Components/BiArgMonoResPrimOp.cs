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
    public abstract class BiArgMonoResPrimOp : FIRRTLPrimOP
    {
        public readonly string OpName;
        public readonly Sink A;
        public readonly Sink B;

        public BiArgMonoResPrimOp(string opName, Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base(outType, defNode)
        {
            OpName = opName;
            A = new Sink(this, aIn.Type);
            B = new Sink(this, bIn.Type);

            aIn.ConnectToInput(A);
            bIn.ConnectToInput(B);
        }

        public override Sink[] GetSinks()
        {
            return new Sink[] { A, B };
        }

        public override IEnumerable<FIRIO> GetIO()
        {
            yield return A;
            yield return B;
            yield return Result;
        }

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
        protected abstract IFIRType? BiArgInferType();
    }

    public abstract class BiArgPrimOpAlwaysPropUnknown : BiArgMonoResPrimOp
    {
        public BiArgPrimOpAlwaysPropUnknown(string opName, Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base(opName, aIn, bIn, outType, defNode)
        { }

        public override void Compute()
        {
            if (Result.Value == null)
            {
                throw new InvalidOperationException($"{nameof(Result)} value not initialized.");
            }

            ValueType a = A.FetchValueFromSourceFast();
            ValueType b = B.FetchValueFromSourceFast();
            if (!a.Value.IsValidBinary || !b.Value.IsValidBinary)
            {
                Result.Value.Value.SetAllUnknown();
                return;
            }

            Result.Value.Value.IsValidBinary = true;
            BiArgCompute(a, b, Result.Value);
        }

        protected abstract void BiArgCompute(ValueType a, ValueType b, ValueType result);
    }

    public abstract class BiArgPrimOpCheckPropUnknown : BiArgMonoResPrimOp
    {
        public BiArgPrimOpCheckPropUnknown(string opName, Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base(opName, aIn, bIn, outType, defNode)
        { }

        public override void Compute()
        {
            ref BinaryVarValue aVal = ref A.UpdateValueFromSourceFast();
            ref BinaryVarValue bVal = ref B.UpdateValueFromSourceFast();
            ref BinaryVarValue resultVal = ref Result.GetValue();

            Debug.Assert(aVal.IsValidBinary == aVal.Bits.IsAllBinary());
            Debug.Assert(bVal.IsValidBinary == bVal.Bits.IsAllBinary());

            resultVal.IsValidBinary = true;
            BiArgCompute(ref aVal, ref bVal, ref resultVal);

            Debug.Assert(resultVal.IsValidBinary == resultVal.Bits.IsAllBinary(), $"A: {aVal.BitsToString()}\nB: {bVal.BitsToString()}\nR: {resultVal.BitsToString()}");
        }

        protected abstract void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result);
    }

    public sealed class FIRAdd : BiArgPrimOpAlwaysPropUnknown
    {
        public FIRAdd(Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base("+", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ValueType a, ValueType b, ValueType result)
        {
            result.SetFromBigInt(a.GetAsBigInt() + b.GetAsBigInt());
        }

        protected override IFIRType? BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(Math.Max(a.Width, b.Width) + 1),
            (UIntType a, SIntType b) => new SIntType(Math.Max(a.Width, b.Width) + 1),
            (SIntType a, UIntType b) => new SIntType(Math.Max(a.Width, b.Width) + 1),
            (SIntType a, SIntType b) => new SIntType(Math.Max(a.Width, b.Width) + 1),
            _ => null
        };
    }

    public sealed class FIRSub : BiArgPrimOpAlwaysPropUnknown
    {
        public FIRSub(Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base("-", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ValueType a, ValueType b, ValueType result)
        {
            result.SetFromBigInt(a.GetAsBigInt() - b.GetAsBigInt());
        }

        protected override IFIRType? BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new SIntType(Math.Max(a.Width, b.Width) + 1),
            (UIntType a, SIntType b) => new SIntType(Math.Max(a.Width, b.Width) + 1),
            (SIntType a, UIntType b) => new SIntType(Math.Max(a.Width, b.Width) + 1),
            (SIntType a, SIntType b) => new SIntType(Math.Max(a.Width, b.Width) + 1),
            _ => null
        };
    }

    public sealed class FIRMul : BiArgPrimOpAlwaysPropUnknown
    {
        public FIRMul(Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base("*", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ValueType a, ValueType b, ValueType result)
        {
            result.SetFromBigInt(a.GetAsBigInt() * b.GetAsBigInt());
        }

        protected override IFIRType? BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(a.Width + b.Width),
            (UIntType a, SIntType b) => new SIntType(a.Width + b.Width),
            (SIntType a, UIntType b) => new SIntType(a.Width + b.Width),
            (SIntType a, SIntType b) => new SIntType(a.Width + b.Width),
            _ => null
        };
    }

    public sealed class FIRDiv : BiArgPrimOpAlwaysPropUnknown
    {
        public FIRDiv(Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base("/", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ValueType a, ValueType b, ValueType result)
        {
            BigInteger aVal = a.GetAsBigInt();
            BigInteger bVal = b.GetAsBigInt();
            //Handle divide by zero
            if (bVal == 0)
            {
                result.Value.SetAllUnknown();
            }
            else
            {
                result.SetFromBigInt(aVal / bVal);
            }
        }

        protected override IFIRType? BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(a.Width),
            (UIntType a, SIntType b) => new SIntType(a.Width + 1),
            (SIntType a, UIntType b) => new SIntType(a.Width),
            (SIntType a, SIntType b) => new SIntType(a.Width + 1),
            _ => null
        };
    }

    public sealed class FIRRem : BiArgPrimOpAlwaysPropUnknown
    {
        public FIRRem(Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base("%", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ValueType a, ValueType b, ValueType result)
        {
            BigInteger aVal = a.GetAsBigInt();
            BigInteger bVal = b.GetAsBigInt();
            //Handle divide by zero
            if (bVal == 0)
            {
                result.Value.SetAllUnknown();
            }
            else
            {
                result.SetFromBigInt(aVal % bVal);
            }
        }

        protected override IFIRType? BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(Math.Min(a.Width, b.Width)),
            (UIntType a, SIntType b) => new UIntType(Math.Min(a.Width, b.Width)),
            (SIntType a, UIntType b) => new SIntType(Math.Min(a.Width, b.Width) + 1),
            (SIntType a, SIntType b) => new SIntType(Math.Min(a.Width, b.Width)),
            _ => null
        };
    }

    public sealed class FIRDshl : BiArgPrimOpCheckPropUnknown
    {
        public FIRDshl(Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base("<<", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            if (!b.IsValidBinary)
            {
                result.SetAllUnknown();
                return;
            }

            if (A.Value == null)
            {
                throw new InvalidOperationException("Value not initialized.");
            }

            int shift = b.AsInt();
            result.Bits.Slice(0, shift).Fill(BitState.Zero);

            int copyLength = Math.Min(result.Bits.Length - shift, a.Bits.Length);
            a.Bits.CopyTo(result.Bits.Slice(shift, copyLength));

            BitState signFill = A.Value.IsSigned ? a.Bits[^1] : BitState.Zero;
            result.Bits.Slice(shift + copyLength).Fill(signFill);

            if (!a.IsValidBinary)
            {
                result.IsValidBinary = result.Bits.IsAllBinary();
            }
        }

        protected override IFIRType? BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(a.Width + (1 << b.Width) - 1),
            (SIntType a, UIntType b) => new SIntType(a.Width + (1 << b.Width) - 1),
            _ => null
        };
    }

    public sealed class FIRDshr : BiArgPrimOpCheckPropUnknown
    {
        public FIRDshr(Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base(">>", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            if (!b.IsValidBinary)
            {
                result.SetAllUnknown();
                return;
            }

            if (A.Value == null)
            {
                throw new InvalidOperationException("Value not initialized.");
            }

            int shift = b.AsInt();
            result.Bits.Fill(A.Value.IsSigned ? a.Bits[^1] : BitState.Zero);
            a.Bits.Slice(Math.Min(a.Bits.Length - 1, shift), Math.Max(0, a.Bits.Length - shift)).CopyTo(result.Bits);

            if (!a.IsValidBinary)
            {
                result.IsValidBinary = result.Bits.IsAllBinary();
            }
        }

        protected override IFIRType? BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(a.Width),
            (SIntType a, UIntType b) => new SIntType(a.Width),
            _ => null
        };
    }

    public sealed class FIRCat : BiArgPrimOpCheckPropUnknown
    {
        public FIRCat(Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base("cat", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            Span<BitState> aCopy = result.Bits.Slice(b.Bits.Length);
            Span<BitState> bCopy = result.Bits.Slice(0);

            a.Bits.CopyTo(aCopy);
            b.Bits.CopyTo(bCopy);

            result.IsValidBinary = a.IsValidBinary & b.IsValidBinary;
        }

        protected override IFIRType? BiArgInferType() => (A.Type, B.Type) switch
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
        public FIRCompOp(string opName, Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base(opName, aIn, bIn, outType, defNode) { }

        protected override IFIRType? BiArgInferType() => (A.Type, B.Type) switch
        {
            (UIntType a, UIntType b) => new UIntType(1),
            (UIntType a, SIntType b) => new UIntType(1),
            (SIntType a, UIntType b) => new UIntType(1),
            (SIntType a, SIntType b) => new UIntType(1),
            _ => null
        };
    }

    public sealed class FIREq : FIRCompOp
    {
        public FIREq(Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base("=", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ValueType a, ValueType b, ValueType result)
        {
            bool value = a.SameValueAs(b);
            result.Value.Bits[0] = value ? BitState.One : BitState.Zero;
        }
    }

    public sealed class FIRNeq : FIRCompOp
    {
        public FIRNeq(Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base("≠", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ValueType a, ValueType b, ValueType result)
        {
            bool value = a.SameValueAs(b);
            result.Value.Bits[0] = !value ? BitState.One : BitState.Zero;
        }
    }

    public sealed class FIRGeq : FIRCompOp
    {
        public FIRGeq(Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base("≥", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ValueType a, ValueType b, ValueType result)
        {
            BigInteger aVal = a.GetAsBigInt();
            BigInteger bVal = b.GetAsBigInt();
            result.Value.SetBits(aVal >= bVal ? 1 : 0);
        }
    }

    public sealed class FIRLeq : FIRCompOp
    {
        public FIRLeq(Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base("≤", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ValueType a, ValueType b, ValueType result)
        {
            BigInteger aVal = a.GetAsBigInt();
            BigInteger bVal = b.GetAsBigInt();
            result.Value.SetBits(aVal <= bVal ? 1 : 0);
        }
    }

    public sealed class FIRGt : FIRCompOp
    {
        public FIRGt(Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base(">", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ValueType a, ValueType b, ValueType result)
        {
            BigInteger aVal = a.GetAsBigInt();
            BigInteger bVal = b.GetAsBigInt();
            result.Value.SetBits(aVal > bVal ? 1 : 0);
        }
    }

    public sealed class FIRLt : FIRCompOp
    {
        public FIRLt(Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base("<", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ValueType a, ValueType b, ValueType result)
        {
            BigInteger aVal = a.GetAsBigInt();
            BigInteger bVal = b.GetAsBigInt();
            result.Value.SetBits(aVal < bVal ? 1 : 0);
        }
    }

    public abstract class FIRBitwise : BiArgPrimOpCheckPropUnknown
    {
        public FIRBitwise(string opName, Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base(opName, aIn, bIn, outType, defNode) { }

        protected override IFIRType? BiArgInferType() => (A.Type, B.Type) switch
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
            return isNotBinary | (BitState)((int)isNotBinary >> 1 ^ 1) & computed;
        }
    }

    public sealed class FIRAnd : FIRBitwise
    {
        public FIRAnd(Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base("&", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            static void ExtendedBitwise(int length, Sink shorter, Span<BitState> shorterBits, Span<BitState> longerBits, Span<BitState> resultBits)
            {
                if (shorter.Value == null)
                {
                    throw new InvalidOperationException("Value not initialized.");
                }

                BitState shorterEnd = shorter.Value.IsSigned ? shorterBits[^1] : BitState.Zero;
                for (int i = length; i < longerBits.Length; i++)
                {
                    resultBits[i] = CompOpPropX(longerBits[i], shorterEnd, longerBits[i] & shorterEnd);
                }
            }

            int length = Math.Min(a.Bits.Length, b.Bits.Length);
            for (int i = 0; i < length; i++)
            {
                result.Bits[i] = CompOpPropX(a.Bits[i], b.Bits[i], a.Bits[i] & b.Bits[i]);
            }

            if (a.Bits.Length > b.Bits.Length)
            {
                ExtendedBitwise(length, B, b.Bits, a.Bits, result.Bits);
            }
            else if (a.Bits.Length < b.Bits.Length)
            {
                ExtendedBitwise(length, A, a.Bits, b.Bits, result.Bits);
            }

            result.IsValidBinary = a.IsValidBinary & b.IsValidBinary;
        }
    }

    public sealed class FIROr : FIRBitwise
    {
        public FIROr(Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base("|", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            static void ExtendedBitwise(int length, Sink shorter, Span<BitState> shorterBits, Span<BitState> longerBits, Span<BitState> resultBits)
            {
                if (shorter.Value == null)
                {
                    throw new InvalidOperationException("Value not initialized.");
                }

                BitState shorterEnd = shorter.Value.IsSigned ? shorterBits[^1] : BitState.Zero;
                for (int i = length; i < longerBits.Length; i++)
                {
                    resultBits[i] = CompOpPropX(longerBits[i], shorterEnd, longerBits[i] | shorterEnd);
                }
            }

            int length = Math.Min(a.Bits.Length, b.Bits.Length);
            for (int i = 0; i < length; i++)
            {
                result.Bits[i] = CompOpPropX(a.Bits[i], b.Bits[i], a.Bits[i] | b.Bits[i]);
            }

            if (a.Bits.Length > b.Bits.Length)
            {
                ExtendedBitwise(length, B, b.Bits, a.Bits, result.Bits);
            }
            else if (a.Bits.Length < b.Bits.Length)
            {
                ExtendedBitwise(length, A, a.Bits, b.Bits, result.Bits);
            }

            result.IsValidBinary = a.IsValidBinary & b.IsValidBinary;
        }
    }

    public sealed class FIRXor : FIRBitwise
    {
        public FIRXor(Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base("^", aIn, bIn, outType, defNode) { }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            static void ExtendedBitwise(int length, Sink shorter, Span<BitState> shorterBits, Span<BitState> longerBits, Span<BitState> resultBits)
            {
                if (shorter.Value == null)
                {
                    throw new InvalidOperationException("Value not initialized.");
                }

                BitState shorterEnd = shorter.Value.IsSigned ? shorterBits[^1] : BitState.Zero;
                for (int i = length; i < longerBits.Length; i++)
                {
                    resultBits[i] = CompOpPropX(longerBits[i], shorterEnd, longerBits[i] ^ shorterEnd);
                }
            }

            int length = Math.Min(a.Bits.Length, b.Bits.Length);
            for (int i = 0; i < length; i++)
            {
                result.Bits[i] = CompOpPropX(a.Bits[i], b.Bits[i], a.Bits[i] ^ b.Bits[i]);
            }

            if (a.Bits.Length > b.Bits.Length)
            {
                ExtendedBitwise(length, B, b.Bits, a.Bits, result.Bits);
            }
            else if (a.Bits.Length < b.Bits.Length)
            {
                ExtendedBitwise(length, A, a.Bits, b.Bits, result.Bits);
            }

            result.IsValidBinary = a.IsValidBinary & b.IsValidBinary;
        }
    }

    public sealed class FIRShl : BiArgPrimOpCheckPropUnknown
    {
        private readonly int ShiftBy;
        public FIRShl(Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base("<<", aIn, bIn, outType, defNode)
        {
            if (bIn.Node == null)
            {
                throw new InvalidOperationException("Node not set.");
            }

            ShiftBy = (int)((ConstValue)bIn.Node).Value.Value;
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

        protected override IFIRType? BiArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(a.Width + ShiftBy),
            SIntType a => new SIntType(a.Width + ShiftBy),
            _ => null
        };
    }

    public sealed class FIRShr : BiArgPrimOpCheckPropUnknown
    {
        private readonly int ShiftBy;
        public FIRShr(Source aIn, Source bIn, IFIRType outType, IFirrtlNode defNode) : base(">>", aIn, bIn, outType, defNode)
        {
            if (bIn.Node == null)
            {
                throw new InvalidOperationException("Node not set.");
            }

            ShiftBy = (int)((ConstValue)bIn.Node).Value.Value;
        }

        protected override void BiArgCompute(ref BinaryVarValue a, ref BinaryVarValue b, ref BinaryVarValue result)
        {
            //Special logic for when shifting by more than the wire length.
            //In that case the result should be equal to zero if unsigned and
            //equal to sign bit if signed.
            if (ShiftBy >= a.Bits.Length)
            {
                if (A.Value == null)
                {
                    throw new InvalidOperationException("Value not initialized.");
                }

                if (!A.Value.IsSigned)
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

        protected override IFIRType? BiArgInferType() => A.Type switch
        {
            UIntType a => new UIntType(Math.Max(a.Width - ShiftBy, 1)),
            SIntType a => new SIntType(Math.Max(a.Width - ShiftBy, 1)),
            _ => null
        };
    }
}