using ChiselDebug;
using ChiselDebug.GraphFIR;
using ChiselDebug.Timeline;
using ChiselDebuggerWebUI.Components;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChiselDebuggerWebUI.Code
{
    public class DebugController : IDisposable
    {
        private readonly CircuitGraph Graph;
        public VCDTimeline Timeline { get; init; }
        private readonly Dictionary<Connection, List<IFIRUINode>> ConToUINode = new Dictionary<Connection, List<IFIRUINode>>();

        public delegate void ReRenderCircuit();
        public event ReRenderCircuit OnReRender;

        private readonly BlockingCollection<ulong> TimeChanges = new BlockingCollection<ulong>(new ConcurrentQueue<ulong>());
        private readonly Task TimeChangesWorker;
        private readonly CancellationTokenSource CancelSource = new CancellationTokenSource();

        public DebugController(CircuitGraph graph, VCDTimeline timeline)
        {
            this.Graph = graph;
            this.Timeline = timeline;
            this.TimeChangesWorker = Task.Factory.StartNew(async () =>
            {
                try
                {
                    var token = CancelSource.Token;

                    while (!token.IsCancellationRequested)
                    {
                        ulong time = TimeChanges.Take(token);

                        //Many time changes may be queued up and the new circuit state
                        //should be set to the latest one added to the queue so the 
                        //shown circuit state is as up to date as possible
                        if (TimeChanges.Count > 0)
                        {
                            time = TimeChanges.Take(TimeChanges.Count).Last();
                        }

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

                        //Limit how often it can update the circuit state so it
                        //doesn't end up rerendering as fast as possible.
                        await Task.Delay(10);
                    }
                }
                catch (OperationCanceledException e) { }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
                }
            }, TaskCreationOptions.LongRunning);
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
            TimeChanges.Add(time);
        }

        public void Dispose()
        {
            CancelSource.Cancel();
            CancelSource.Dispose();
        }
    }
}
