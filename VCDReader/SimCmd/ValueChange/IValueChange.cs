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
    }
    public class RealVarValue: VarValue
    {
        public readonly double Value;

        public RealVarValue(double value, VarDef variable) : base(variable)
        {
            this.Value = value;
        }
    }
}
