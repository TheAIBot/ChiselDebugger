using ChiselDebug.GraphFIR;
using System;
using System.Collections.Generic;
using System.Text;
using ChiselDebug.Graphing;
using System.Linq;
using System.Diagnostics;
using ChiselDebug.GraphFIR.IO;

namespace ChiselDebug
{
    public class SimplePlacer
    {
        private readonly Module Mod;
        private readonly HashSet<FIRRTLNode> MissingNodeDims;
        private readonly Dictionary<FIRRTLNode, Point> SizeChanges = new Dictionary<FIRRTLNode, Point>();
        private readonly Dictionary<FIRRTLNode, Point> NodeSizes = new Dictionary<FIRRTLNode, Point>();

        public SimplePlacer(Module mod)
        {
            this.Mod = mod;
            this.MissingNodeDims = new HashSet<FIRRTLNode>(Mod.GetAllNodes());
            MissingNodeDims.RemoveWhere(x => x is INoPlaceAndRoute);
        }

        public PlacementInfo PositionModuleComponents()
        {
            lock (NodeSizes)
            {
                lock (SizeChanges)
                {
                    foreach (var change in SizeChanges)
                    {
                        NodeSizes[change.Key] = change.Value;
                    }

                    //Changes transferred, now there are no more changed left
                    SizeChanges.Clear();
                }

                try
                {
                    PlacementInfo placments = new PlacementInfo();

                    Graph<FIRRTLNode> graph = new Graph<FIRRTLNode>();

                    //Add nodes to graph
                    Dictionary<FIRRTLNode, Node<FIRRTLNode>> firNodeToNode = new Dictionary<FIRRTLNode, Node<FIRRTLNode>>();
                    foreach (var firNode in NodeSizes.Keys)
                    {
                        var node = new Node<FIRRTLNode>(firNode);
                        graph.AddNode(node);
                        firNodeToNode.Add(firNode, node);
                    }

                    //Relate io to FIRRTLNode
                    Dictionary<ScalarIO, FIRRTLNode> inputToFirNode = new Dictionary<ScalarIO, FIRRTLNode>();
                    Dictionary<ScalarIO, FIRRTLNode> outputToFirNode = new Dictionary<ScalarIO, FIRRTLNode>();
                    foreach (var firNode in NodeSizes.Keys)
                    {
                        if (firNode == Mod)
                        {
                            continue;
                        }
                        foreach (var input in firNode.GetInputs())
                        {
                            inputToFirNode.Add(input, firNode);
                        }
                        foreach (var output in firNode.GetOutputs())
                        {
                            outputToFirNode.Add(output, firNode);
                        }
                    }

                    Dictionary<ScalarIO, Node<FIRRTLNode>> inputToNode = new Dictionary<ScalarIO, Node<FIRRTLNode>>();
                    Dictionary<ScalarIO, Node<FIRRTLNode>> outputToNode = new Dictionary<ScalarIO, Node<FIRRTLNode>>();
                    foreach (var keyValue in inputToFirNode)
                    {
                        inputToNode.Add(keyValue.Key, firNodeToNode[keyValue.Value]);
                    }
                    foreach (var keyValue in outputToFirNode)
                    {
                        outputToNode.Add(keyValue.Key, firNodeToNode[keyValue.Value]);
                    }

                    List<Node<FIRRTLNode>> modInputNodes = new List<Node<FIRRTLNode>>();
                    foreach (var input in Mod.GetInternalInputs())
                    {
                        if (!input.IsConnectedToAnything())
                        {
                            continue;
                        }
                        Node<FIRRTLNode> modInputNode = new Node<FIRRTLNode>(Mod);
                        graph.AddNode(modInputNode);
                        inputToNode.Add(input, modInputNode);
                        modInputNodes.Add(modInputNode);
                    }
                    List<Node<FIRRTLNode>> modOutputNodes = new List<Node<FIRRTLNode>>();
                    foreach (var output in Mod.GetInternalOutputs())
                    {
                        if (!output.IsConnectedToAnything())
                        {
                            continue;
                        }
                        Node<FIRRTLNode> modOutputNode = new Node<FIRRTLNode>(Mod);
                        graph.AddNode(modOutputNode);
                        outputToNode.Add(output, modOutputNode);
                        modOutputNodes.Add(modOutputNode);
                    }


                    //Make edges
                    foreach (Output output in outputToNode.Keys)
                    {
                        if (!output.IsConnectedToAnything())
                        {
                            continue;
                        }
                        var from = outputToNode[output];
                        foreach (var input in output.GetConnectedInputs())
                        {
                            if (input.Node != null && input.Node is INoPlaceAndRoute)
                            {
                                continue;
                            }

                            if (!inputToNode.ContainsKey(input))
                            {
                                continue;
                            }
                            var to = inputToNode[input];
                            graph.AddEdge(from, to);
                        }
                    }
                    graph.MakeIndirectConnections();

                    var placement = GetPlacements(graph, modInputNodes, modOutputNodes);
                    foreach (var modIONode in modInputNodes)
                    {
                        placement.Remove(modIONode);
                    }
                    foreach (var modIONode in modOutputNodes)
                    {
                        placement.Remove(modIONode);
                    }


                    Point min = new Point(int.MaxValue, int.MaxValue);
                    Point max = new Point(int.MinValue, int.MinValue);
                    foreach (var keyVal in placement)
                    {
                        min = Point.Min(min, keyVal.Value);
                        max = Point.Max(max, keyVal.Value);
                    }

                    int columns = max.X - min.X + 1;
                    int rows = max.Y - min.Y + 1;

                    FIRRTLNode[][] nodePlacements = new FIRRTLNode[columns][];
                    for (int i = 0; i < nodePlacements.Length; i++)
                    {
                        nodePlacements[i] = new FIRRTLNode[rows];
                    }

                    foreach (var keyVal in placement)
                    {
                        Point pos = keyVal.Value - min;
                        nodePlacements[pos.X][pos.Y] = keyVal.Key.Value;
                    }

                    var xGroups = placement.GroupBy(x => x.Value.X).OrderBy(x => x.Key).ToArray();
                    var yGroups = placement.GroupBy(x => x.Value.Y).OrderBy(x => x.Key).ToArray();
                    Point borderPadding = new Point(200, 200);

                    int[] xOffsets = MakeXOffsets(xGroups, min, borderPadding, columns);
                    int[] yOffsets = MakeYOffsets(yGroups, min, borderPadding, rows);

                    for (int x = 0; x < columns; x++)
                    {
                        for (int y = 0; y < rows; y++)
                        {
                            FIRRTLNode node = nodePlacements[x][y];
                            if (node == null)
                            {
                                continue;
                            }

                            Point pos = new Point(xOffsets[x], yOffsets[y]);

                            placments.AddNodePlacement(node, new Rectangle(pos, NodeSizes[node]));
                        }
                    }


                    placments.AddEndStuff(borderPadding);
                    return placments;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    Debug.WriteLine(e.StackTrace);
                    throw;
                }
            }
        }

