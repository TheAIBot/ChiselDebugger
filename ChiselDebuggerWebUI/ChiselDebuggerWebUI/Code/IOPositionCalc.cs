using ChiselDebug;
using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerWebUI.Code
{
    internal class ScopedNodeIO
    {
        internal readonly List<ScopedDirIO> InputOffsets;
        internal readonly List<ScopedDirIO> OutputOffsets;
        internal readonly int HeightNeeded;

        public ScopedNodeIO(List<ScopedDirIO> inputs, List<ScopedDirIO> outputs, int heightNeeded)
        {
            this.InputOffsets = inputs;
            this.OutputOffsets = outputs;
            this.HeightNeeded = heightNeeded;
        }
    }
    public class ScopedDirIO
    {
        internal readonly DirectedIO DirIO;
        internal readonly int ScopeDepth;

        public ScopedDirIO(DirectedIO dirIO, int scopeDepth)
        {
            this.DirIO = dirIO;
            this.ScopeDepth = scopeDepth;
        }
    }

    internal static class IOPositionCalc
    {
        private const int MaxSpaceBetweenIO = 40;
        private const int MinSpaceBetweenIO = 20;
        private const int ExtraSpaceBetweenBundles = 10;

        internal static List<DirectedIO> EvenVertical(int height, ScalarIO[] io, int fixedX, int startY)
        {
            int usableHeight = height - startY * 2;
            int spaceBetweenIO;
            if (io.Length <= 1)
            {
                startY += usableHeight / 2;
                spaceBetweenIO = 0;
            }
            else
            {
                int spacersNeeded = io.Length - 1;
                int possibleSpaceBetweenIO = usableHeight / spacersNeeded;
                spaceBetweenIO = Math.Min(MaxSpaceBetweenIO, possibleSpaceBetweenIO);
                int usedSpace = spaceBetweenIO * spacersNeeded;

                startY += (usableHeight - usedSpace) / 2;
            }

            List<DirectedIO> posIO = new List<DirectedIO>();
            for (int i = 0; i < io.Length; i++)
            {
                int y = startY + spaceBetweenIO * i;

                Point pos = new Point(fixedX, y);
                posIO.Add(new DirectedIO(io[i], pos, MoveDirs.Right));
            }

            return posIO;
        }

        internal static ScopedNodeIO VerticalScopedIO(FIRIO[] io, int fixedX, int startYPadding, int endYPadding)
        {
            List<ScopedDirIO> inputIO = new List<ScopedDirIO>();
            List<ScopedDirIO> outputIO = new List<ScopedDirIO>();

            int inputY = startYPadding;
            int outputY = inputY;

            MakeScopedIO(inputIO, outputIO, io, fixedX, ref startYPadding, ref outputY, 0);

            int heightNeeded = Math.Max(inputY, outputY) + endYPadding;
            return new ScopedNodeIO(inputIO, outputIO, heightNeeded);
        }

        private static void MakeScopedIO(List<ScopedDirIO> inputIO, List<ScopedDirIO> outputIO, FIRIO[] io, int fixedX, ref int inputYOffset, ref int outputYOffset, int scopeDepth)
        {
            outputYOffset = inputYOffset;

            for (int i = 0; i < io.Length; i++)
            {
                if (io[i] is IOBundle bundle)
                {
                    scopeDepth++;
                    MakeScopedIO(inputIO, outputIO, bundle.GetIOInOrder(), fixedX, ref inputYOffset, ref outputYOffset, scopeDepth);
                    scopeDepth--;

                    inputYOffset += ExtraSpaceBetweenBundles;
                    outputYOffset = inputYOffset;
                }
                else if (io[i] is ScalarIO scalar)
                {
                    MakeNoScopeIO(inputIO, outputIO, scalar, fixedX, ref inputYOffset, ref outputYOffset, scopeDepth);
                }
                else
                {
                    throw new Exception($"Can't make scoped io for io of type: {io[i].GetType()}");
                }
            }
        }

        private static void MakeNoScopeIO(List<ScopedDirIO> inputIO, List<ScopedDirIO> outputIO, ScalarIO io, int fixedX, ref int inputYOffset, ref int outputYOffset, int scopeDepth)
        {
            if (io is Input)
            {
                Point inputPos = new Point(0, inputYOffset);
                DirectedIO dirIO = new DirectedIO(io, inputPos, MoveDirs.Right);
                inputIO.Add(new ScopedDirIO(dirIO, scopeDepth));

                inputYOffset += MinSpaceBetweenIO;
            }
            else if (io is Output)
            {
                Point outputPos = new Point(fixedX, inputYOffset);
                DirectedIO dirIO = new DirectedIO(io, outputPos, MoveDirs.Right);
                outputIO.Add(new ScopedDirIO(dirIO, scopeDepth));

                outputYOffset += MinSpaceBetweenIO;
            }
        }
    }
}
