using ChiselDebug;
using ChiselDebug.CombGraph;
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
        private readonly Dictionary<Output, string> ConToColor = new Dictionary<Output, string>();
        private readonly HashSet<Output> ConstCons = new HashSet<Output>();
        public Point CircuitSize { get; private set; } = Point.Zero;

        public DebugController(CircuitGraph graph, VCD vcd)
        {
            this.Graph = graph;

            if (vcd != null)
            {
                this.Timeline = new VCDTimeline(vcd);
                this.TimeChanger = new BroadcastBlock<Action>(x => x);
                WorkLimiter.LinkSource(TimeChanger);

                SetCircuitState(Timeline.TimeInterval.StartInclusive);
            }

            Dictionary<Output, CombComputeNode> conToCombNode = new Dictionary<Output, CombComputeNode>();
            CombComputeNode[] combNodes = graph.ComputeGraph.GetValueChangingNodes();
            foreach (var combNode in combNodes)
            {
                foreach (var combCon in combNode.GetResponsibleConnections())
                {
                    //if (conToCombNode.ContainsKey(combCon))
                    //{

                    //}
                    //conToCombNode[combCon] = combNode;
                    conToCombNode.Add(combCon, combNode);
                }
            }

            string[] colors = new string[]
            {
                "red",
                "green",
                "blue",
                "orange",
                "purple",
                //"black",
                "brown",
                "teal",
                //"maroon",
                "magenta",
                "grey"
            };
            int colorIndex = 0;
            Dictionary<CombComputeNode, string> combToColor = new Dictionary<CombComputeNode, string>();
            foreach (var combNode in combNodes)
            {
                combToColor.Add(combNode, colors[colorIndex]);
                colorIndex = (colorIndex + 1) % colors.Length;
            }

            foreach (var conCombNode in conToCombNode)
            {
                ConToColor.Add(conCombNode.Key, combToColor[conCombNode.Value]);
            }

            foreach (var combNode in graph.ComputeGraph.GetConstNodes())
            {
                foreach (var con in combNode.GetResponsibleConnections())
                {
                    ConstCons.Add(con);
                }
            }
        }

        public bool IsConnectionConst(Output con)
        {
            lock (ConToColor)
            {
                return ConstCons.Contains(con);
            }
        }

        public string GetConnectionColor(Output con)
        {
            lock (ConToColor)
            {
                if (ConstCons.Contains(con))
                {
                    return "black";
                }
                if (!ConToColor.ContainsKey(con))
                {
                    return "black";
                }
                return ConToColor[con];
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
                List<Output> changedConnections = Graph.SetState(Timeline.GetStateAtTime(time));

                HashSet<ModuleLayout> modulesToReRender = new HashSet<ModuleLayout>();

                //Only rerender the uiNodes that are connected to a connection
                //that changed value. UiNodes are not rerendered here, but they
                //are being allowed to rerender here.
                foreach (var connection in changedConnections)
                {
                    foreach (var node in connection.GetConnectedInputs())
                    {
                        if (node.Node != null &&
                            FIRNodeToModCtrl.TryGetValue(node.Node, out var modCtrl1))
                        {
                            modulesToReRender.Add(modCtrl1);
                        }
                    }
                    if (connection.Node != null &&
                        FIRNodeToModCtrl.TryGetValue(connection.Node, out var modCtrl2))
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
