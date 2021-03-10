using ChiselDebug;
using ChiselDebug.GraphFIR;
using ChiselDebug.Routing;
using ChiselDebuggerWebUI.Components;
using ChiselDebuggerWebUI.Pages.FIRRTLUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static ChiselDebug.SimplePlacer;

namespace ChiselDebuggerWebUI.Code
{
    public class ModuleController
    {
        private readonly Module Mod;
        private readonly ModuleUI ModUI;
        private readonly ModuleController ParentModCtrl;
        private readonly ConnectionsHandler ConHandler;
        private readonly SimpleRouter WireRouter;
        private readonly SimplePlacer NodePlacer;
        private readonly List<IFIRUINode> UINodes = new List<IFIRUINode>();
        private readonly HashSet<Module> NotRenderedYet = new HashSet<Module>();

        public delegate void PlacedHandler(PlacementInfo placements);
        public event PlacedHandler OnPlacedNodes;

        public delegate void RoutedHandler(List<WirePath> wirePaths);
        public event RoutedHandler OnWiresRouted;

        private readonly ExecuteOnlyLatest<bool> PlaceLimiter = new ExecuteOnlyLatest<bool>();
        private readonly ExecuteOnlyLatest<PlacementInfo> RouteLimiter = new ExecuteOnlyLatest<PlacementInfo>();


        public ModuleController(Module mod, ModuleUI modUI, ModuleController parentModCtrl)
        {
            this.Mod = mod;
            this.ModUI = modUI;
            this.ParentModCtrl = parentModCtrl;
            this.ConHandler = new ConnectionsHandler(Mod);
            this.WireRouter = new SimpleRouter(ConHandler);
            this.NodePlacer = new SimplePlacer(Mod);
            NodePlacer.OnReadyToPlaceNodes += PlaceNodes;

            List<Module> nestedModules = mod.GetAllNodes().Where(x => x is Module).Cast<Module>().ToList();
            foreach (var nested in nestedModules)
            {
                NotRenderedYet.Add(nested);
            }

            PlaceLimiter.Start(_ => OnPlacedNodes?.Invoke(NodePlacer.PositionModuleComponents()));
            RouteLimiter.Start(placements => OnWiresRouted?.Invoke(WireRouter.PathLines(placements)));
        }

        public void AddUINode(IFIRUINode uiNode)
        {
            UINodes.Add(uiNode);
        }

        private void ChildCtrlRendered(Module childMod)
        {
            NotRenderedYet.Remove(childMod);
            PlaceNodes();
        }

        public void RerenderModule()
        {
            foreach (var uiNode in UINodes)
            {
                uiNode.PrepareForRender();
            }

            ParentModCtrl?.ChildCtrlRendered(Mod);
        }

        public void RerenderWithoutPreparation()
        {
            foreach (var uiNode in UINodes)
            {
                uiNode.PrepareForRender();
            }

            ModUI.InvokestateHasChanged();
        }

        private void PlaceNodes()
        {
            if (NotRenderedYet.Count > 0)
            {
                return;
            }
            PlaceLimiter.AddWork(true);
        }

        public void RouteWires(PlacementInfo placements)
        {
            RouteLimiter.AddWork(placements);
        }

        public void UpdateComponentInfo(FIRComponentUpdate updateData)
        {
            ConHandler.UpdateIOFromNode(updateData.Node, updateData.InputOffsets, updateData.OutputOffsets);
            NodePlacer.SetNodeSize(updateData.Node, updateData.Size);
        }

        public void UpdateIOFromNode(FIRRTLNode node, List<DirectedIO> inputOffsets, List<DirectedIO> outputOffsets)
        {
            ConHandler.UpdateIOFromNode(node, inputOffsets, outputOffsets);
        }
    }
}
