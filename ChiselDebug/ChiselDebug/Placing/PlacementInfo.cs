﻿using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.Placing
{
    public sealed class PlacementInfo
    {
        public readonly List<Positioned<FIRRTLNode>> NodePositions;
        public readonly Dictionary<FIRRTLNode, Rectangle> UsedSpace;
        public Point SpaceNeeded { get; private set; }

        public PlacementInfo() : this(new List<Positioned<FIRRTLNode>>(), new Dictionary<FIRRTLNode, Rectangle>(), Point.Zero)
        { }

        public PlacementInfo(List<Positioned<FIRRTLNode>> nodePoses, Dictionary<FIRRTLNode, Rectangle> usedSpace, Point spaceNeeded)
        {
            NodePositions = nodePoses;
            UsedSpace = usedSpace;
            SpaceNeeded = spaceNeeded;
        }

        public void AddNodePlacement(FIRRTLNode node, Rectangle shape)
        {
            NodePositions.Add(new Positioned<FIRRTLNode>(shape.Pos, node));
            SpaceNeeded = Point.Max(SpaceNeeded, new Point(shape.RightX, shape.BottomY));

            UsedSpace.Add(node, shape);
        }

        public void SetSpaceNeeded(Point spaceNeeded)
        {
            SpaceNeeded = spaceNeeded;
        }

        public void AutoSpacePlacementRanks(Module mod)
        {
            List<RankWidth> ranks = GetNodeRanks();

            int modOutputCount = SpaceForWire(mod.GetInternalSources());
            int modInputCount = SpaceForWire(mod.GetInternalSinks());

            int[] xSpacing = new int[ranks.Count + 1];

            const int spaceForWire = 12;
            xSpacing[0] += modOutputCount * spaceForWire;
            xSpacing[^1] += modInputCount * spaceForWire;

            for (int i = 0; i < ranks.Count; i++)
            {
                xSpacing[i] += ranks[i].Nodes.Sum(x => SpaceForWire(x.GetSinks())) * spaceForWire;
                xSpacing[i + 1] += ranks[i].Nodes.Sum(x => SpaceForWire(x.GetSources())) * spaceForWire;
            }

            int xOffset = xSpacing[0];
            int prevRankMaxX = 0;
            int[] xOffsets = new int[ranks.Count];
            for (int i = 0; i < ranks.Count; i++)
            {
                int widest = ranks[i].GetWidth();
                int currRankDist = ranks[i].StartX - prevRankMaxX - 1;
                int wantedRankDist = xSpacing[i];
                int rankDistOffset = wantedRankDist - currRankDist;

                xOffsets[i] = xOffset + rankDistOffset;

                const int extraXDist = 20;
                xOffset += rankDistOffset + extraXDist;
                prevRankMaxX = ranks[i].EndX;
            }

            SpaceNeeded = Point.Zero;
            NodePositions.Clear();
            for (int i = 0; i < ranks.Count; i++)
            {
                foreach (var node in ranks[i].Nodes)
                {
                    Rectangle oldNodeRect = UsedSpace[node];
                    Point newPos = new Point(oldNodeRect.LeftX + xOffsets[i], oldNodeRect.TopY);
                    Rectangle newRect = new Rectangle(newPos, oldNodeRect.Size);

                    NodePositions.Add(new Positioned<FIRRTLNode>(newPos, node));
                    UsedSpace[node] = newRect;
                    SpaceNeeded = Point.Max(SpaceNeeded, newRect.Pos + newRect.Size);
                }
            }

            SpaceNeeded = new Point(SpaceNeeded.X + xSpacing[^1], SpaceNeeded.Y);
        }

        private static int SpaceForWire(IEnumerable<ScalarIO> io)
        {
            Dictionary<AggregateIO, bool> aggConnectionHasBeenHit = new Dictionary<AggregateIO, bool>();
            Dictionary<ScalarIO, AggregateIO> scalarToAggregateIO = new Dictionary<ScalarIO, AggregateIO>();
            foreach (var aggIO in IOHelper.GetAllAggregateIOs(io).OrderByDescending(x => x.GetScalarsCount()))
            {
                if (aggIO.IsPassive() && aggIO.OnlyConnectedWithAggregateConnections())
                {
                    aggConnectionHasBeenHit.Add(aggIO, false);
                    foreach (var scalar in aggIO.Flatten())
                    {
                        scalarToAggregateIO.TryAdd(scalar, aggIO);
                    }
                }
            }

            int spaceCounter = 0;
            foreach (var scalar in io)
            {
                if (scalarToAggregateIO.TryGetValue(scalar, out var aggIO))
                {
                    if (!aggConnectionHasBeenHit[aggIO])
                    {
                        aggConnectionHasBeenHit[aggIO] = true;
                        spaceCounter++;
                    }
                }
                else
                {
                    spaceCounter += scalar.IsConnectedToAnythingPlaceable() ? 1 : 0;
                }
            }

            return spaceCounter;
        }

        public void SetBorderPadding(Point padding)
        {
            Point minPos = new Point(int.MaxValue, int.MaxValue);
            Point maxPos = new Point(int.MinValue, int.MinValue);
            foreach (var nodeRect in UsedSpace.Values)
            {
                minPos = Point.Min(minPos, nodeRect.Pos);
                maxPos = Point.Max(maxPos, nodeRect.Pos + nodeRect.Size);
            }

            Point offset = padding - minPos;
            offset.X = 0;

            NodePositions.Clear();
            foreach (var nodeRect in UsedSpace.ToArray())
            {
                NodePositions.Add(new Positioned<FIRRTLNode>(nodeRect.Value.Pos + offset, nodeRect.Key));
                UsedSpace[nodeRect.Key] = new Rectangle(nodeRect.Value.Pos + offset, nodeRect.Value.Size);
            }

            SpaceNeeded = new Point(SpaceNeeded.X, maxPos.Y + offset.Y + padding.Y * 2);
        }

        private sealed class RankWidth
        {
            public int StartX { get; private set; }
            public int EndX { get; private set; }
            public readonly List<FIRRTLNode> Nodes = new List<FIRRTLNode>();

            public RankWidth(FIRRTLNode node, Rectangle nodeRect)
            {
                StartX = nodeRect.LeftX;
                EndX = nodeRect.RightX;
                Nodes.Add(node);
            }

            public bool IsInSameRank(Rectangle rect)
            {
                return StartX <= rect.LeftX && rect.LeftX <= EndX ||
                       StartX <= rect.RightX && rect.RightX <= EndX ||
                       rect.LeftX <= StartX && StartX <= rect.RightX ||
                       rect.LeftX <= EndX && EndX <= rect.RightX;
            }

            public bool IsBeforeThisRank(Rectangle rect)
            {
                return rect.RightX < StartX;
            }

            public void AddNode(FIRRTLNode node, Rectangle nodeRect)
            {
                StartX = Math.Min(StartX, nodeRect.LeftX);
                EndX = Math.Max(EndX, nodeRect.RightX);

                Nodes.Add(node);
            }

            public int GetWidth()
            {
                return EndX - StartX + 1;
            }
        }

        private List<RankWidth> GetNodeRanks()
        {
            List<RankWidth> nodeRanks = new List<RankWidth>();

            foreach (var firNodeAndRect in UsedSpace)
            {
                bool addedNode = false;

                for (int i = 0; i < nodeRanks.Count; i++)
                {
                    var rank = nodeRanks[i];
                    if (rank.IsBeforeThisRank(firNodeAndRect.Value))
                    {
                        nodeRanks.Insert(i, new RankWidth(firNodeAndRect.Key, firNodeAndRect.Value));
                        addedNode = true;
                        break;
                    }
                    else if (rank.IsInSameRank(firNodeAndRect.Value))
                    {
                        rank.AddNode(firNodeAndRect.Key, firNodeAndRect.Value);
                        addedNode = true;
                        break;
                    }
                }

                if (!addedNode)
                {
                    nodeRanks.Add(new RankWidth(firNodeAndRect.Key, firNodeAndRect.Value));
                }
            }

            return nodeRanks;
        }
    }
}
