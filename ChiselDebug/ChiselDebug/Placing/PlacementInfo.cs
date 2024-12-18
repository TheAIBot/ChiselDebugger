using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Routing;
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

        public void AutoSpacePlacementRanks(Module mod,
                                            Dictionary<FIRRTLNode, DirectedIO[]> nodeInputOffsets,
                                            Dictionary<FIRRTLNode, DirectedIO[]> nodeOutputOffsets)
        {
            int xSpaceNeeded = AutoSpaceXAxis(mod);

            // Currently not being used because the nodes are not yet moved
            int ySpaceNeeded = AutoSpaceYAxis(mod, xSpaceNeeded, nodeInputOffsets, nodeOutputOffsets);
            if (ySpaceNeeded < SpaceNeeded.Y)
            {
                Console.WriteLine($"{SpaceNeeded.Y} -> {ySpaceNeeded}");
            }

            SpaceNeeded = new Point(xSpaceNeeded, Math.Max(SpaceNeeded.Y, ySpaceNeeded));
        }

        private int AutoSpaceXAxis(Module mod)
        {
            List<RankWidth> ranks = GetNodeRanks();

            int modOutputCount = SpaceForWire(mod.GetInternalSources());
            int modInputCount = SpaceForWire(mod.GetInternalSinks());

            int[] xSpacing = new int[ranks.Count + 1];

            const int spaceForWire = (int)(RouterBoard.CellSize * 1.2f);
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

                const int extraXDist = RouterBoard.CellSize * 2;
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

            return SpaceNeeded.X + xSpacing[^1];
        }

        private int AutoSpaceYAxis(Module mod,
                                   int moduleWidth,
                                   Dictionary<FIRRTLNode, DirectedIO[]> nodeInputOffsets,
                                   Dictionary<FIRRTLNode, DirectedIO[]> nodeOutputOffsets)
        {
            int maxX = NodePositions.Max(x => x.Position.X);
            int[] yHeightNeededPerXCell = new int[((maxX + (RouterBoard.CellSize - 1)) / RouterBoard.CellSize)];

            foreach (var rectangle in UsedSpace.Values)
            {
                int startIndex = Math.Max(0, (rectangle.LeftX / RouterBoard.CellSize) - 1);
                int endIndex = Math.Min(yHeightNeededPerXCell.Length - 1, (rectangle.RightX / RouterBoard.CellSize) + 1);
                for (int i = startIndex; i <= endIndex; i++)
                {
                    yHeightNeededPerXCell[i] += rectangle.Height;
                }
            }

            Dictionary<FIRIO, int> ioToXPosition = [];
            foreach (KeyValuePair<FIRRTLNode, DirectedIO[]> nodeInputOffset in nodeInputOffsets)
            {
                FIRRTLNode node = nodeInputOffset.Key;
                DirectedIO[] ios = nodeInputOffset.Value;

                int nodeStartX = node == mod ? 0 : UsedSpace[node].LeftX; // Not sure if offset is relative to left or right
                foreach (var ioPosition in ios)
                {
                    int ioXPosition = nodeStartX + ioPosition.Position.X;
                    ioToXPosition[ioPosition.IO] = ioXPosition;

                    foreach (var flat in ioPosition.IO.Flatten())
                    {
                        ioToXPosition[flat] = ioXPosition;
                    }

                    foreach (var flat in IOHelper.GetAllAggregateIOs(ioPosition.IO.Flatten()))
                    {
                        ioToXPosition[flat] = ioXPosition;
                    }
                }
            }
            foreach (KeyValuePair<FIRRTLNode, DirectedIO[]> nodeOutputOffset in nodeOutputOffsets)
            {
                FIRRTLNode node = nodeOutputOffset.Key;
                DirectedIO[] ios = nodeOutputOffset.Value;

                int nodeStartX = node == mod ? 0 : UsedSpace[node].LeftX; // Not sure if offset is relative to left or right
                foreach (var ioPosition in ios)
                {
                    int ioXPosition = nodeStartX + ioPosition.Position.X;
                    ioToXPosition[ioPosition.IO] = ioXPosition;

                    foreach (var flat in ioPosition.IO.Flatten())
                    {
                        ioToXPosition[flat] = ioXPosition;
                    }

                    foreach (var flat in IOHelper.GetAllAggregateIOs(ioPosition.IO.Flatten()))
                    {
                        ioToXPosition[flat] = ioXPosition;
                    }
                }
            }
            foreach (var internalModuleSource in mod.GetInternalSources())
            {
                ioToXPosition[internalModuleSource] = 0;

                foreach (var flat in IOHelper.GetAllAggregateIOs(internalModuleSource.Flatten()))
                {
                    ioToXPosition[flat] = 0;
                }

            }
            foreach (var internalModuleSink in mod.GetInternalSinks())
            {
                ioToXPosition[internalModuleSink] = moduleWidth;

                foreach (var flat in IOHelper.GetAllAggregateIOs(internalModuleSink.Flatten()))
                {
                    ioToXPosition[flat] = moduleWidth;
                }
            }


            foreach (KeyValuePair<FIRRTLNode, DirectedIO[]> nodeToOutputDirectedIO in nodeOutputOffsets)
            {
                DirectedIO[] ios = nodeToOutputDirectedIO.Value;

                foreach (var io in GetUniqueConnections(ios.SelectMany(x => x.IO.Flatten())))
                {
                    int ioStartX;
                    IEnumerable<FIRIO> connections;
                    // nodeOutputOffsets is either sources or aggregates
                    if (io is Source scalar)
                    {
                        if (!scalar.IsConnectedToAnythingPlaceable())
                        {
                            continue;
                        }
                        ioStartX = ioToXPosition[scalar];
                        connections = scalar.GetConnectedInputs();
                    }
                    else if (io is AggregateIO aggregateIO)
                    {
                        ioStartX = ioToXPosition[aggregateIO];
                        connections = aggregateIO.GetConnections().Select(x => x.To);
                    }
                    else
                    {
                        throw new InvalidOperationException("");
                    }

                    int startIndex = Math.Max(0, (ioStartX / RouterBoard.CellSize) - 1);
                    foreach (var sink in connections)
                    {
                        if (sink.IsAnonymous)
                        {
                            continue;
                        }

                        int ioEndX;
                        // Sinks can reside in nodes in other modules which has to be handled.
                        // Not entirely sure that is actually legal.
                        // I think perhaps only connections used for conditional connections
                        // will ignore mod boundaries but i will have to check.
                        if (!ioToXPosition.ContainsKey(sink))
                        {
                            // This one is wierd though. For some reason the
                            //if (sink.Node != null && nodeInputOffsets.ContainsKey(sink.Node))
                            //{
                            //    ioEndX = ioToXPosition[nodeInputOffsets[sink.Node][0].IO];
                            //}
                            //else 
                            if (sink.Node?.ResideIn != null && nodeInputOffsets.ContainsKey(sink.Node.ResideIn))
                            {
                                ioEndX = ioToXPosition[nodeInputOffsets[sink.Node.ResideIn][0].IO];
                            }
                            else
                            {
                                throw new InvalidOperationException("");
                            }
                        }
                        else
                        {
                            ioEndX = ioToXPosition[sink];
                        }
                        int endIndex = Math.Min(yHeightNeededPerXCell.Length - 1, (ioEndX / RouterBoard.CellSize) + 1);

                        for (int i = startIndex; i <= endIndex; i++)
                        {
                            yHeightNeededPerXCell[i] += RouterBoard.CellSize;
                        }
                    }

                }
            }

            // need to move nodes according to the amount of space required


            return yHeightNeededPerXCell.Max();
        }

        private static int SpaceForWire(IEnumerable<ScalarIO> io)
        {
            HashSet<AggregateIO> aggConnectionHasBeenHit = new HashSet<AggregateIO>();
            Dictionary<ScalarIO, AggregateIO> scalarToAggregateIO = GetScalarToAggregateIO(io);

            int spaceCounter = 0;
            foreach (var scalar in io)
            {
                if (scalarToAggregateIO.TryGetValue(scalar, out var aggIO))
                {
                    if (aggConnectionHasBeenHit.Add(aggIO))
                    {
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

        private static IEnumerable<FIRIO> GetUniqueConnections(IEnumerable<ScalarIO> io)
        {
            HashSet<AggregateIO> aggConnectionHasBeenHit = new HashSet<AggregateIO>();
            Dictionary<ScalarIO, AggregateIO> scalarToAggregateIO = GetScalarToAggregateIO(io);

            foreach (var scalar in io)
            {
                if (scalarToAggregateIO.TryGetValue(scalar, out var aggIO))
                {
                    if (aggConnectionHasBeenHit.Add(aggIO))
                    {
                        yield return aggIO;
                    }
                }
                else
                {
                    if (scalar.IsConnectedToAnythingPlaceable())
                    {
                        yield return scalar;
                    }
                }
            }
        }

        private static Dictionary<ScalarIO, AggregateIO> GetScalarToAggregateIO(IEnumerable<ScalarIO> io)
        {
            Dictionary<ScalarIO, AggregateIO> scalarToAggregateIO = new Dictionary<ScalarIO, AggregateIO>();
            foreach (var aggIO in IOHelper.GetAllAggregateIOs(io).OrderByDescending(x => x.GetScalarsCount()))
            {
                if (aggIO.IsPassive() && aggIO.OnlyConnectedWithAggregateConnections())
                {
                    foreach (var scalar in aggIO.Flatten())
                    {
                        scalarToAggregateIO.TryAdd(scalar, aggIO);
                    }
                }
            }

            return scalarToAggregateIO;
        }

        public void SetBorderPadding(Point padding)
        {
            NodePositions.Clear();
            foreach (var nodeRect in UsedSpace.ToArray())
            {
                NodePositions.Add(new Positioned<FIRRTLNode>(nodeRect.Value.Pos + padding, nodeRect.Key));
                UsedSpace[nodeRect.Key] = new Rectangle(nodeRect.Value.Pos + padding, nodeRect.Value.Size);
            }

            SpaceNeeded = SpaceNeeded + padding * 2;
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
