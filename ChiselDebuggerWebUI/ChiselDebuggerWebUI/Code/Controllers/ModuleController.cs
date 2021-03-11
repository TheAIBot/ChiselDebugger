using ChiselDebug;
using ChiselDebug.GraphFIR;
using ChiselDebug.Routing;
using ChiselDebuggerWebUI.Components;
using ChiselDebuggerWebUI.Pages.FIRRTLUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using static ChiselDebug.SimplePlacer;

namespace ChiselDebuggerWebUI.Code
{
    public class ModuleController : IDisposable
    {
        private readonly Module Mod;
        private readonly ModuleUI ModUI;
        private readonly ModuleController ParentModCtrl;
        private readonly SimpleRouter WireRouter;
        private readonly SimplePlacer NodePlacer;
        private readonly List<IFIRUINode> UINodes = new List<IFIRUINode>();

        public delegate void PlacedHandler(PlacementInfo placements);
        public event PlacedHandler OnPlacedNodes;

        public delegate void RoutedHandler(List<WirePath> wirePaths);
        public event RoutedHandler OnWiresRouted;

        private readonly BroadcastBlock<Action> PlaceLimiter = new BroadcastBlock<Action>(x => x);
        private readonly BroadcastBlock<Action> RouteLimiter = new BroadcastBlock<Action>(x => x);

        public ModuleController(Module mod, ModuleUI modUI, ModuleController parentModCtrl)
        {
            this.Mod = mod;
            this.ModUI = modUI;
            this.ParentModCtrl = parentModCtrl;
            this.WireRouter = new SimpleRouter(Mod);
            this.NodePlacer = new SimplePlacer(Mod);

            WorkLimiter.LinkSource(PlaceLimiter);
            WorkLimiter.LinkSource(RouteLimiter);
        }

        public void AddUINode(IFIRUINode uiNode)
        {
            UINodes.Add(uiNode);
        }

        public void PrepareToRerenderModule()
        {
            foreach (var uiNode in UINodes)
            {
                uiNode.PrepareForRender();
            }
        }

        public void RerenderWithoutPreparation()
        {
            foreach (var uiNode in UINodes)
            {
                uiNode.PrepareForRender();
            }

            ModUI.InvokestateHasChanged();
        }

        public bool IsReadyToRender()
        {
            return NodePlacer.IsReadyToPlace();
        }

        private void PlaceNodes()
        {
            PlaceLimiter.Post(() => OnPlacedNodes?.Invoke(NodePlacer.PositionModuleComponents()));
        }

        public void RouteWires(PlacementInfo placements)
        {
            RouteLimiter.Post(() => OnWiresRouted?.Invoke(WireRouter.PathLines(placements)));
        }



        public void UpdateComponentInfo(FIRComponentUpdate updateData)
        {
            WireRouter.UpdateIOFromNode(updateData.Node, updateData.InputOffsets, updateData.OutputOffsets);
            NodePlacer.SetNodeSize(updateData.Node, updateData.Size);

            if (IsReadyToRender())
            {
                PlaceNodes();
            }
        }

        public void UpdateIOFromNode(FIRRTLNode node, List<DirectedIO> inputOffsets, List<DirectedIO> outputOffsets)
        {
            WireRouter.UpdateIOFromNode(node, inputOffsets, outputOffsets);
        }

        public void Dispose()
        {
            WorkLimiter.UnlinkSource(PlaceLimiter);
            WorkLimiter.UnlinkSource(RouteLimiter);
        }
    }
}
