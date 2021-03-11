using ChiselDebug;
using ChiselDebug.GraphFIR;
using ChiselDebug.Timeline;
using ChiselDebuggerWebUI.Components;
using ChiselDebuggerWebUI.Pages.FIRRTLUI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using VCDReader;

namespace ChiselDebuggerWebUI.Code
{
    public class DebugController : IDisposable
    {
        private readonly CircuitGraph Graph;
        public VCDTimeline Timeline { get; init; } = null;
        private readonly Dictionary<FIRRTLNode, ModuleController> FIRNodeToModCtrl = new Dictionary<FIRRTLNode, ModuleController>();
        private readonly List<ModuleController> ModControllers = new List<ModuleController>();
        private readonly BroadcastBlock<Action> TimeChanger = null;

        public DebugController(CircuitGraph graph, VCD vcd)
        {
            this.Graph = graph;

            if (vcd != null)
            {
                this.Timeline = new VCDTimeline(vcd);
                this.TimeChanger = new BroadcastBlock<Action>(x => x);
                WorkLimiter.LinkSource(TimeChanger);
            }
        }

        public void AddModCtrl(ModuleController modCtrl, FIRRTLNode[] modNodes)
        {
            ModControllers.Add(modCtrl);
            foreach (var node in modNodes)
            {
                FIRNodeToModCtrl.Add(node, modCtrl);
            }
        }

        public void SetCircuitState(ulong time)
        {
            TimeChanger.Post(() =>
            {
                List<Connection> changedConnections = Graph.SetState(Timeline.GetStateAtTime(time));

                HashSet<ModuleController> modulesToReRender = new HashSet<ModuleController>();

                //Only rerender the uiNodes that are connected to a connection
                //that changed value. UiNodes are not rerendered here, but they
                //are being allowed to rerender here.
                foreach (var connection in changedConnections)
                {
                    foreach (var node in connection.To)
                    {
                        if (node.Node != null &&
                            FIRNodeToModCtrl.TryGetValue(node.Node, out var modCtrl1))
                        {
                            modulesToReRender.Add(modCtrl1);
                        }
                    }
                    if (connection.From.Node != null &&
                        FIRNodeToModCtrl.TryGetValue(connection.From.Node, out var modCtrl2))
                    {
                        modulesToReRender.Add(modCtrl2);
                    }
                }

                foreach (var moduleUI in modulesToReRender)
                {
                    moduleUI.RerenderWithoutPreparation();
                }
            });
        }

        public void Dispose()
        {
            WorkLimiter.UnlinkSource(TimeChanger);
        }
    }
}
