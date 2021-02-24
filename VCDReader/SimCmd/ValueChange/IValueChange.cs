using System;

namespace VCDReader
{
    public interface IValueChange : ISimCmd { }

    public class BinaryChange : IValueChange
    {
        public readonly BitState[] Bits;
        public readonly VarDef Variable;

        public BinaryChange(BitState[] bits, VarDef variable)
        {
            this.Bits = new BitState[variable.Size];
            this.Variable = variable;

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
    public record RealChange(double Value, VarDef Variable) : IValueChange;
}
