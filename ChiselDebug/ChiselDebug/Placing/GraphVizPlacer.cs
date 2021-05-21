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
            graph.SafeSetAttribute("nodesep", "1.0", "0.25");
            //graph.SafeSetAttribute("ranksep", )

            //Add nodes to graph
            Dictionary<FIRRTLNode, Node> firNodeToNode = new Dictionary<FIRRTLNode, Node>();
            Dictionary<Input, string> inputToPort = new Dictionary<Input, string>();
            Dictionary<Output, string> outputToPort = new Dictionary<Output, string>();
            int nodeCounter = 0;
            int portName = 0;
            foreach (var firNode in nodeSizes)
            {
                if (firNode.Key == mod)
                {
                    continue;
                }

                string nodeName = $"n{nodeCounter++}";
                Node node = graph.GetOrAddNode(nodeName);
                node.SafeSetAttribute("shape", "record", "ellipse");
                node.SafeSetAttribute("width", ((double)firNode.Value.X / dpi).ToString(), "0.75");
                node.SafeSetAttribute("height", ((double)firNode.Value.Y / dpi).ToString(), "0.5");

                Input[] nodeInputs = firNode.Key.GetInputs();
                Output[] nodeOutputs = firNode.Key.GetOutputs();

                MakeIntoRecord(node, nodeInputs, nodeOutputs, inputToPort, outputToPort, ref portName);

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
            Node modInputNode = graph.GetOrAddNode("modInput");
            MakeIntoRecord(modInputNode, mod.GetInputs(), Array.Empty<Output>(), inputToPort, outputToPort, ref portName);
            modInputNode.SafeSetAttribute("rank", "sink", "sink");

            Node modOutputNode = graph.GetOrAddNode("modOutput");
            MakeIntoRecord(modInputNode, Array.Empty<Input>(), mod.GetOutputs(), inputToPort, outputToPort, ref portName);
            modOutputNode.SafeSetAttribute("rank", "source", "source");

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
                    var edge = graph.GetOrAddEdge(from, to, (edgeCounter++).ToString());
                    edge.SafeSetAttribute("tailport", outputToPort[output], " ");
                    edge.SafeSetAttribute("headport", inputToPort[input], " ");
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

            Point offsetBy = min.Abs();
            foreach (var firRect in firNodeRects)
            {
                placments.AddNodePlacement(firRect.Key, new Rectangle(offsetBy + firRect.Value.Pos, firRect.Value.Size));
            }

            Point borderPadding = new Point(100, 200);
            placments.AutoSpacePlacementRanks(mod);
            placments.AddBorderPadding(borderPadding);
            return placments;
        }

        private void MakeIntoRecord(Node node, Input[] inputs, Output[] outputs, Dictionary<Input, string> inputToPort, Dictionary<Output, string> outputToPort, ref int portName)
        {
            int length = Math.Max(inputs.Length, outputs.Length);
            List<string> ports = new List<string>();
            for (int i = 0; i < length; i++)
            {
                string inputName = $"e{portName + i}";
                string outputName = $"e{portName + i + length}";
                ports.Add($"{{<{inputName}> | <{outputName}>}}");
            }
            for (int i = 0; i < inputs.Length; i++)
            {
                string inputName = $"e{portName + i}";
                inputToPort.Add(inputs[i], inputName);
            }
            for (int i = 0; i < outputs.Length; i++)
            {
                string outputName = $"e{portName + i + length}";
                outputToPort.Add(outputs[i], outputName);
            }
            node.SafeSetAttribute("label", string.Join(" | ", ports), " ");
            portName += length * 2;
        }
    }
}
