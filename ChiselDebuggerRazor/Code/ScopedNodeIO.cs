using ChiselDebug;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ChiselDebuggerRazor.Code
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
            Scopes.AddRange(MakeScopes(InputOffsets, false));
            Scopes.AddRange(MakeScopes(OutputOffsets, true));
        }

        private static List<IOScope> MakeScopes(List<ScopedDirIO> io, bool isOutputIO)
        {
            List<IOScope> scopes = new List<IOScope>();

            Dictionary<AggregateIO, (Point start, int end)> bundleSizes = new Dictionary<AggregateIO, (Point start, int end)>();
            foreach (var scopedIO in io)
            {
                var zIO = scopedIO.DirIO.IO;
                int yStart = scopedIO.DirIO.Position.Y;
                int yEnd = scopedIO.DirIO.Position.Y;
                int xStart = scopedIO.DirIO.Position.X + scopedIO.ScopeXOffset - (isOutputIO ? 0 : IOPositionCalc.ScopeWidth);
                while (zIO.IsPartOfAggregateIO)
                {
                    if (!bundleSizes.TryAdd(zIO.ParentIO, (new Point(xStart, yStart), yEnd)))
                    {
                        var currScope = bundleSizes[zIO.ParentIO];
                        currScope.start.Y = Math.Min(currScope.start.Y, yStart);
                        currScope.end = Math.Max(currScope.end, yEnd);

                        bundleSizes[zIO.ParentIO] = currScope;
                    }

                    zIO = zIO.ParentIO;
                    yStart -= IOPositionCalc.ScopeExtraY / 2;
                    yEnd += IOPositionCalc.ScopeExtraY / 2;
                    xStart += IOPositionCalc.ScopeWidth * (isOutputIO ? 1 : -1);
                }
            }

            int colorIndex = 0;
            foreach (var scopeData in bundleSizes.Values)
            {
                string color = PrettyColors[colorIndex++ % PrettyColors.Length];

                scopes.Add(new IOScope(color, scopeData.start.X, scopeData.start.Y, scopeData.end));
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
}
