using System.Text;

namespace VCDReader
{
    public static class Extensions
    {
        private static readonly BitState[] LeftExtend = new BitState[] { BitState.Zero, BitState.Zero, BitState.X, BitState.Z };
        private static readonly char[] BitAsChar = new char[] { '0', '1', 'X', 'Z' };

        public static BitState LeftExtendWith(this BitState bit)
        {
            return LeftExtend[(int)bit];
        }

        public static char ToChar(this BitState bit)
        {
            return BitAsChar[(int)bit];
        }

        public static bool IsBinary(this BitState bit)
        {
            return bit == BitState.Zero || bit == BitState.One;
        }

        public static string BitsToString(this BitState[] bits)
        {
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < bits.Length; i++)
            {
                sBuilder.Append(bits[i].ToChar());
            }

            return sBuilder.ToString();
        }
    }
}
