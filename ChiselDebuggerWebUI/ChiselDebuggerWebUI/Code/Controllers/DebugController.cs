using ChiselDebug;
using ChiselDebug.GraphFIR;
using ChiselDebug.Timeline;
using ChiselDebuggerWebUI.Components;
using System;
using System.Collections.Generic;

namespace ChiselDebuggerWebUI.Code
{
    public class DebugController : IDisposable
    {
        private readonly CircuitGraph Graph;
        public VCDTimeline Timeline { get; init; }
        private readonly Dictionary<Connection, List<IFIRUINode>> ConToUINode = new Dictionary<Connection, List<IFIRUINode>>();
        private readonly List<ModuleController> ModControllers = new List<ModuleController>();

        public delegate void ReRenderCircuit();
        public event ReRenderCircuit OnReRender;

        private readonly ExecuteOnlyLatest<ulong> TimeChanger = new ExecuteOnlyLatest<ulong>();

        public DebugController(CircuitGraph graph, VCDTimeline timeline)
        {
            this.Graph = graph;
            this.Timeline = timeline;
            TimeChanger.Start(time =>
            {
                List<Connection> changedConnections = Graph.SetState(Timeline.GetStateAtTime(time));

                //Only rerender the uiNodes that are connected to a connection
                //that changed value. UiNodes are not rerendered here, but they
                //are being allowed to rerender here.
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

                //Ask ui to render again so the allowed uiNodes
                //can be rerendered.
                OnReRender?.Invoke();
            });
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

        public void AddModCtrl(ModuleController modCtrl)
        {
            ModControllers.Add(modCtrl);
        }

        public void SetCircuitState(ulong time)
        {
            TimeChanger.AddWork(time);
        }

        public void Dispose()
        {
            TimeChanger.Dispose();
        }
    }
}
