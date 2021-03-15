using ChiselDebug.GraphFIR.IO;
using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ChiselDebug.GraphFIR
{
    public static class Extensions
    {
        public static string ToSignedBinaryString(this BigInteger value, FIRRTL.GroundType type)
        {
            string binary = value.ToBinaryString();
            binary = binary.Substring(0, Math.Min(binary.Length, type.Width));

            if (type is FIRRTL.SIntType)
            {
                return binary.PadLeft(type.Width, binary.Last());
            }
            else if (type is FIRRTL.UIntType)
            {
                return binary.PadLeft(type.Width, '0');
            }
            else
            {
                throw new Exception();
            }
        }

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

        public static SizedType ToSizedType(this FIRRTL.Literal literal)
        {
            if (literal is FIRRTL.UIntLiteral)
            {
                return new SizedType(GroundType.UInt, literal.Width);
            }
            else if (literal is FIRRTL.SIntLiteral)
            {
                return new SizedType(GroundType.SInt, literal.Width);
            }
            else
            {
                throw new Exception($"Unexpected literal type: {literal}");
            }
        }

        public static ScalarIO[] Flatten(this FIRIO[] io)
        {
            return io.SelectMany(x => x.Flatten()).ToArray();
        }
    }
}
