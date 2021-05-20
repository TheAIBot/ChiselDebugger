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
    public class SimplePlacer : PlacingBase
    {
        public SimplePlacer(Module mod) : base(mod)
        { }

        protected override PlacementInfo PositionComponents(Dictionary<FIRRTLNode, Point> nodeSizes, Module mod)
        {
            PlacementInfo placments = new PlacementInfo();

            Graph<FIRRTLNode> graph = new Graph<FIRRTLNode>();

            //Add nodes to graph
            Dictionary<FIRRTLNode, Node<FIRRTLNode>> firNodeToNode = new Dictionary<FIRRTLNode, Node<FIRRTLNode>>();
            foreach (var firNode in nodeSizes.Keys)
            {
                var node = new Node<FIRRTLNode>(firNode);
                graph.AddNode(node);
                firNodeToNode.Add(firNode, node);
            }

            //Relate io to FIRRTLNode
            Dictionary<ScalarIO, FIRRTLNode> inputToFirNode = new Dictionary<ScalarIO, FIRRTLNode>();
            Dictionary<ScalarIO, FIRRTLNode> outputToFirNode = new Dictionary<ScalarIO, FIRRTLNode>();
            foreach (var firNode in nodeSizes.Keys)
            {
                if (firNode == mod)
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
            foreach (var input in mod.GetInternalInputs())
            {
                if (!input.IsConnectedToAnything())
                {
                    continue;
                }
                Node<FIRRTLNode> modInputNode = new Node<FIRRTLNode>(mod);
                graph.AddNode(modInputNode);
                inputToNode.Add(input, modInputNode);
                modInputNodes.Add(modInputNode);
            }
            List<Node<FIRRTLNode>> modOutputNodes = new List<Node<FIRRTLNode>>();
            foreach (var output in mod.GetInternalOutputs())
            {
                if (!output.IsConnectedToAnything())
                {
                    continue;
                }
                Node<FIRRTLNode> modOutputNode = new Node<FIRRTLNode>(mod);
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

            var placement = GetPlacements(graph);
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

            int[] xOffsets = MakeXOffsets(xGroups, min, columns, nodeSizes, mod);
            int[] yOffsets = MakeYOffsets(yGroups, min, 72, rows, nodeSizes);

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

                    placments.AddNodePlacement(node, new Rectangle(pos, nodeSizes[node]));
                }
            }

            Point borderPadding = new Point(100, 200);
            placments.AutoSpacePlacementRanks(mod);
            placments.AddBorderPadding(borderPadding);
            return placments;
        }

        private int[] MakeXOffsets(IGrouping<int, KeyValuePair<Node<FIRRTLNode>, Point>>[] xGroups, Point minPos, int columns, Dictionary<FIRRTLNode, Point> nodeSizes, Module mod)
        {
            int xOffset = 0;
            int[] xOffsets = new int[columns];
            for (int i = 0; i < xGroups.Length; i++)
            {
                int widest = xGroups[i].Max(x => nodeSizes[x.Key.Value].X);
                int xIndex = xGroups[i].Key - minPos.X;

                xOffsets[xIndex] = xOffset;

                xOffset += widest + 1;
            }

            return xOffsets;
        }

        private int[] MakeYOffsets(IGrouping<int, KeyValuePair<Node<FIRRTLNode>, Point>>[] yGroups, Point minPos, int yCompDist, int rows, Dictionary<FIRRTLNode, Point> nodeSizes)
        {
            int yOffset = 0;
            int[] yOffsets = new int[rows];
            foreach (var yGroup in yGroups)
            {
                int widest = yGroup.Max(x => nodeSizes[x.Key.Value].Y);
                int yIndex = yGroup.Key - minPos.Y;

                yOffsets[yIndex] = yOffset;

                yOffset += widest + yCompDist;
            }

            return yOffsets;
        }

        private Dictionary<Node<FIRRTLNode>, Point> GetPlacements(Graph<FIRRTLNode> graph)
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


    }
}
