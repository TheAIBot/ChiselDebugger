using System;
using System.Diagnostics;
using System.Numerics;

namespace VCDReader
{
    public abstract class VarValue : ISimCmd
    {
        public readonly VarDef? Variable;

        public VarValue(VarDef? variable)
        {
            this.Variable = variable;
        }

        public abstract bool SameValue(VarValue other);
    }

    public class BinaryVarValue : VarValue
    {
        public readonly BitState[] Bits;

        public BinaryVarValue(BitState[] bits, VarDef variable) : base(variable)
        {
            this.Bits = new BitState[variable.Size];

            Array.Fill(Bits, bits[^1].LeftExtendWith());
            bits.CopyTo(Bits, 0);
        }

        public BinaryVarValue(int bitCount) : base(null)
        {
            this.Bits = new BitState[bitCount];
        }

        public string BitsToString()
        {
            return Bits.BitsToString();
        }

        public bool IsValidBinary()
        {
            for (int i = 0; i < Bits.Length; i++)
            {
                if (!Bits[i].IsBinary())
                {
                    return false;
                }
            }

            return true;
        }

        public override bool SameValue(VarValue other)
        {
            return other is BinaryVarValue binary && SameValue(binary);
        }

        public bool SameValue(BinaryVarValue other)
        {
            if (Bits.Length != other.Bits.Length)
            {
                return false;
            }

            for (int i = 0; i < Bits.Length; i++)
            {
                if (Bits[i] != other.Bits[i])
                {
                    return false;
                }
            }

            return true;
        }

        public bool SameValueZeroExtend(BinaryVarValue other)
        {
            int minLength = Math.Min(Bits.Length, other.Bits.Length);
            for (int i = 0; i < minLength; i++)
            {
                if (Bits[i] != other.Bits[i])
                {
                    return false;
                }
            }

            if (Bits.Length > other.Bits.Length)
            {
                for (int i = minLength; i < Bits.Length; i++)
                {
                    if (Bits[i] != BitState.Zero)
                    {
                        return false;
                    }
                }
            }
            else if (Bits.Length < other.Bits.Length)
            {
                for (int i = minLength; i < other.Bits.Length; i++)
                {
                    if (other.Bits[i] != BitState.Zero)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void SetBitsAndExtend(BinaryVarValue value, bool asSigned)
        {
            Array.Copy(value.Bits, Bits, value.Bits.Length);
            ExtendBits(value.Bits.Length, asSigned);
        }

        private void ExtendBits(int lengthAlreadySet, bool asSigned)
        {
            if (lengthAlreadySet == Bits.Length)
            {
                return;
            }

            BitState extendWith = asSigned ? Bits[lengthAlreadySet - 1] : BitState.Zero;
            Array.Fill(Bits, extendWith, lengthAlreadySet, Bits.Length - lengthAlreadySet);
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
                Array.Fill(Bits, sign, valueBits, Bits.Length - valueBits);
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

            int sign = (int)Bits[^1];
            value |= sign;

            for (int i = Bits.Length - 1; i >= 0; i--)
            {
                value = value << 1;
                value |= 1 & (int)Bits[i];
            }

            return value;
        }
    }
    public class RealVarValue: VarValue
    {
        public readonly double Value;

        public RealVarValue(double value, VarDef variable) : base(variable)
        {
            this.Value = value;
        }

        public override bool SameValue(VarValue other)
        {
            return other is RealVarValue real &&
                   Value == real.Value;
        }
    }
}
