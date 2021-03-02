using System;

namespace VCDReader
{
    public abstract class VarValue : ISimCmd
    {
        public readonly VarDef Variable;

        public VarValue(VarDef variable)
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
            if (other is BinaryVarValue binary)
            {
                if (Bits.Length != binary.Bits.Length)
                {
                    return false;
                }

                for (int i = 0; i < Bits.Length; i++)
                {
                    if (Bits[i] != binary.Bits[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
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
