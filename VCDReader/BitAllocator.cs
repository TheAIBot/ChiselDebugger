using System;

namespace VCDReader
{
    internal class BitAllocator
    {
        private const int BitsPerAllocate = 100_000;
        private Memory<BitState> Bits = new BitState[BitsPerAllocate];

        internal Memory<BitState> GetBits(int length)
        {
            if (length > BitsPerAllocate)
            {
                throw new Exception($"Currently only supports bit array of up to {BitsPerAllocate:N0} bits.");
            }

            if (Bits.Length < length)
            {
                Bits = new BitState[BitsPerAllocate];
            }

            Memory<BitState> usedBits = Bits.Slice(0, length);
            Bits = Bits.Slice(length);

            return usedBits;
        }
    }
}