        private int[] MakeXOffsets(IGrouping<int, KeyValuePair<Node<FIRRTLNode>, Point>>[] xGroups, Point minPos, Point borderPadding, int columns)
        {
            int modOutputCount = Mod.GetInternalOutputs().Sum(x => x.IsConnectedToAnything() ? 1 : 0);
            int modInputCount = Mod.GetInternalInputs().Sum(x => x.IsConnectedToAnything() ? 1 : 0);

            int[] xSpacing = new int[xGroups.Length + 1];

            const int spaceForWire = 12;
            xSpacing[0] += modOutputCount * spaceForWire;
            xSpacing[^1] += modInputCount * spaceForWire;

            for (int i = 0; i < xGroups.Length; i++)
            {
                xSpacing[i] += xGroups[i].Sum(x => x.Key.Incomming.Count) * spaceForWire;
                xSpacing[i + 1] += xGroups[i].Sum(x => x.Key.Outgoing.Count) * spaceForWire;
            }

            int xOffset = borderPadding.X + xSpacing[0];
            int[] xOffsets = new int[columns];
            for (int i = 0; i < xGroups.Length; i++)
            {
                int widest = xGroups[i].Max(x => NodeSizes[x.Key.Value].X);
                int xIndex = xGroups[i].Key - minPos.X;

                xOffsets[xIndex] = xOffset;

                xOffset += widest + xSpacing[i + 1];
            }

            return xOffsets;
        }

