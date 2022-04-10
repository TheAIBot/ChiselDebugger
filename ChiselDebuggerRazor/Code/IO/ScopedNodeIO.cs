using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Routing;
using ChiselDebug.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ChiselDebuggerRazor.Code.IO
{
    public sealed class ScopedNodeIO
    {
        internal readonly List<ScopedDirIO> SinkOffsets;
        internal readonly List<ScopedDirIO> SourceOffsets;
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


        public ScopedNodeIO(List<ScopedDirIO> sinks, List<ScopedDirIO> sources, int yStartPad, int yEndPad)
        {
            SinkOffsets = sinks;
            SourceOffsets = sources;
            YStartPadding = yStartPad;
            YEndPadding = yEndPad;

            int inputHeight = sinks.Count > 0 ? sinks.Max(x => x.DirIO.Position.Y) : 0;
            int outputHeight = sources.Count > 0 ? sources.Max(x => x.DirIO.Position.Y) : 0;
            HeightNeeded = Math.Max(inputHeight, outputHeight) + yEndPad;

            RemakeScopes();
        }

        private void RemakeScopes()
        {
            Scopes.Clear();
            Scopes.AddRange(MakeScopes(SinkOffsets, false));
            Scopes.AddRange(MakeScopes(SourceOffsets, true));
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

        public DirectedIO[] GetSinkDirIO()
        {
            return SinkOffsets.Select(x => x.DirIO).ToArray();
        }

        public DirectedIO[] GetSourceDirIO()
        {
            return SourceOffsets.Select(x => x.DirIO).ToArray();
        }

        public void UpdateOutputX(int newX)
        {
            foreach (var offset in SourceOffsets)
            {
                offset.SetX(newX);
            }

            RemakeScopes();
        }

        public void UpdateIOX(int inputX, int outputX)
        {
            foreach (var offset in SinkOffsets)
            {
                offset.SetX(inputX);
            }
            foreach (var offset in SourceOffsets)
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
            foreach (var offset in SinkOffsets)
            {
                int currY = offset.DirIO.Position.Y;
                offset.SetY((int)(currY * scaleFactor));
            }
            foreach (var offset in SourceOffsets)
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

            foreach (var offset in SinkOffsets)
            {
                int currY = offset.DirIO.Position.Y;
                offset.SetY(currY + startPad);
            }
            foreach (var offset in SourceOffsets)
            {
                int currY = offset.DirIO.Position.Y;
                offset.SetY(currY + startPad);
            }

            RemakeScopes();
        }
    }
}
