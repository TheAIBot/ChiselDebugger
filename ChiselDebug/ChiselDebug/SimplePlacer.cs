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
                        if (input.Con == null)
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
                        if (!output.Con.IsUsed())
                        {
                            continue;
                        }
                        Node<FIRRTLNode> modOutputNode = new Node<FIRRTLNode>(Mod);
                        graph.AddNode(modOutputNode);
                        outputToNode.Add(output, modOutputNode);
                        modOutputNodes.Add(modOutputNode);
                    }


                    //Make edges
                    foreach (var output in outputToNode.Keys)
                    {
                        if (!output.Con.IsUsed())
                        {
                            continue;
                        }
                        var from = outputToNode[output];
                        foreach (var input in output.Con.To)
                        {
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

                    {
                        FIRRTLNode[][] nodePlacements = placement
                            .GroupBy(x => x.Value.X)
                            .OrderBy(x => x.Key)
                            .Select(x => x
                                .OrderBy(y => y.Value.Y)
                                .Select(y => y.Key.Value)
                                .ToArray())
                            .ToArray();

                        int xOffset = 50;
                        for (int x = 0; x < nodePlacements.Length; x++)
                        {
                            int yOffset = 50;
                            int largestWidth = 0;
                            for (int y = 0; y < nodePlacements[x].Length; y++)
                            {
                                FIRRTLNode node = nodePlacements[x][y];

                                Point offset = new Point(xOffset, yOffset);
                                Point padding = new Point(200, 100);
                                Point pos = offset + padding;
                                Point size = NodeSizes[node];
                                Point paddedSize = size + padding;

                                placments.AddNodePlacement(node, new Rectangle(pos, NodeSizes[node]));

                                largestWidth = Math.Max(largestWidth, paddedSize.X);
                                yOffset += paddedSize.Y;
                            }

                            xOffset += largestWidth;
                        }
                    }

                    placments.AddEndStuff();
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

        private Dictionary<Node<FIRRTLNode>, Point> GetPlacements(Graph<FIRRTLNode> graph, List<Node<FIRRTLNode>> modInputNodes, List<Node<FIRRTLNode>> modOutputNodes)
        {
            Node<FIRRTLNode>[][] GetXGroups(Dictionary<Node<FIRRTLNode>, int> xOrdering)
            {
                return xOrdering
                    .GroupBy(x => x.Value)
                    .OrderBy(x => x.First().Value)
                    .Select(x => x.Select(y => y.Key).ToArray())
                    .ToArray();
            }

            void RecenterModuleIO(Dictionary<Node<FIRRTLNode>, float> yOrdering)
            {
                {
                    List<float> inputNodeValues = modInputNodes.SelectMany(x => x.Incomming).Select(x => yOrdering[x]).ToList();
                    float inputStartY = (inputNodeValues.Sum() / inputNodeValues.Count) / 2.0f;
                    for (int y = 0; y < modInputNodes.Count; y++)
                    {
                        yOrdering[modInputNodes[y]] = y - inputStartY;
                    }
                }
                {
                    List<float> outputNodeValues = modOutputNodes.SelectMany(x => x.Outgoing).Select(x => yOrdering[x]).ToList();
                    float outputStartY = (outputNodeValues.Sum() / outputNodeValues.Count) / 2.0f;
                    for (int y = 0; y < modOutputNodes.Count; y++)
                    {
                        yOrdering[modOutputNodes[y]] = y - outputStartY;
                    }
                }
            }

            Dictionary<Node<FIRRTLNode>, Point> ToPlacement(Dictionary<Node<FIRRTLNode>, int> xOrdering, Dictionary<Node<FIRRTLNode>, float> yOrdering)
            {
                Dictionary<Node<FIRRTLNode>, Point> placement = new Dictionary<Node<FIRRTLNode>, Point>();
                foreach (var node in xOrdering.Keys)
                {
                    placement.Add(node, new Point(xOrdering[node], (int)MathF.Round(yOrdering[node], 0)));
                }

                return placement;
            }

            void OptimizeXOrdering(Dictionary<Node<FIRRTLNode>, int> xOrdering)
            {
                int minOrder = xOrdering.Values.Min();
                int maxOrder = xOrdering.Values.Max();
                Dictionary<Node<FIRRTLNode>, int> prefXOrdering = new Dictionary<Node<FIRRTLNode>, int>();
                foreach (var node in xOrdering.Keys)
                {
                    int nodeMinOrder = minOrder;
                    int nodeMaxOrder = maxOrder;

                    if (node.Incomming.Count > 0)
                    {
                        nodeMinOrder = node.Incomming.Max(x => xOrdering[x]) + 1;
                    }

                    if (node.Outgoing.Count > 0)
                    {
                        nodeMaxOrder = node.Outgoing.Min(x => xOrdering[x]) - 1;
                    }

                    //if (nodeMinOrder >= nodeMaxOrder)
                    //{
                        prefXOrdering[node] = nodeMaxOrder;
                    //}
                    //else
                    //{
                    //    prefXOrdering[node] = (nodeMinOrder + nodeMaxOrder) / 2;
                    //}
                }

                foreach (var nodePref in prefXOrdering)
                {
                    xOrdering[nodePref.Key] = nodePref.Value;
                }
            }

            void OptimizeYOrdering(Node<FIRRTLNode>[][] xGroups, Dictionary<Node<FIRRTLNode>, int> xOrdering, Dictionary<Node<FIRRTLNode>, float> yOrdering)
            {
                var prefYGroups = new List<Dictionary<Node<FIRRTLNode>, float>>();
                foreach (var group in xGroups)
                {
                    Dictionary<Node<FIRRTLNode>, float> prefYPoses = new Dictionary<Node<FIRRTLNode>, float>();
                    foreach (var node in group)
                    {
                        if (node.Incomming.Count == 0 &&
                            node.Outgoing.Count == 0 &&
                            node.Indirectly.Count == 0)
                        {
                            prefYPoses.Add(node, yOrdering[node]);
                            continue;
                        }

                        float parentWeight = 1;
                        float childWeight = 1;
                        float indirectweight = 5;

                        float ySum = 0;
                        ySum += node.Incomming.Sum(x => yOrdering[x]) * parentWeight;
                        ySum += node.Outgoing.Sum(x => yOrdering[x]) * childWeight;
                        ySum += node.Indirectly.Sum(x => yOrdering[x]) * indirectweight;

                        float weightSum = 0;
                        weightSum += node.Incomming.Count * parentWeight;
                        weightSum += node.Outgoing.Count * childWeight;
                        weightSum += node.Indirectly.Count * indirectweight;

                        float mean = ySum / weightSum;
                        prefYPoses.Add(node, mean);
                    }

                    prefYGroups.Add(prefYPoses);
                }

                foreach (var newPosition in prefYGroups)
                {
                    List<List<Node<FIRRTLNode>>> clusters = new List<List<Node<FIRRTLNode>>>();
                    float prevElemY = -100000;
                    List<Node<FIRRTLNode>> currCluster = null;
                    foreach (var node in newPosition.OrderBy(x => x.Value))
                    {
                        if (Math.Abs(node.Value - prevElemY) > 1.0f)
                        {
                            if (currCluster != null)
                            {
                                clusters.Add(currCluster);
                            }

                            currCluster = new List<Node<FIRRTLNode>>();
                            currCluster.Add(node.Key);
                        }
                        else
                        {
                            if (currCluster == null)
                            {
                                currCluster = new List<Node<FIRRTLNode>>();
                            }

                            currCluster.Add(node.Key);
                        }

                        prevElemY = node.Value;
                    }
                    if (currCluster != null)
                    {
                        clusters.Add(currCluster);
                    }

                    float prevClusterMaxY = -100000;
                    foreach (var cluster in clusters)
                    {
                        float yMean = cluster.Sum(x => newPosition[x]) / cluster.Count;

                        float middleIndex = (cluster.Count - 1) / 2.0f;
                        float overlapWithPrevCluster = Math.Max(0.0f, prevClusterMaxY + 1 - (yMean - middleIndex));

                        for (int z = 0; z < cluster.Count; z++)
                        {
                            yOrdering[cluster[z]] = yMean + (z - middleIndex) + overlapWithPrevCluster;
                        }

                        prevClusterMaxY = yOrdering[cluster.Last()];
                    }
                }
            }

            float GetPlacementScore(Dictionary<Node<FIRRTLNode>, Point> placement)
            {
                float score = 0;
                foreach (var node in placement.Keys)
                {
                    Point nodePos = placement[node];

                    foreach (var parent in node.Incomming)
                    {
                        Point parentPos = placement[parent];

                        Point diff = (nodePos - parentPos).Abs();
                        score += diff.X + diff.Y;
                    }

                    //foreach (var child in node.Outgoing)
                    //{
                    //    Point childPos = placement[child];

                    //    Point diff = (nodePos - childPos).Abs();
                    //    score += diff.X + diff.Y;
                    //}
                }

                return score;
            }

            //Make initial x ordering
            Dictionary<Node<FIRRTLNode>, int> xOrdering = graph.TopologicalSort();

            //Make y ordering
            Dictionary<Node<FIRRTLNode>, float> yOrdering = new Dictionary<Node<FIRRTLNode>, float>();
            var xGroups = GetXGroups(xOrdering);

            //Set initial y ordering
            foreach (var group in xGroups)
            {
                int y = 0;
                foreach (var node in group)
                {
                    yOrdering.Add(node, y++);
                }
            }

            //for (int x = 0; x < 5; x++)
            {
                //OptimizeXOrdering(xOrdering);
                //xGroups = GetXGroups(xOrdering);

                for (int y = 0; y < 200; y++)
                {
                    OptimizeYOrdering(xGroups, xOrdering, yOrdering);
                }
            }

            return ToPlacement(xOrdering, yOrdering);
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
        public readonly List<Positioned<FIRRTLNode>> NodePositions = new List<Positioned<FIRRTLNode>>();
        public readonly Dictionary<FIRRTLNode, Rectangle> UsedSpace = new Dictionary<FIRRTLNode, Rectangle>();
        public Point SpaceNeeded { get; private set; } = Point.Zero;

        internal void AddNodePlacement(FIRRTLNode node, Rectangle shape)
        {
            NodePositions.Add(new Positioned<FIRRTLNode>(shape.Pos, node));
            SpaceNeeded = Point.Max(SpaceNeeded, new Point(shape.RightX, shape.BottomY));

            UsedSpace.Add(node, shape);
        }

        internal void AddEndStuff()
        {
            SpaceNeeded += new Point(50, 50);
        }
    }
}
