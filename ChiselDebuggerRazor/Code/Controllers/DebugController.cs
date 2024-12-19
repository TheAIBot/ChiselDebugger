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
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using VCDReader;

namespace ChiselDebuggerRazor.Code.Controllers
{
    public sealed class DebugController : IDisposable
    {
        public readonly CircuitGraph Graph;
        public VCDTimeline? Timeline { get; set; } = null;
        private readonly Dictionary<FIRRTLNode, ModuleLayout> FIRNodeToModCtrl = new Dictionary<FIRRTLNode, ModuleLayout>();
        private readonly List<ModuleLayout> ModControllers = new List<ModuleLayout>();
        private readonly PlacementTemplator PlacementTemplates;
        private readonly RouteTemplator RouteTemplates;
        private readonly SeqWorkOverrideOld<ulong> StateLimiter;
        private readonly CancellationTokenSource CancelSource = new CancellationTokenSource();
        internal readonly PlaceAndRouteStats PlaceRouteStats = new PlaceAndRouteStats();
        private ModuleLayout? RootModCtrl = null;

        public Point CircuitSize { get; private set; } = Point.Zero;

        public delegate Task CircuitSizeChangedHandler(Point circuitSize);
        public event CircuitSizeChangedHandler? OnCircuitSizeChanged;

        public DebugController(CircuitGraph graph, PlacementTemplator placementTemplates, RouteTemplator routeTemplates, SeqWorkOverrideOld<ulong> stateLimiter)
        {
            Graph = graph;
            PlacementTemplates = placementTemplates;
            RouteTemplates = routeTemplates;
            StateLimiter = stateLimiter;
        }

        public Task AddVCDAsync(VCD vcd)
        {
            Timeline = new VCDTimeline(vcd);

            return SetCircuitStateAsync(Timeline.TimeInterval.StartInclusive);
        }

        public async Task AddModCtrlAsync(string moduleName, ModuleLayout modCtrl, FIRRTLNode[] modNodes, FIRRTLNode[] modNodesIncludeMod, FIRIO[] modIO)
        {
            lock (FIRNodeToModCtrl)
            {
                ModControllers.Add(modCtrl);
                foreach (var node in modNodes)
                {
                    FIRNodeToModCtrl.Add(node, modCtrl);
                }
            }

            await PlacementTemplates.SubscribeToTemplateAsync(moduleName, modCtrl, modNodes);
            await RouteTemplates.SubscribeToTemplateAsync(moduleName, modCtrl, modNodesIncludeMod, modIO);
        }

        internal Task AddPlaceTemplateParametersAsync(string moduleName, INodePlacer placer, FIRRTLNode[] nodeOrder)
        {
            return PlacementTemplates.AddTemplateParametersAsync(moduleName, placer, nodeOrder, CancelSource.Token);
        }

        internal Task AddRouteTemplateParametersAsync(string moduleName, SimpleRouter router, PlacementInfo placeInfo, FIRRTLNode[] nodeOrder, FIRIO[] ioOrder)
        {
            return RouteTemplates.AddTemplateParametersAsync(moduleName, router, placeInfo, nodeOrder, ioOrder, CancelSource.Token);
        }

        internal bool TryGetPlaceTemplate(string moduleName, ModuleLayout modLayout, [NotNullWhen(true)] out PlacementInfo? placement)
        {
            return PlacementTemplates.TryGetTemplate(moduleName, modLayout, out placement);
        }

        internal bool TryGetRouteTemplate(string moduleName, ModuleLayout modLayout, [NotNullWhen(true)] out List<WirePath>? wires)
        {
            return RouteTemplates.TryGetTemplate(moduleName, modLayout, out wires);
        }

        public Task SetCircuitSizeAsync(Point size, ModuleLayout rootModCtrl)
        {
            CircuitSize = size;
            RootModCtrl = rootModCtrl;
            return OnCircuitSizeChanged?.Invoke(CircuitSize) ?? Task.CompletedTask;
        }

        public Task SetCircuitStateAsync(ulong time)
        {
            var timeline = Timeline;
            if (timeline == null)
            {
                throw new InvalidOperationException("Attempting to set time of a null timeline");
            }

            CancellationToken cancelToken = CancelSource.Token;
            return StateLimiter.AddWorkAsync(time, async stateTime =>
            {
                Graph.SetState(timeline.GetStateAtTime(stateTime));
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

                    await RootModCtrl.RenderAsync();
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
