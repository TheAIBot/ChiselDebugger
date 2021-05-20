using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using Rubjerg.Graphviz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug
{
    public class GraphVizPlacer : PlacingBase
    {
        public GraphVizPlacer(Module mod) : base(mod)
        { }

        protected override PlacementInfo PositionComponents(Dictionary<FIRRTLNode, Point> nodeSizes, Module mod)
        {
            PlacementInfo placments = new PlacementInfo();

            const int dpi = 96;
            const int ppi = 72;

            RootGraph graph = RootGraph.CreateNew("some name??", GraphType.Directed);
            graph.SafeSetAttribute("rankdir", "LR", "TB");
            graph.SafeSetAttribute("ranksep", "7", "0.5");
            graph.SafeSetAttribute("nodesep", "3.0", "0.25");
            //graph.SafeSetAttribute("ranksep", )

            //Add nodes to graph
            Dictionary<FIRRTLNode, Node> firNodeToNode = new Dictionary<FIRRTLNode, Node>();
            int nodeCounter = 0;
            foreach (var firNode in nodeSizes)
            {
                if (firNode.Key == mod)
                {
                    continue;
                }
                Node node = graph.GetOrAddNode((nodeCounter++).ToString());
                node.SafeSetAttribute("shape", "rect", "ellipse");
                node.SafeSetAttribute("width", ((double)firNode.Value.X / dpi).ToString(), "0.75");
                node.SafeSetAttribute("height", ((double)firNode.Value.Y / dpi).ToString(), "0.5");
                firNodeToNode.Add(firNode.Key, node);
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

            Dictionary<ScalarIO, Node> inputToNode = new Dictionary<ScalarIO, Node>();
            Dictionary<ScalarIO, Node> outputToNode = new Dictionary<ScalarIO, Node>();
            foreach (var keyValue in inputToFirNode)
            {
                inputToNode.Add(keyValue.Key, firNodeToNode[keyValue.Value]);
            }
            foreach (var keyValue in outputToFirNode)
            {
                outputToNode.Add(keyValue.Key, firNodeToNode[keyValue.Value]);
            }

            //Make edges
            int edgeCounter = 0;
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
                    graph.GetOrAddEdge(from, to, (edgeCounter++).ToString());
                }
            }

            graph.ComputeLayout();

            Dictionary<FIRRTLNode, Rectangle> firNodeRects = new Dictionary<FIRRTLNode, Rectangle>();
            foreach (var firToVizNode in firNodeToNode)
            {
                var centerF = firToVizNode.Value.Position();
                var center = new Point((int)(centerF.X * dpi), (int)(centerF.Y * dpi)) / ppi;

                var nodeSize = nodeSizes[firToVizNode.Key];
                var topLeft = center - (nodeSize / 2);

                firNodeRects.Add(firToVizNode.Key, new Rectangle(topLeft, nodeSize));
            }
            graph.FreeLayout();

            Point min = new Point(int.MaxValue, int.MaxValue);
            foreach (var rect in firNodeRects.Values)
            {
                min = Point.Min(min, rect.Pos);
            }

            Point borderPadding = new Point(200, 200);
            Point offsetBy = min.Abs() + borderPadding;
            foreach (var firRect in firNodeRects)
            {
                placments.AddNodePlacement(firRect.Key, new Rectangle(offsetBy + firRect.Value.Pos, firRect.Value.Size));
            }

            placments.AddEndStuff(borderPadding);
            return placments;
        }
    }
}
