using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Utilities;
using Rubjerg.Graphviz;

namespace ChiselDebug.Placing.GraphViz
{
    internal sealed class GraphVizPlacer : PlacingBase
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

            //Add nodes to graph
            Dictionary<FIRRTLNode, Node> firNodeToNode = new Dictionary<FIRRTLNode, Node>();
            Dictionary<Sink, string> inputToPort = new Dictionary<Sink, string>();
            Dictionary<Source, string> outputToPort = new Dictionary<Source, string>();
            Dictionary<Sink, Node> inputToNode = new Dictionary<Sink, Node>();
            Dictionary<Source, Node> outputToNode = new Dictionary<Source, Node>();
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

                Sink[] nodeInputs = firNode.Key.GetSinks();
                Source[] nodeOutputs = firNode.Key.GetSources();

                MakeIntoRecord(node, nodeInputs, nodeOutputs, inputToPort, outputToPort, ref portName);

                firNodeToNode.Add(firNode.Key, node);

                foreach (var input in nodeInputs)
                {
                    inputToNode.Add(input, node);
                }
                foreach (var output in nodeOutputs)
                {
                    outputToNode.Add(output, node);
                }
            }

            Node modInputNode = graph.GetOrAddNode("modInput");
            MakeIntoRecord(modInputNode, mod.GetSinks(), Array.Empty<Source>(), inputToPort, outputToPort, ref portName);
            modInputNode.SafeSetAttribute("rank", "sink", "sink");

            Node modOutputNode = graph.GetOrAddNode("modOutput");
            MakeIntoRecord(modInputNode, Array.Empty<Sink>(), mod.GetSources(), inputToPort, outputToPort, ref portName);
            modOutputNode.SafeSetAttribute("rank", "source", "source");

            //Make edges
            int edgeCounter = 0;
            foreach (Source output in outputToNode.Keys)
            {
                if (!output.IsConnectedToAnythingPlaceable())
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
                    var edge = graph.GetOrAddEdge(from, to, edgeCounter++.ToString());
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
                var topLeft = center - nodeSize / 2;

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

            Point borderPadding = new Point(20, 40);
            placments.AutoSpacePlacementRanks(mod);
            placments.SetBorderPadding(borderPadding);
            return placments;
        }

        private void MakeIntoRecord(Node node, Sink[] inputs, Source[] outputs, Dictionary<Sink, string> inputToPort, Dictionary<Source, string> outputToPort, ref int portName)
        {
            Array.Reverse(inputs);
            Array.Reverse(outputs);

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
