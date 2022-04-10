using ChiselDebug.GraphFIR.Circuit;
using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Placing;
using ChiselDebug.Routing;
using ChiselDebug.Timeline;
using ChiselDebug.Utilities;
using ChiselDebuggerRazor.Code.Templates;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VCDReader;

namespace ChiselDebuggerRazor.Code.Controllers
{
    public sealed class DebugController : IDisposable
    {
        public readonly CircuitGraph Graph;
        public VCDTimeline Timeline { get; set; } = null;
        private readonly Dictionary<FIRRTLNode, ModuleLayout> FIRNodeToModCtrl = new Dictionary<FIRRTLNode, ModuleLayout>();
        private readonly List<ModuleLayout> ModControllers = new List<ModuleLayout>();
        private readonly PlacementTemplator PlacementTemplates;
        private readonly RouteTemplator RouteTemplates;
        private readonly SeqWorkOverrideOld<ulong> StateLimiter;
        private readonly CancellationTokenSource CancelSource = new CancellationTokenSource();
        internal readonly PlaceAndRouteStats PlaceRouteStats = new PlaceAndRouteStats();
        private ModuleLayout RootModCtrl = null;

        public Point CircuitSize { get; private set; } = Point.Zero;

        public delegate void CircuitSizeChangedHandler(Point circuitSize);
        public event CircuitSizeChangedHandler OnCircuitSizeChanged;

        public DebugController(CircuitGraph graph, PlacementTemplator placementTemplates, RouteTemplator routeTemplates, SeqWorkOverrideOld<ulong> stateLimiter)
        {
            Graph = graph;
            PlacementTemplates = placementTemplates;
            RouteTemplates = routeTemplates;
            StateLimiter = stateLimiter;
        }

        public Task AddVCD(VCD vcd)
        {
            Timeline = new VCDTimeline(vcd);

            return SetCircuitState(Timeline.TimeInterval.StartInclusive);
        }

        public async Task AddModCtrl(string moduleName, ModuleLayout modCtrl, FIRRTLNode[] modNodes, FIRRTLNode[] modNodesIncludeMod, FIRIO[] modIO)
        {
            lock (FIRNodeToModCtrl)
            {
                ModControllers.Add(modCtrl);
                foreach (var node in modNodes)
                {
                    FIRNodeToModCtrl.Add(node, modCtrl);
                }
            }

            await PlacementTemplates.SubscribeToTemplate(moduleName, modCtrl, modNodes);
            await RouteTemplates.SubscribeToTemplate(moduleName, modCtrl, modNodesIncludeMod, modIO);
        }

        internal Task AddPlaceTemplateParameters(string moduleName, INodePlacer placer, FIRRTLNode[] nodeOrder)
        {
            return PlacementTemplates.AddTemplateParameters(moduleName, placer, nodeOrder, CancelSource.Token);
        }

        internal Task AddRouteTemplateParameters(string moduleName, SimpleRouter router, PlacementInfo placeInfo, FIRRTLNode[] nodeOrder, FIRIO[] ioOrder)
        {
            return RouteTemplates.AddTemplateParameters(moduleName, router, placeInfo, nodeOrder, ioOrder, CancelSource.Token);
        }

        internal bool TryGetPlaceTemplate(string moduleName, ModuleLayout modLayout, out PlacementInfo placement)
        {
            return PlacementTemplates.TryGetTemplate(moduleName, modLayout, out placement);
        }

        internal bool TryGetRouteTemplate(string moduleName, ModuleLayout modLayout, out List<WirePath> wires)
        {
            return RouteTemplates.TryGetTemplate(moduleName, modLayout, out wires);
        }

        public void SetCircuitSize(Point size, ModuleLayout rootModCtrl)
        {
            CircuitSize = size;
            RootModCtrl = rootModCtrl;
            OnCircuitSizeChanged?.Invoke(CircuitSize);
        }

        public Task SetCircuitState(ulong time)
        {
            CancellationToken cancelToken = CancelSource.Token;
            return StateLimiter.AddWork(time, stateTime =>
            {
                Graph.SetState(Timeline.GetStateAtTime(stateTime));
                Graph.ComputeRemainingGraph();
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }

                if (RootModCtrl != null)
                {
                    foreach (var modUI in ModControllers)
                    {
                        modUI.PrepareToRerenderLayout();
                    }

                    RootModCtrl.Render();
                }
            });
        }

        public void Dispose()
        {
            CancelSource.Cancel();
            CancelSource.Dispose();
        }
    }
}
