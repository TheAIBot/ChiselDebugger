using ChiselDebug.FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebug
{
    public class ConnectionsHandler
    {
        private readonly struct IOInfo
        {
            internal readonly Point Position;
            internal readonly FIRRTLNode Node;

            public IOInfo(Point pos, FIRRTLNode node)
            {
                this.Position = pos;
                this.Node = node;
            }
        }

        private readonly Module Mod;
        private readonly List<Connection> UsedModuleConnections;
        private readonly Dictionary<Input, IOInfo> InputOffsets = new Dictionary<Input, IOInfo>();
        private readonly Dictionary<Output, IOInfo> OutputOffsets = new Dictionary<Output, IOInfo>();

        public ConnectionsHandler(Module mod)
        {
            this.Mod = mod;
            this.UsedModuleConnections = Mod.GetAllModuleConnections();
        }

        public void UpdateIOFromNode(FIRRTLNode node, List<Positioned<Input>> inputOffsets, List<Positioned<Output>> outputOffsets)
        {
            foreach (var inputPos in inputOffsets)
            {
                InputOffsets[inputPos.Value] = new IOInfo(inputPos.Position, node);
            }
            foreach (var outputPos in outputOffsets)
            {
                OutputOffsets[outputPos.Value] = new IOInfo(outputPos.Position, node);
            }
        }

        public List<Line> GetAllConnectionLines(PlacementInfo placements)
        {
            Dictionary<FIRRTLNode, Point> nodePoses = new Dictionary<FIRRTLNode, Point>();
            foreach (var nodePos in placements.NodePositions)
            {
                nodePoses.Add(nodePos.Value, nodePos.Position);
            }
            nodePoses.Add(Mod, new Point(0, 0));

            List<Line> lines = new List<Line>();
            foreach (var connection in UsedModuleConnections)
            {
                if (OutputOffsets.TryGetValue(connection.From, out IOInfo outputInfo))
                {
                    foreach (var input in connection.To)
                    {
                        if (InputOffsets.TryGetValue(input, out IOInfo inputInfo))
                        {
                            Point startOffset = nodePoses[outputInfo.Node];
                            Point endOffset = nodePoses[inputInfo.Node];

                            Point start = startOffset + outputInfo.Position;
                            Point end = endOffset + inputInfo.Position;
                            lines.Add(new Line(start, end));
                        }
                    }
                }
            }

            return lines;
        }
    }
}
