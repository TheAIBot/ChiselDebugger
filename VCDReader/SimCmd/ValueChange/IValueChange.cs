using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace VCDReader
{
    public interface VarValue : ISimCmd
    {
        public List<VarDef>? Variables { get; }

        public bool SameValue(VarValue other);
    }

    public struct BinaryVarValue : VarValue
    {
        private readonly UnsafeMemory<BitState> BitSlice;
        private readonly List<VarDef>? Vars;
        public bool IsValidBinary;

        public Span<BitState> Bits => BitSlice.Span;
        public List<VarDef>? Variables => Vars;

        public BinaryVarValue(UnsafeMemory<BitState> bits, List<VarDef> variables, bool isValidBinary)
        {
            Debug.Assert(isValidBinary == bits.Span.IsAllBinary());
            this.BitSlice = bits;
            this.Vars = variables;
            this.IsValidBinary = isValidBinary;
        }

        public BinaryVarValue(int bitCount, bool isValidBinary)
        {
            this.BitSlice = new BitState[bitCount];
            this.Vars = null;
            this.IsValidBinary = isValidBinary;
        }

        public string BitsToString()
        {
            return Bits.BitsToString();
        }

        public void SetAllUnknown()
        {
            IsValidBinary = false;
            Bits.Fill(BitState.X);
        }

        public bool SameValue(VarValue other)
        {
            return other is BinaryVarValue binary && SameValue(ref binary);
        }

        public bool SameValue(ref BinaryVarValue other)
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
            if (rBits.Length >= sizeof(ulong))
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

        public bool SameValue(ref BinaryVarValue other, bool isSigned)
        {
            if (Bits.Length == other.Bits.Length)
            {
                return SameValue(ref other);
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

        public void SetBitsAndExtend(ref BinaryVarValue value, bool asSigned)
        {
            IsValidBinary = value.IsValidBinary;

            if (Bits.Length <= value.Bits.Length)
            {
                value.Bits.Slice(0, Bits.Length).CopyTo(Bits);
                
                //Has shorted it now so need to check if it's valid binary again
                if (!value.IsValidBinary)
                {
                    IsValidBinary = Bits.IsAllBinary();
                }
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
            IsValidBinary = true;

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
            IsValidBinary = true;

            int valueBits = (int)value.GetBitLength();
            int minBits = Math.Min(valueBits, Bits.Length);

            //Extract and vonert 32 bits from BigInteger to bits.
            //32bits are extracted at once instead of one bit at a time
            //because operations on BigInteger are expensive compared to
            //he same operations on an int.
            const int bitsPerInt = 32;

            //How many chunks of 32 bits should be extracted
            int intCount = (minBits + (bitsPerInt - 1)) / bitsPerInt;
            int remainingBits = minBits;
            for (int x = 0; x < intCount; x++)
            {
                //Shift to get right 32bit chunk
                BigInteger bitValue = value >> (x * bitsPerInt);
                uint intValue = (uint)(uint.MaxValue & bitValue);

                //There may be less than 32 bits left to do
                int doBits = Math.Min(remainingBits, bitsPerInt);
                remainingBits -= doBits;
                for (int i = 0; i < doBits; i++)
                {
                    Bits[x * bitsPerInt + i] = (BitState)(1 & (intValue >> i));
                }
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

        [SkipLocalsInit]
        public BigInteger AsUnsignedBigInteger()
        {
            const int ulongBitCount = 64;
            if (Bits.Length <= ulongBitCount)
            {
                ulong value = 0;
                for (int i = Bits.Length - 1; i >= 0; i--)
                {
                    value = value << 1;
                    value |= 1 & (ulong)Bits[i];
                }

                return new BigInteger(value);
            }
            else if (Bits.Length <= 1024)
            {
                const int bitsPerByte = 8;
                Span<byte> bytes = stackalloc byte[(Bits.Length + (bitsPerByte - 1)) / bitsPerByte];

                int wholeBytes = Bits.Length / bitsPerByte;
                for (int i = 0; i < wholeBytes; i++)
                {
                    int value = 0;
                    for (int x = bitsPerByte - 1; x >= 0; x--)
                    {
                        value <<= 1;
                        value |= 1 & (int)Bits[i * bitsPerByte + x];
                    }

                    bytes[i] = (byte)value;
                }

                if (Bits.Length % bitsPerByte != 0)
                {
                    int lastByte = 0;
                    for (int i = (Bits.Length % bitsPerByte) - 1; i >= 0; i--)
                    {
                        lastByte <<= 1;
                        lastByte |= 1 & (int)Bits[wholeBytes * bitsPerByte + i];
                    }
                    bytes[^1] = (byte)lastByte;
                }

                return new BigInteger(bytes, true);
            }
            else
            {
                var value = BigInteger.Zero;
                for (int i = Bits.Length - 1; i >= 0; i--)
                {
                    value = value << 1;
                    value |= 1 & (int)Bits[i];
                }

                return value;
            }
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
