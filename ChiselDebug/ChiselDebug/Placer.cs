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

                Node<FIRRTLNode> modInputNode = new Node<FIRRTLNode>(Mod);
                graph.AddNode(modInputNode);
                foreach (var input in Mod.InternalInputs)
                {
                    inputToNode.Add(input, modInputNode);
                }

                Node<FIRRTLNode> modOutputNode = new Node<FIRRTLNode>(Mod);
                graph.AddNode(modOutputNode);
                foreach (var output in Mod.InternalOutputs)
                {
                    outputToNode.Add(output, modOutputNode);
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
                xOrdering.Remove(modInputNode);
                xOrdering.Remove(modOutputNode);
                graph.RemoveNode(modInputNode);
                graph.RemoveNode(modOutputNode);

                Dictionary<Node<FIRRTLNode>, int> yOrdering = new Dictionary<Node<FIRRTLNode>, int>();
                var xGroups = xOrdering.GroupBy(x => x.Value).Select(x => x.Select(y => y.Key).ToArray()).ToArray();
                foreach (var group in xGroups)
                {
                    int y = 0;
                    foreach (var node in group)
                    {
                        yOrdering.Add(node, y++);
                    }
                }

                const int iterations = 40;
                for (int i = 0; i < iterations; i++)
                {
                    foreach (var group in xGroups)
                    {
                        Dictionary<Node<FIRRTLNode>, float> newPosition = new Dictionary<Node<FIRRTLNode>, float>();
                        foreach (var node in group)
                        {
                            float parentSum = node.Incomming.Sum(x => yOrdering[x]);
                            float childSum = node.Outgoing.Sum(x => yOrdering[x]);
                            float mean = (parentSum + childSum) / (node.Incomming.Count + node.Outgoing.Count);

                            newPosition.Add(node, mean);
                        }

                        int minY = 0;
                        foreach (var node in newPosition.OrderBy(x => x.Value))
                        {
                            int newY = (int)MathF.Round(MathF.Max((float)minY, node.Value));
                            yOrdering[node.Key] = newY;
                            minY = newY + 1;
                        }
                    }
                }

                int maxYOrdering = yOrdering.Values.Max();
                var yGroups = yOrdering.GroupBy(x => x.Value).Select(x => x.ToArray()).ToArray();
                int unusedYCount = 0;
                int prevY = -1;
                foreach (var group in yGroups)
                {
                    unusedYCount += (group[0].Value - prevY) - 1;
                    prevY = group[0].Value;
                    foreach (var node in group)
                    {
                        yOrdering[node.Key] = node.Value - unusedYCount;
                    }
                }


                int minXOrdering = xOrdering.Values.Min();
                int minYOrdering = yOrdering.Values.Min();
                foreach (var node in xOrdering.Keys)
                {
                    Point size = NodeSizes[node.Value];
                    int xOrder = xOrdering[node] - minXOrdering;
                    int yOrder = yOrdering[node] - minYOrdering;

                    Point pos = new Point(50 + xOrder * 100, yOrder * 150);
                    placments.AddNodePlacement(node.Value, new Rectangle(pos, size));
                }

                placments.AddEndStuff();
                return placments;
            }
            catch(Exception e)
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
