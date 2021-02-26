using ChiselDebug.GraphFIR;
using System;
using System.Collections.Generic;
using System.Text;
using ChiselDebug.Graphing;
using System.Linq;
using System.Diagnostics;

namespace ChiselDebug
{
    public class Placer
    {
        private readonly Module Mod;
        private readonly HashSet<FIRRTLNode> MissingNodeDims;
        private readonly Dictionary<FIRRTLNode, Point> NodeSizes = new Dictionary<FIRRTLNode, Point>();

        public delegate void PlacedHandler(PlacementInfo placements);
        public event PlacedHandler OnPlacedNodes;

        public Placer(Module mod)
        {
            this.Mod = mod;
            this.MissingNodeDims = new HashSet<FIRRTLNode>(Mod.GetAllNodes());
            MissingNodeDims.RemoveWhere(x => x is INoPlaceAndRoute);
        }

        private PlacementInfo PositionModuleComponents()
        {
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
                Dictionary<Input, FIRRTLNode> inputToFirNode = new Dictionary<Input, FIRRTLNode>();
                Dictionary<Output, FIRRTLNode> outputToFirNode = new Dictionary<Output, FIRRTLNode>();
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

                Dictionary<Input, Node<FIRRTLNode>> inputToNode = new Dictionary<Input, Node<FIRRTLNode>>();
                Dictionary<Output, Node<FIRRTLNode>> outputToNode = new Dictionary<Output, Node<FIRRTLNode>>();
                foreach (var keyValue in inputToFirNode)
                {
                    inputToNode.Add(keyValue.Key, firNodeToNode[keyValue.Value]);
                }
                foreach (var keyValue in outputToFirNode)
                {
                    outputToNode.Add(keyValue.Key, firNodeToNode[keyValue.Value]);
                }

                List<Node<FIRRTLNode>> modInputNodes = new List<Node<FIRRTLNode>>();
                foreach (var input in Mod.InternalInputs)
                {
                    Node<FIRRTLNode> modInputNode = new Node<FIRRTLNode>(Mod);
                    graph.AddNode(modInputNode);
                    inputToNode.Add(input, modInputNode);
                    modInputNodes.Add(modInputNode);
                }
                List<Node<FIRRTLNode>> modOutputNodes = new List<Node<FIRRTLNode>>();
                foreach (var output in Mod.InternalOutputs)
                {
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

                Dictionary<Node<FIRRTLNode>, int> xOrdering = graph.TopologicalSort();

                {
                    int minXOrdering = xOrdering.Values.Min();
                    int maxXOrdering = xOrdering.Values.Max();

                    foreach (var modInputNode in modInputNodes)
                    {
                        xOrdering[modInputNode] = maxXOrdering + 1;
                    }
                    foreach (var modOutputNode in modOutputNodes)
                    {
                        xOrdering[modOutputNode] = minXOrdering - 1;
                    }
                }


                Dictionary<Node<FIRRTLNode>, float> yOrdering = new Dictionary<Node<FIRRTLNode>, float>();
                var xGroups = xOrdering
                    .GroupBy(x => x.Value)
                    .OrderByDescending(x => x.First().Value)
                    .Select(x => x.Select(y => y.Key).Where(y => y.Value != Mod).ToArray())
                    .ToArray();

                foreach (var group in xGroups)
                {
                    int y = 0;
                    foreach (var node in group)
                    {
                        yOrdering.Add(node, y++);
                    }
                }

                {
                    float inputStartY = modInputNodes.Count / 2.0f;
                    for (int y = 0; y < modInputNodes.Count; y++)
                    {
                        yOrdering.Add(modInputNodes[y], y - inputStartY);
                    }
                    float outputStartY = modOutputNodes.Count / 2.0f;
                    for (int y = 0; y < modOutputNodes.Count; y++)
                    {
                        yOrdering.Add(modOutputNodes[y], y - outputStartY);
                    }
                }

                const int iterations = 400;
                for (int i = 0; i < iterations; i++)
                {
                    {
                        List<float> inputNodeValues = modInputNodes.SelectMany(x => x.Incomming).Select(x => yOrdering[x]).ToList();
                        float inputStartY = (inputNodeValues.Sum() / inputNodeValues.Count) / 2.0f;
                        Debug.WriteLine(inputStartY);
                        for (int y = 0; y < modInputNodes.Count; y++)
                        {
                            yOrdering[modInputNodes[y]] = y - inputStartY;
                        }
                        List<float> outputNodeValues = modOutputNodes.SelectMany(x => x.Outgoing).Select(x => yOrdering[x]).ToList();
                        float outputStartY = (outputNodeValues.Sum() / outputNodeValues.Count) / 2.0f;
                        for (int y = 0; y < modOutputNodes.Count; y++)
                        {
                            yOrdering[modOutputNodes[y]] = y - outputStartY;
                        }
                    }
                    foreach (var group in xGroups)
                    {
                        Dictionary<Node<FIRRTLNode>, float> newPosition = new Dictionary<Node<FIRRTLNode>, float>();
                        foreach (var node in group)
                        {
                            float WeighedDistance(float currNodeY, float otherNodeY)
                            {
                                return otherNodeY;
                                float distance = otherNodeY - currNodeY;
                                float weighedDist = MathF.Pow(MathF.Abs(distance), 0.95f);
                                float signedWeighedDist = MathF.CopySign(weighedDist, distance);
                                return currNodeY + signedWeighedDist;
                            }

                            float currY = yOrdering[node];
                            float parentSum = node.Incomming.Sum(x => WeighedDistance(currY, yOrdering[x]));
                            float childSum = node.Outgoing.Sum(x => WeighedDistance(currY, yOrdering[x]));
                            float mean = (parentSum + childSum) / (node.Incomming.Count + node.Outgoing.Count);

                            newPosition.Add(node, mean);
                        }

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

                            float middleIndex = cluster.Count / 2.0f;
                            float overlapWithPrevCluster =  Math.Max(0.0f, prevClusterMaxY + 1 - (yMean - middleIndex));

                            for (int z = 0; z < cluster.Count; z++)
                            {
                                yOrdering[cluster[z]] = yMean + (z - middleIndex) + overlapWithPrevCluster;
                            }

                            prevClusterMaxY = yOrdering[cluster.Last()];
                        }
                    }
                }

                foreach (var modIONode in modInputNodes)
                {
                    xOrdering.Remove(modIONode);
                    yOrdering.Remove(modIONode);
                    graph.RemoveNode(modIONode);
                }
                foreach (var modIONode in modOutputNodes)
                {
                    xOrdering.Remove(modIONode);
                    yOrdering.Remove(modIONode);
                    graph.RemoveNode(modIONode);
                }

                {
                    int minXOrdering = xOrdering.Values.Min();
                    float minYOrdering = yOrdering.Values.Min();
                    foreach (var node in xOrdering.Keys)
                    {
                        Point size = NodeSizes[node.Value];
                        int xOrder = xOrdering[node] - minXOrdering;
                        int yOrder = (int)(yOrdering[node] - minYOrdering);

                        Point pos = new Point(50 + xOrder * 150, 30 + yOrder * 85);
                        placments.AddNodePlacement(node.Value, new Rectangle(pos, size));
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

        public void SetNodeSize(FIRRTLNode node, Point size)
        {
            NodeSizes[node] = size;

            MissingNodeDims.Remove(node);
            if (MissingNodeDims.Count == 0)
            {
                OnPlacedNodes?.Invoke(PositionModuleComponents());
            }
        }
    }

    public class PlacementInfo
    {
        public readonly List<Positioned<FIRRTLNode>> NodePositions = new List<Positioned<FIRRTLNode>>();
        public readonly Dictionary<FIRRTLNode, Rectangle> UsedSpace = new Dictionary<FIRRTLNode, Rectangle>();
        public Point SpaceNeeded { get; private set; } = new Point(0, 0);

        internal void AddNodePlacement(FIRRTLNode node, Rectangle shape)
        {
            NodePositions.Add(new Positioned<FIRRTLNode>(shape.Pos, node));
            SpaceNeeded = Point.Max(SpaceNeeded, new Point(shape.RightX, shape.BottomY));

            UsedSpace.Add(node, shape);
        }

        internal void AddEndStuff()
        {
            SpaceNeeded += new Point(50, 0);
        }
    }
}
