using ChiselDebug;
using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Routing;
using ChiselDebug.Timeline;
using ChiselDebuggerWebUI.Code.Templates;
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
        private readonly Dictionary<FIRRTLNode, ModuleLayout> FIRNodeToModCtrl = new Dictionary<FIRRTLNode, ModuleLayout>();
        private readonly List<ModuleLayout> ModControllers = new List<ModuleLayout>();
        private readonly BroadcastBlock<Action> TimeChanger = null;
        private readonly PlacementTemplator PlacementTemplates = new PlacementTemplator();
        private readonly RouteTemplator RouteTemplates = new RouteTemplator();
        public Point CircuitSize { get; private set; } = Point.Zero;

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

        public void AddModCtrl(string moduleName, ModuleLayout modCtrl, FIRRTLNode[] modNodes, FIRRTLNode[] modNodesIncludeMod, FIRIO[] modIO)
        {
            ModControllers.Add(modCtrl);
            foreach (var node in modNodes)
            {
                FIRNodeToModCtrl.Add(node, modCtrl);
            }

            PlacementTemplates.SubscribeToTemplate(moduleName, modCtrl, modNodes);
            RouteTemplates.SubscribeToTemplate(moduleName, modCtrl, modNodesIncludeMod, modIO);
        }

        internal void AddPlaceTemplateParameters(string moduleName, SimplePlacer placer, FIRRTLNode[] nodeOrder)
        {
            PlacementTemplates.AddTemplateParameters(moduleName, placer, nodeOrder);
        }

        internal void AddRouteTemplateParameters(string moduleName, SimpleRouter router, PlacementInfo placeInfo, FIRRTLNode[] nodeOrder, FIRIO[] ioOrder)
        {
            RouteTemplates.AddTemplateParameters(moduleName, router, placeInfo, nodeOrder, ioOrder);
        }

        internal bool TryGetPlaceTemplate(string moduleName, ModuleLayout modLayout, out PlacementInfo placement)
        {
            return PlacementTemplates.TryGetTemplate(moduleName, modLayout, out placement);
        }

        internal bool TryGetRouteTemplate(string moduleName, ModuleLayout modLayout, out List<WirePath> wires)
        {
            return RouteTemplates.TryGetTemplate(moduleName, modLayout, out wires);
        }

        public void SetCircuitSize(Point size)
        {
            CircuitSize = size;
        }

        public void SetCircuitState(ulong time)
        {
            TimeChanger.Post(() =>
            {
                List<Connection> changedConnections = Graph.SetState(Timeline.GetStateAtTime(time));

                HashSet<ModuleLayout> modulesToReRender = new HashSet<ModuleLayout>();

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
            if (TimeChanger != null)
            {
                WorkLimiter.UnlinkSource(TimeChanger);
            }
        }
    }
}
