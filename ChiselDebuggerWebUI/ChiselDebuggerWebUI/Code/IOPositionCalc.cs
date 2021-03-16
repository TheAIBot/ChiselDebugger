using ChiselDebug;
using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerWebUI.Code
{
    public class ScopedNodeIO
    {
        internal readonly List<ScopedDirIO> InputOffsets;
        internal readonly List<ScopedDirIO> OutputOffsets;
        internal int HeightNeeded { get; private set; }
        private int YStartPadding;
        private int YEndPadding;
        internal readonly List<IOScope> Scopes = new List<IOScope>();

        //Colors from https://www.w3schools.com/colors/colors_2021.asp
        private static readonly string[] PrettyColors = new string[]
        {
            "#FDAC53",
            "#9BB7D4",
            "#b55a30",
            "#F5DF4D",
            "#0072B5",
            "#A0DAA9",
            "#E9897E",
            "#00A170",
            "#926AA6",
            "#D2386C",
        };


        public ScopedNodeIO(List<ScopedDirIO> inputs, List<ScopedDirIO> outputs, int heightNeeded, int yStartPad, int yEndPad)
        {
            this.InputOffsets = inputs;
            this.OutputOffsets = outputs;
            this.HeightNeeded = heightNeeded;
            this.YStartPadding = yStartPad;
            this.YEndPadding = yEndPad;

            RemakeScopes();
        }

        private void RemakeScopes()
        {
            Scopes.Clear();
            Scopes.AddRange(MakeScopes(InputOffsets));
            Scopes.AddRange(MakeScopes(OutputOffsets));
        }

        private static List<IOScope> MakeScopes(List<ScopedDirIO> io)
        {
            List<IOScope> scopes = new List<IOScope>();

            var bundleGroups = io
                .Where(x => x.DirIO.IO.IsPartOfBundle)
                .GroupBy(x => x.DirIO.IO.Bundle);

            int colorIndex = 0;
            foreach (var bundleGroup in bundleGroups)
            {
                string color = PrettyColors[colorIndex++ % PrettyColors.Length];
                int xStart = bundleGroup.First().DirIO.Position.X + bundleGroup.First().ScopeXOffset;
                int yStart = bundleGroup.Min(x => x.DirIO.Position.Y);
                int yEnd = bundleGroup.Max(x => x.DirIO.Position.Y);

                scopes.Add(new IOScope(color, xStart, yStart, yEnd));
            }

            return scopes;
        }

        public List<DirectedIO> GetInputDirIO()
        {
            return InputOffsets.Select(x => x.DirIO).ToList();
        }

        public List<DirectedIO> GetOutputDirIO()
        {
            return OutputOffsets.Select(x => x.DirIO).ToList();
        }

        public void UpdateOutputX(int newX)
        {
            foreach (var offset in OutputOffsets)
            {
                offset.SetX(newX);
            }

            RemakeScopes();
        }

        public void UpdateIOX(int inputX, int outputX)
        {
            foreach (var offset in InputOffsets)
            {
                offset.SetX(inputX);
            }
            foreach (var offset in OutputOffsets)
            {
                offset.SetX(outputX);
            }

            RemakeScopes();
        }

        public float GetMaxScaling(int height, float maxAllowedScaling)
        {
            int heightNeededWithoutPad = HeightNeeded;
            int heightAvailWithoutPad = height;

            float maxScaling = (float)heightAvailWithoutPad / heightNeededWithoutPad;
            return Math.Min(maxAllowedScaling, maxScaling);
        }

        public float ScaleFillY(int height, float maxAllowedScaling)
        {
            float scaleFactor = GetMaxScaling(height, maxAllowedScaling);
            foreach (var offset in InputOffsets)
            {
                int currY = offset.DirIO.Position.Y;
                offset.SetY((int)(currY * scaleFactor));
            }
            foreach (var offset in OutputOffsets)
            {
                int currY = offset.DirIO.Position.Y;
                offset.SetY((int)(currY * scaleFactor));
            }

            HeightNeeded = (int)(HeightNeeded * scaleFactor);
            YStartPadding = (int)(YStartPadding * scaleFactor);
            YEndPadding = (int)(YEndPadding * scaleFactor);
            RemakeScopes();

            Debug.Assert(height >= HeightNeeded);
            return scaleFactor;
        }

        public void VerticalRecenter(int height)
        {
            if (height == HeightNeeded)
            {
                return;
            }

            Debug.Assert(height >= HeightNeeded);


            int heightNeededWithoutPad = HeightNeeded - YStartPadding - YEndPadding;
            int heightAvailWithoutPad = height - YStartPadding - YEndPadding;
            int remainingSpace = heightAvailWithoutPad - heightNeededWithoutPad;
            int startPad = remainingSpace / 2;

            foreach (var offset in InputOffsets)
            {
                int currY = offset.DirIO.Position.Y;
                offset.SetY(currY + startPad);
            }
            foreach (var offset in OutputOffsets)
            {
                int currY = offset.DirIO.Position.Y;
                offset.SetY(currY + startPad);
            }

            RemakeScopes();
        }

        public ScopedNodeIO Copy()
        {
            var inputCopies = InputOffsets.Select(x => x.Copy()).ToList();
            var outputCopies = OutputOffsets.Select(x => x.Copy()).ToList();

            return new ScopedNodeIO(inputCopies, outputCopies, HeightNeeded, YStartPadding, YEndPadding);
        }
    }
    public class ScopedDirIO
    {
        internal DirectedIO DirIO { get; private set; }
        internal readonly int ScopeXOffset;

        public ScopedDirIO(DirectedIO dirIO, int scopeXOffset)
        {
            this.DirIO = dirIO;
            this.ScopeXOffset = scopeXOffset;
        }

        public void SetX(int newX)
        {
            Point currPos = DirIO.Position;
            Point newPos = new Point(newX, DirIO.Position.Y);
            Point offset = newPos - currPos;

            DirIO = DirIO.WithOffsetPosition(offset);
        }

        public void SetY(int newY)
        {
            Point currPos = DirIO.Position;
            Point newPos = new Point(DirIO.Position.X, newY);
            Point offset = newPos - currPos;

            DirIO = DirIO.WithOffsetPosition(offset);
        }

        public ScopedDirIO Copy()
        {
            return new ScopedDirIO(DirIO.WithOffsetPosition(Point.Zero), ScopeXOffset);
        }
    }
    internal class IOScope
    {
        internal readonly string ScopeColor;
        internal readonly int XStart;
        internal readonly int YStart;
        internal readonly int Width = IOPositionCalc.ScopeWidth;
        internal readonly int Height;

        public IOScope(string color, int xStart, int yStart, int yEnd)
        {
            this.ScopeColor = color;
            this.XStart = xStart;
            this.YStart = yStart - IOPositionCalc.ScopeExtraY;
            this.Height = (yEnd + IOPositionCalc.ScopeExtraY) - YStart;
        }
    }

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

        internal static ScopedNodeIO VerticalScopedIO(FIRIO[] io, int fixedX, int startYPadding, int endYPadding)
        {
            List<ScopedDirIO> inputIO = new List<ScopedDirIO>();
            List<ScopedDirIO> outputIO = new List<ScopedDirIO>();

            int inputY = startYPadding;
            int outputY = inputY;

            MakeScopedIO(inputIO, outputIO, io, fixedX, ref inputY, ref outputY, 0);

            int heightNeeded = Math.Max(inputY, outputY) + endYPadding;
            return new ScopedNodeIO(inputIO, outputIO, heightNeeded, startYPadding, endYPadding);
        }

        private static void MakeScopedIO(List<ScopedDirIO> inputIO, List<ScopedDirIO> outputIO, FIRIO[] io, int fixedX, ref int inputYOffset, ref int outputYOffset, int scopeDepth)
        {
            inputYOffset = Math.Max(inputYOffset, outputYOffset);
            outputYOffset = Math.Max(inputYOffset, outputYOffset);

            for (int i = 0; i < io.Length; i++)
            {
                if (io[i] is IOBundle bundle)
                {
                    scopeDepth++;
                    MakeScopedIO(inputIO, outputIO, bundle.GetIOInOrder(), fixedX, ref inputYOffset, ref outputYOffset, scopeDepth);
                    scopeDepth--;

                    inputYOffset += ExtraSpaceBetweenBundles;
                    outputYOffset += ExtraSpaceBetweenBundles;
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

            inputYOffset = Math.Max(inputYOffset, outputYOffset);
            outputYOffset = Math.Max(inputYOffset, outputYOffset);
        }

        private static void MakeNoScopeIO(List<ScopedDirIO> inputIO, List<ScopedDirIO> outputIO, ScalarIO io, int fixedX, ref int inputYOffset, ref int outputYOffset, int scopeDepth)
        {
            if (io is Input)
            {
                int scopeOffset = Math.Max(0, scopeDepth - 1) * ScopeWidth;
                Point inputPos = new Point(0, inputYOffset);
                DirectedIO dirIO = new DirectedIO(io, inputPos, MoveDirs.Right);
                inputIO.Add(new ScopedDirIO(dirIO, scopeOffset));

                inputYOffset += MinSpaceBetweenIO;
            }
            else if (io is Output)
            {
                int scopeOffset = -scopeDepth * ScopeWidth;
                Point outputPos = new Point(fixedX, outputYOffset);
                DirectedIO dirIO = new DirectedIO(io, outputPos, MoveDirs.Right);
                outputIO.Add(new ScopedDirIO(dirIO, scopeOffset));

                outputYOffset += MinSpaceBetweenIO;
            }
        }
    }
}
