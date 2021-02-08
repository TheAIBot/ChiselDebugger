using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ChiselDebug
{
    public struct ValueType
    {
        BigInteger Value;
        SizedType ValueT;

        public ValueType(BigInteger value, SizedType type)
        {
            Value = value;
            ValueT = type;
        }

        public string ToBinaryString()
        {
            string binary = Value.ToBinaryString();
            binary = binary.Substring(0, Math.Min(binary.Length, ValueT.Width));

            if (ValueT.GType == GroundType.SInt)
            {
                return binary.PadLeft(ValueT.Width, binary.Last());
            }
            else
            {
                return binary.PadLeft(ValueT.Width, '0');
            }
        }
    }

    public static class Extensions
    {
        public static string ToBinaryString(this BigInteger value)
        {
            StringBuilder sBuilder = new StringBuilder();

            byte[] valueBytes = value.ToByteArray();
            foreach (var vByte in valueBytes)
            {
                sBuilder.Append(Convert.ToString(vByte, 2));
            }

            return sBuilder.ToString();
        }
    }
}
