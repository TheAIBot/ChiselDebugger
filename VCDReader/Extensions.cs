﻿using System;
using System.Collections.Generic;
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
            return bits.AsSpan().BitsToString();
        }

        public static string BitsToString(this Span<BitState> bits)
        {
            StringBuilder sBuilder = new StringBuilder();
            for (int i = bits.Length - 1; i >= 0; i--)
            {
                sBuilder.Append(bits[i].ToChar());
            }

            return sBuilder.ToString();
        }

        public static ISimCmd[] ToArray(this IEnumerable<SimPass> iter)
        {
            List<ISimCmd> cmds = new List<ISimCmd>();
            foreach (var cmd in iter)
            {
                cmds.Add(cmd.GetCmd());
            }

            return cmds.ToArray();
        }
    }
}
