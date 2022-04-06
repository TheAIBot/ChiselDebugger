﻿using ChiselDebug.GraphFIR.Circuit;
using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Placing;
using ChiselDebug.Routing;
using ChiselDebug.Timeline;
using ChiselDebug.Utilities;
using ChiselDebuggerRazor.Code.Templates;
using ChiselDebuggerRazor.Pages.FIRRTLUI.IOUI;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Threading;
using VCDReader;

namespace ChiselDebuggerRazor.Code.Controllers
{
    public class DebugController : IDisposable
    {
        private readonly CircuitGraph Graph;
        public VCDTimeline Timeline { get; set; } = null;
        private readonly Dictionary<FIRRTLNode, ModuleLayout> FIRNodeToModCtrl = new Dictionary<FIRRTLNode, ModuleLayout>();
        private readonly List<ModuleLayout> ModControllers = new List<ModuleLayout>();
        private readonly PlacementTemplator PlacementTemplates;
        private readonly RouteTemplator RouteTemplates;
        private readonly SeqWorkOverrideOld<ulong> StateLimiter;
        private readonly CancellationTokenSource CancelSource = new CancellationTokenSource();
        internal readonly PlaceAndRouteStats PlaceRouteStats = new PlaceAndRouteStats();
        private ModuleLayout RootModCtrl = null;
        private IOWindowUI IOWindow = null;

        public Point CircuitSize { get; private set; } = Point.Zero;

        public delegate void CircuitSizeChangedHandler(Point circuitSize);
        public event CircuitSizeChangedHandler OnCircuitSizeChanged;

        public DebugController(CircuitGraph graph, PlacementTemplator placementTemplates, RouteTemplator routeTemplates, WorkLimiter workLimiter)
        {
            Graph = graph;
            PlacementTemplates = placementTemplates;
            RouteTemplates = routeTemplates;
            StateLimiter = new SeqWorkOverrideOld<ulong>(workLimiter);
        }

        public void AddVCD(VCD vcd)
        {
            Timeline = new VCDTimeline(vcd);

            SetCircuitState(Timeline.TimeInterval.StartInclusive);
        }

        public void AddModCtrl(string moduleName, ModuleLayout modCtrl, FIRRTLNode[] modNodes, FIRRTLNode[] modNodesIncludeMod, FIRIO[] modIO)
        {
            lock (FIRNodeToModCtrl)
            {
                ModControllers.Add(modCtrl);
                foreach (var node in modNodes)
                {
                    FIRNodeToModCtrl.Add(node, modCtrl);
                }
            }

            PlacementTemplates.SubscribeToTemplate(moduleName, modCtrl, modNodes);
            RouteTemplates.SubscribeToTemplate(moduleName, modCtrl, modNodesIncludeMod, modIO);
        }

        internal void AddPlaceTemplateParameters(string moduleName, PlacingBase placer, FIRRTLNode[] nodeOrder)
        {
            PlacementTemplates.AddTemplateParameters(moduleName, placer, nodeOrder, CancelSource.Token);
        }

        internal void AddRouteTemplateParameters(string moduleName, SimpleRouter router, PlacementInfo placeInfo, FIRRTLNode[] nodeOrder, FIRIO[] ioOrder)
        {
            RouteTemplates.AddTemplateParameters(moduleName, router, placeInfo, nodeOrder, ioOrder, CancelSource.Token);
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

        public void SetCircuitState(ulong time)
        {
            CancellationToken cancelToken = CancelSource.Token;
            StateLimiter.AddWork(time, stateTime =>
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

        public void SetIOWindow(IOWindowUI window)
        {
            IOWindow = window;
        }

        public void MouseEntersIO(FIRIO io, MouseEventArgs args)
        {
            IOWindow?.MouseEnter(io, args);
        }

        public void MouseExitIO(FIRIO io, MouseEventArgs args)
        {
            IOWindow?.MouseExit(io, args);
        }

        public void Dispose()
        {
            CancelSource.Cancel();
        }
    }
}
