﻿using ChiselDebug;
using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code
{
    internal static class IOPositionCalc
    {
        private const int MaxSpaceBetweenIO = 40;
        private const int MinSpaceBetweenIO = 20;
        private const int ExtraSpaceBetweenBundles = 10;
        public const int ScopeExtraY = (MinSpaceBetweenIO + ExtraSpaceBetweenBundles) / 3;
        public const int ScopeWidth = 5;

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

        internal static ScopedNodeIO VerticalScopedIO(FIRIO[] io, int fixedX, int startYPadding, int endYPadding, bool ignoreDisconnectedIO = false)
        {
            List<ScopedDirIO> inputIO = new List<ScopedDirIO>();
            List<ScopedDirIO> outputIO = new List<ScopedDirIO>();

            int inputY = startYPadding;
            int outputY = inputY;

            MakeScopedIO(inputIO, outputIO, io, fixedX, ref inputY, ref outputY, 0, ignoreDisconnectedIO);

            int heightNeeded = Math.Max(inputY, outputY) + endYPadding;
            return new ScopedNodeIO(inputIO, outputIO, heightNeeded, startYPadding, endYPadding);
        }

        private static bool MakeScopedIO(List<ScopedDirIO> inputIO, List<ScopedDirIO> outputIO, FIRIO[] io, int fixedX, ref int inputYOffset, ref int outputYOffset, int scopeDepth, bool ignoreDisconnectedIO)
        {
            FIRIO[] allIO = io.SelectMany(x => x.Flatten()).ToArray();
            if (allIO.Any(x => x is Input) && allIO.Any(x => x is Output))
            {
                inputYOffset = Math.Max(inputYOffset, outputYOffset);
                outputYOffset = Math.Max(inputYOffset, outputYOffset);
            }

            bool addedIO = false;
            for (int i = 0; i < io.Length; i++)
            {
                if (io[i] is AggregateIO aggIO)
                {
                    int inScope = aggIO.IsPartOfAggregateIO ? 1 : 0;
                    bool addedAtleastOne = MakeScopedIO(inputIO, outputIO, aggIO.GetIOInOrder(), fixedX, ref inputYOffset, ref outputYOffset, scopeDepth + inScope, ignoreDisconnectedIO);

                    //Only add padding if aggregate added some io and if
                    //there might be more io to follow
                    if (addedAtleastOne && i + 1 != io.Length)
                    {
                        inputYOffset += ExtraSpaceBetweenBundles;
                        outputYOffset += ExtraSpaceBetweenBundles;
                    }
                }
                else if (io[i] is ScalarIO scalar)
                {
                    addedIO |= MakeNoScopeIO(inputIO, outputIO, scalar, fixedX, ref inputYOffset, ref outputYOffset, scopeDepth, ignoreDisconnectedIO);
                }
                else
                {
                    throw new Exception($"Can't make scoped io for io of type: {io[i].GetType()}");
                }
            }

            //inputYOffset = Math.Max(inputYOffset, outputYOffset);
            //outputYOffset = Math.Max(inputYOffset, outputYOffset);

            return addedIO;
        }

        private static bool MakeNoScopeIO(List<ScopedDirIO> inputIO, List<ScopedDirIO> outputIO, ScalarIO io, int fixedX, ref int inputYOffset, ref int outputYOffset, int scopeDepth, bool ignoreDisconnectedIO)
        {
            if (ignoreDisconnectedIO && !io.IsConnectedToAnything())
            {
                return false;
            }

            scopeDepth = Math.Max(0, scopeDepth);
            if (io is Input)
            {
                int scopeOffset = (scopeDepth + 1) * ScopeWidth;
                Point inputPos = new Point(0, inputYOffset);
                DirectedIO dirIO = new DirectedIO(io, inputPos, MoveDirs.Right);
                inputIO.Add(new ScopedDirIO(dirIO, scopeOffset));

                inputYOffset += MinSpaceBetweenIO;
                return true;
            }
            else if (io is Output)
            {
                int scopeOffset = -(scopeDepth + 1) * ScopeWidth;
                Point outputPos = new Point(fixedX, outputYOffset);
                DirectedIO dirIO = new DirectedIO(io, outputPos, MoveDirs.Right);
                outputIO.Add(new ScopedDirIO(dirIO, scopeOffset));

                outputYOffset += MinSpaceBetweenIO;
                return true;
            }
            else
            {
                throw new Exception($"Unknown scalar io type. Type: {io}");
            }
        }
    }
}