using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Placing;
using ChiselDebug.Routing;
using ChiselDebug.Utilities;
using ChiselDebuggerRazor.Pages.FIRRTLUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebuggerRazor.Code
{
    public class ModuleLayout : FIRLayout
    {
        private readonly DebugController DebugCtrl;
        private readonly Module Mod;
        private readonly ModuleUI ModUI;
        private readonly SimpleRouter WireRouter;
        private readonly PlacingBase NodePlacer;
        private WiresUI WireUI;

        private readonly FIRRTLNode[] ModuleNodes;
        private readonly FIRRTLNode[] ModuleNodesWithModule;
        private readonly FIRIO[] ModuleIO;

        public delegate void PlacedHandler(PlacementInfo placements);
        private event PlacedHandler OnPlacedNodes;

        public delegate void RoutedHandler(List<WirePath> wirePaths);
        private event RoutedHandler OnWiresRouted;

        public ModuleLayout(DebugController debugCtrl, Module mod, ModuleUI modUI)
        {
            this.DebugCtrl = debugCtrl;
            this.Mod = mod;
            this.ModUI = modUI;
            this.WireRouter = new SimpleRouter(Mod);
            if (OperatingSystem.IsWindows())
            {
                this.NodePlacer = new GraphVizPlacer(Mod);
            }
            else
            {
                this.NodePlacer = new SimplePlacer(Mod);
            }
            this.ModuleNodes = Mod.GetAllNodes();
            this.ModuleNodesWithModule = Mod.GetAllNodesIncludeModule();

            List<FIRIO> allShownIO = new List<FIRIO>();
            allShownIO.AddRange(Mod.GetAllIOOrdered());
            allShownIO.AddRange(allShownIO.Select(x => x.ParentIO).Where(x => x != null).Distinct().ToArray());
            this.ModuleIO = allShownIO.ToArray();

            DebugCtrl.AddModCtrl(Mod.Name, this, ModuleNodes, ModuleNodesWithModule, ModuleIO);
        }

        public override void PrepareToRerenderLayout()
        {
            base.PrepareToRerenderLayout();

            ModUI.PrepareForRender();
            WireUI?.PrepareForRender();
        }

        public void Render()
        {
            ModUI.InvokestateHasChanged();
        }

        public bool IsReadyToRender()
        {
            return NodePlacer.IsReadyToPlace();
        }

        public bool IsEmpty()
        {
            return ModuleNodes.Where(x => x is not INoPlaceAndRoute).Count() == 0;
        }

        public void SubscribeToPlaceTemplate(PlacedHandler onPlaced)
        {
            DebugCtrl.PlaceRouteStats.IncrementNeedToPlace();
            OnPlacedNodes += onPlaced;

            if (DebugCtrl.TryGetPlaceTemplate(Mod.Name, this, out var placement))
            {
                PlaceNodes(placement);
            }
        }

        public void SubscribeToRouteTemplate(RoutedHandler onRouted)
        {
            DebugCtrl.PlaceRouteStats.IncrementNeedToRoute();
            OnWiresRouted += onRouted;

            if (DebugCtrl.TryGetRouteTemplate(Mod.Name, this, out var wires))
            {
                PlaceWires(wires);
            }
        }

        public void PlaceNodes(PlacementInfo placements)
        {
            DebugCtrl.PlaceRouteStats.PlaceDone(this);
            OnPlacedNodes?.Invoke(placements);
        }

        public void PlaceWires(List<WirePath> wires)
        {
            DebugCtrl.PlaceRouteStats.RouteDone(this);
            OnWiresRouted?.Invoke(wires);
        }

        public void RouteWires(PlacementInfo placements)
        {
            if (WireRouter.IsReadyToRoute())
            {
                DebugCtrl.AddRouteTemplateParameters(Mod.Name, WireRouter, placements, ModuleNodesWithModule, ModuleIO);
            }
        }

        public override void UpdateComponentInfo(FIRComponentUpdate updateData)
        {
            WireRouter.UpdateIOFromNode(updateData.Node, updateData.InputOffsets, updateData.OutputOffsets);
            NodePlacer.SetNodeSize(updateData.Node, updateData.Size);

            if (IsReadyToRender())
            {
                DebugCtrl.AddPlaceTemplateParameters(Mod.Name, NodePlacer, ModuleNodes);
            }
        }

        public void UpdateIOFromNode(FIRRTLNode node, DirectedIO[] inputOffsets, DirectedIO[] outputOffsets)
        {
            WireRouter.UpdateIOFromNode(node, inputOffsets, outputOffsets);
        }

        public override void UpdateLayoutDisplay(float scaling)
        {
            Point layoutSize = ModUI.GetModuleSize();
            float actualWidth = layoutSize.X * scaling;
            float actualHeight = layoutSize.Y * scaling;
            //if layout is too small on screen then don't display its
            //content in order to speed up render
            if (actualWidth < 200 && actualHeight < 200)
            {
                ModUI.SetShowModuleName(true);
                return;
            }

            ModUI.SetShowModuleName(false);

            float childScaling = scaling * ModUI.GetContentScaling();
            foreach (var childLayout in ChildLayouts)
            {
                childLayout.UpdateLayoutDisplay(childScaling);
            }
        }

        public void SetWireUI(WiresUI wireUI)
        {
            WireUI = wireUI;
        }
    }
}
