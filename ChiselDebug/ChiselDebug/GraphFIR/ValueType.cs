using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ChiselDebug.GraphFIR
{
    public struct ValueType
    {
        string ValueString;
        SizedType ValueT;

        public ValueType(string valueString, SizedType type)
        {
            ValueString = valueString;
            ValueT = type;
        }

        public string ToBinaryString()
        {
            return ValueString;
            //string binary = Value.ToBinaryString();
            //binary = binary.Substring(0, Math.Min(binary.Length, ValueT.Width));

            //if (ValueT.GType == GroundType.SInt)
            //{
            //    return binary.PadLeft(ValueT.Width, binary.Last());
            //}
            //else
            //{
            //    return binary.PadLeft(ValueT.Width, '0');
            //}
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