        private int[] MakeYOffsets(IGrouping<int, KeyValuePair<Node<FIRRTLNode>, Point>>[] yGroups, Point minPos, Point borderPadding, int rows)
        {
            int yOffset = borderPadding.Y;
            int[] yOffsets = new int[rows];
            foreach (var yGroup in yGroups)
            {
                int widest = yGroup.Max(x => NodeSizes[x.Key.Value].Y);
                int yIndex = yGroup.Key - minPos.Y;

                yOffsets[yIndex] = yOffset;

                yOffset += widest + borderPadding.Y;
            }

            return yOffsets;
        }

        private Dictionary<Node<FIRRTLNode>, Point> GetPlacements(Graph<FIRRTLNode> graph, List<Node<FIRRTLNode>> modInputNodes, List<Node<FIRRTLNode>> modOutputNodes)
        {
            //Make initial x ordering
            Dictionary<Node<FIRRTLNode>, int> xOrdering = graph.TopologicalSort();

            //Make y ordering
            Dictionary<Node<FIRRTLNode>, float> yOrdering = new Dictionary<Node<FIRRTLNode>, float>();
            var xGroups = xOrdering
                    .GroupBy(x => x.Value)
                    .OrderBy(x => x.First().Value)
                    .Select(x => x.Select(y => y.Key).ToArray())
                    .ToArray();

            //Set initial y ordering
            foreach (var group in xGroups)
            {
                int y = 0;
                foreach (var node in group)
                {
                    yOrdering.Add(node, y++);
                }
            }

            Dictionary<Node<FIRRTLNode>, Point> placement = new Dictionary<Node<FIRRTLNode>, Point>();
            foreach (var node in xOrdering.Keys)
            {
                placement.Add(node, new Point(xOrdering[node], (int)MathF.Round(yOrdering[node], 0)));
            }

            return placement;
        }

        public void SetNodeSize(FIRRTLNode node, Point size)
        {
            lock (SizeChanges)
            {
                //If the size hasn't changed then there is no need to
                //do anything at all as the result will be the same
                if (NodeSizes.TryGetValue(node, out var oldSize) && oldSize == size)
                {
                    return;
                }

                SizeChanges[node] = size;
                MissingNodeDims.Remove(node);
            }
        }

        public bool IsReadyToPlace()
        {
            return MissingNodeDims.Count == 0 && SizeChanges.Count > 0;
        }
    }

    public class PlacementInfo
    {
        public readonly List<Positioned<FIRRTLNode>> NodePositions;
        public readonly Dictionary<FIRRTLNode, Rectangle> UsedSpace;
        public Point SpaceNeeded { get; private set; }
        
        public PlacementInfo() : this(new List<Positioned<FIRRTLNode>>(), new Dictionary<FIRRTLNode, Rectangle>(), Point.Zero)
        { }

        public PlacementInfo(List<Positioned<FIRRTLNode>> nodePoses, Dictionary<FIRRTLNode, Rectangle> usedSpace, Point spaceNeeded)
        {
            this.NodePositions = nodePoses;
            this.UsedSpace = usedSpace;
            this.SpaceNeeded = spaceNeeded;
        }

        internal void AddNodePlacement(FIRRTLNode node, Rectangle shape)
        {
            NodePositions.Add(new Positioned<FIRRTLNode>(shape.Pos, node));
            SpaceNeeded = Point.Max(SpaceNeeded, new Point(shape.RightX, shape.BottomY));

            UsedSpace.Add(node, shape);
        }

        internal void AddEndStuff(Point endPadding)
        {
            SpaceNeeded += endPadding;
        }

        public void SetSpaceNeeded(Point spaceNeeded)
        {
            SpaceNeeded = spaceNeeded;
        }
    }
}
