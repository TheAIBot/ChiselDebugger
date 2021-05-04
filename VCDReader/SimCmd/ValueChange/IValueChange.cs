using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace VCDReader
{
    public interface VarValue : ISimCmd
    {
        public List<VarDef>? Variables { get; }

        public bool SameValue(VarValue other);
    }

    public readonly struct BinaryVarValue : VarValue
    {
        private readonly Memory<BitState> BitSlice;
        private readonly List<VarDef>? Vars;

        public Span<BitState> Bits => BitSlice.Span;
        public List<VarDef>? Variables => Vars;

        public BinaryVarValue(Memory<BitState> bits, List<VarDef> variables)
        {
            this.BitSlice = bits;
            this.Vars = variables;
        }

        public BinaryVarValue(int bitCount)
        {
            this.BitSlice = new BitState[bitCount];
            this.Vars = null;
        }

        public string BitsToString()
        {
            return Bits.BitsToString();
        }

        public bool IsValidBinary()
        {
            ReadOnlySpan<BitState> rBits = Bits;
            ReadOnlySpan<ulong> uBits = MemoryMarshal.Cast<BitState, ulong>(rBits);
            ulong val = 0;
            for (int i = 0; i < uBits.Length; i++)
            {
                val |= uBits[i];
            }
            
            for (int i = uBits.Length * Marshal.SizeOf<ulong>(); i < Bits.Length; i++)
            {
                val |= (ulong)rBits[i];
            }

            //If is binary then only the first bit in each
            //byte should be set as BitState.Zero and Bitstate.One
            //only sets the first bit
            return (val & (~0x0101_0101_0101_0101ul)) == 0;
        }

        public bool SameValue(VarValue other)
        {
            return other is BinaryVarValue binary && SameValue(binary);
        }

        public bool SameValue(BinaryVarValue other)
        {
            ReadOnlySpan<BitState> rBits = Bits;
            ReadOnlySpan<BitState> rBitsOther = other.Bits;
            if (rBits.Length != rBitsOther.Length)
            {
                return false;
            }

            //xor of two equivalent values gives 0 and anything else
            //gives not 0. If check can be replaced with this check by checking
            //that all xors give 0 in return. That's how this check work. In
            //addition to that, poor simd is used to xor 8 BitStates at once.
            ulong xored = 0;
            int index = 0;
            if (rBits.Length > sizeof(ulong))
            {
                ReadOnlySpan<ulong> uBits = MemoryMarshal.Cast<BitState, ulong>(rBits);
                ReadOnlySpan<ulong> uBitsOther = MemoryMarshal.Cast<BitState, ulong>(rBitsOther);

                for (; index < uBits.Length; index++)
                {
                    xored |= uBits[index] ^ uBitsOther[index];
                }

                const int sizeDiff = sizeof(BitState) / sizeof(ulong);
                index *= sizeDiff;
            }

            for (; index < rBits.Length; index++)
            {
                xored |= (ulong)rBits[index] ^ (ulong)rBitsOther[index];
            }

            return xored == 0;
        }

        public bool SameValue(BinaryVarValue other, bool isSigned)
        {
            if (Bits.Length == other.Bits.Length)
            {
                return SameValue(other);
            }

            ReadOnlySpan<BitState> minL = Bits.Length < other.Bits.Length ? Bits : other.Bits;
            ReadOnlySpan<BitState> maxL = Bits.Length < other.Bits.Length ? other.Bits : Bits;
            if (minL.Length == 0)
            {
                for (int i = 0; i < maxL.Length; i++)
                {
                    if (maxL[i] != BitState.Zero)
                    {
                        return false;
                    }
                }

                return true;
            }

            BitState expectedRemainer = isSigned ? minL[^1] : BitState.Zero;
            for (int i = 0; i < minL.Length; i++)
            {
                if (minL[i] != maxL[i])
                {
                    return false;
                }
            }
            for (int i = minL.Length; i < maxL.Length; i++)
            {
                if (maxL[i] != expectedRemainer)
                {
                    return false;
                }
            }

            return true;
        }

        public void SetBitsAndExtend(BinaryVarValue value, bool asSigned)
        {
            if (Bits.Length <= value.Bits.Length)
            {
                value.Bits.Slice(0, Bits.Length).CopyTo(Bits);
            }
            else if (value.Bits.Length == 0)
            {
                Bits.Fill(BitState.Zero);
            }
            else
            {
                value.Bits.CopyTo(Bits);

                BitState extendWith = asSigned ? value.Bits[^1] : BitState.Zero;
                Bits.Slice(value.Bits.Length).Fill(extendWith);
            }
        }

        public void SetBits(ulong value)
        {
            Debug.Assert(Bits.Length <= 64);

            for (int i = 0; i < Bits.Length; i++)
            {
                ulong bitValue = value >> i;
                Bits[i] = (BitState)(1 & bitValue);
            }
        }

        public void SetBits(long value)
        {
            SetBits((ulong)value);
        }

        public void SetBitsAndExtend(BigInteger value, bool asSigned)
        {
            int valueBits = (int)value.GetBitLength();
            int minBits = Math.Min(valueBits, Bits.Length);
            for (int i = 0; i < minBits; i++)
            {
                BigInteger bitValue = value >> i;
                Bits[i] = (BitState)(int)(1 & bitValue);
            }

            if (valueBits < Bits.Length)
            {
                BitState sign = value.Sign == -1 ? BitState.One : BitState.Zero;
                Bits.Slice(valueBits, Bits.Length - valueBits).Fill(sign);
            }
        }

        public int AsInt()
        {
            Debug.Assert(Bits.Length <= 32);

            int value = 0;
            for (int i = 0; i < Bits.Length; i++)
            {
                int bitValue = 1 & (int)Bits[i];
                value |= bitValue << i;
            }

            return value;
        }

        public ulong AsULong()
        {
            Debug.Assert(Bits.Length <= 64);

            ulong value = 0;
            for (int i = 0; i < Bits.Length; i++)
            {
                ulong bitValue = 1 & (ulong)Bits[i];
                value |= bitValue << i;
            }

            return value;
        }

        public long AsLong()
        {
            Debug.Assert(Bits.Length <= 64);

            long value = Bits[^1] == BitState.One ? long.MaxValue : 0;
            for (int i = 0; i < Bits.Length; i++)
            {
                long bitValue = 1 & (long)Bits[i];
                value |= bitValue << i;
            }

            return value;
        }

        public BigInteger AsBigInteger(bool asSigned)
        {
            if (asSigned)
            {
                return AsSignedBigInteger();
            }
            else
            {
                return AsUnsignedBigInteger();
            }
        }

        public BigInteger AsUnsignedBigInteger()
        {
            var value = BigInteger.Zero;
            for (int i = Bits.Length - 1; i >= 0; i--)
            {
                value = value << 1;
                value |= 1 & (int)Bits[i];
            }

            return value;
        }

        public BigInteger AsSignedBigInteger()
        {
            var value = BigInteger.Zero;

            int sign = -(int)Bits[^1];
            value |= sign;

            for (int i = Bits.Length - 1; i >= 0; i--)
            {
                value = value << 1;
                value |= 1 & (int)Bits[i];
            }

            return value;
        }
    }
    public readonly struct RealVarValue: VarValue
    {
        public readonly double Value;
        private readonly List<VarDef>? Vars;
        public List<VarDef>? Variables => Vars;

        public RealVarValue(double value, List<VarDef> variables)
        {
            this.Value = value;
            this.Vars = variables;
        }        

        public bool SameValue(VarValue other)
        {
            return other is RealVarValue real &&
                   Value == real.Value;
        }
    }
}
