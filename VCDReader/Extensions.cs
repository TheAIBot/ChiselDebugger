﻿using System;
using System.Collections.Generic;
using System.Text;

namespace VCDReader
{
    public static class Extensions
    {
        public static char ToChar(this BitState bit)
        {
            ReadOnlySpan<byte> bitAsChar = new byte[] { (byte)'0', (byte)'1', (byte)'X', (byte)'Z' };
            return (char)bitAsChar[(int)bit & 0b11];
        }

        public static bool IsBinary(this BitState bit)
        {
            return (int)(bit & BitState.X) == 0;
        }

        public static bool IsAllBinary(this Span<BitState> bits)
        {
            for (int i = 0; i < bits.Length; i++)
            {
                if (!IsBinary(bits[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static string BitsToString(this BitState[] bits)
        {
            return bits.AsSpan().BitsToString();
        }

        public static string BitsToString(this Span<BitState> bits)
        {
            StringBuilder sBuilder = new StringBuilder(bits.Length);
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

        public static void CopyToCharArray(this ReadOnlySpan<byte> bytes, Span<char> chars)
        {
            if (bytes.Length > chars.Length)
            {
                throw new Exception("Failed to copy from byte array to char array as char array is not long enough.");
            }

            for (int i = 0; i < bytes.Length; i++)
            {
                chars[i] = (char)bytes[i];
            }
        }

        public static string ToCharString(this ReadOnlySpan<byte> bytes)
        {
            Span<char> chars = stackalloc char[bytes.Length];
            bytes.CopyToCharArray(chars);

            return chars.ToString();
        }
    }
}
