using ChiselDebug;
using ChiselDebug.GraphFIR;
using ChiselDebug.Timeline;
using ChiselDebuggerWebUI.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerWebUI.Code
{
    public class DebugController
    {
        private readonly CircuitGraph Graph;
        public VCDTimeline Timeline { get; init; }
        private readonly Dictionary<Connection, List<IFIRUINode>> ConToUINode = new Dictionary<Connection, List<IFIRUINode>>();

        public delegate void ReRenderCircuit();
        public event ReRenderCircuit OnReRender;

        public DebugController(CircuitGraph graph, VCDTimeline timeline)
        {
            this.Graph = graph;
            this.Timeline = timeline;
        }

        public void AddUINode(IFIRUINode node, List<Connection> connections)
        {
            foreach (var con in connections)
            {
                if (ConToUINode.TryGetValue(con, out var uiNodes))
                {
                    uiNodes.Add(node);
                }
                else
                {
                    List<IFIRUINode> nodes = new List<IFIRUINode>();
                    nodes.Add(node);
                    ConToUINode.Add(con, nodes);
                }
            }
        }

        public void SetCircuitState(ulong time)
        {
            List<Connection> changedConnections = Graph.SetState(Timeline.GetStateAtTime(time));
            foreach (var connection in changedConnections)
            {
                if (ConToUINode.TryGetValue(connection, out var uiNodes))
                {
                    foreach (var uiNode in uiNodes)
                    {
                        uiNode.PrepareForRender();
                    }
                }
            }

            OnReRender?.Invoke();
        }
    }
}
