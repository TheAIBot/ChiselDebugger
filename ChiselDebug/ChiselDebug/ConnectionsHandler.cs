using ChiselDebug.GraphFIR;
using ChiselDebug.Routing;
using PriorityQueue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebug
{
    public class ConnectionsHandler
    {

        private readonly Module Mod;
        private readonly List<Connection> UsedModuleConnections;
        private readonly Dictionary<FIRIO, IOInfo> IOInfos = new Dictionary<FIRIO, IOInfo>();

        public ConnectionsHandler(Module mod)
        {
            this.Mod = mod;
            this.UsedModuleConnections = Mod.GetAllModuleConnections();
        }

        public void UpdateIOFromNode(FIRRTLNode node, List<DirectedIO> inputOffsets, List<DirectedIO> outputOffsets)
        {
            foreach (var inputPos in inputOffsets)
            {
                IOInfos[inputPos.IO] = new IOInfo(node, inputPos);
            }
            foreach (var inputPos in outputOffsets)
            {
                IOInfos[inputPos.IO] = new IOInfo(node, inputPos);
            }
        }

        internal List<(IOInfo start, IOInfo end)> GetAllConnectionLines(PlacementInfo placements)
        {
            Dictionary<FIRRTLNode, Point> nodePoses = new Dictionary<FIRRTLNode, Point>();
            foreach (var nodePos in placements.NodePositions)
            {
                nodePoses.Add(nodePos.Value, nodePos.Position);
            }
            nodePoses.Add(Mod, new Point(0, 0));

            List<(IOInfo start, IOInfo end)> lines = new List<(IOInfo start, IOInfo end)>();
            foreach (var connection in UsedModuleConnections)
            {
                if (IOInfos.TryGetValue(connection.From, out IOInfo outputInfo))
                {
                    foreach (var input in connection.To)
                    {
                        if (IOInfos.TryGetValue(input, out IOInfo inputInfo))
                        {
                            Point startOffset = nodePoses[outputInfo.Node];
                            Point endOffset = nodePoses[inputInfo.Node];

                            IOInfo offsetStartIO = new IOInfo(outputInfo.Node, outputInfo.DirIO.WithOffsetPosition(startOffset));
                            IOInfo offsetEndIO = new IOInfo(inputInfo.Node, inputInfo.DirIO.WithOffsetPosition(endOffset));

                            lines.Add((offsetStartIO, offsetEndIO));
                        }
                    }
                }
            }

            return lines;
        }
    }
}
