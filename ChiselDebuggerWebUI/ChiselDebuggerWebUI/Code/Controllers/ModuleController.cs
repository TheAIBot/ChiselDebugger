using ChiselDebug;
using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Routing;
using ChiselDebuggerWebUI.Pages.FIRRTLUI;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebuggerWebUI.Code
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
            this.NodePlacer = new SimplePlacer(Mod);
            //this.NodePlacer = new GraphVizPlacer(Mod);
            this.ModuleNodes = Mod.GetAllNodes();
            this.ModuleNodesWithModule = Mod.GetAllNodesIncludeModule();
            this.ModuleIO = Mod.GetAllIOOrdered();

            DebugCtrl.AddModCtrl(Mod.Name, this, ModuleNodes, ModuleNodesWithModule, ModuleIO);
        }

        public void RerenderWithoutPreparation()
        {
            foreach (var uiNode in UINodes)
            {
                uiNode.PrepareForRender();
            }

            WireUI?.PrepareForRender();
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
            OnPlacedNodes += onPlaced;

            if (DebugCtrl.TryGetPlaceTemplate(Mod.Name, this, out var placement))
            {
                PlaceNodes(placement);
            }
        }

        public void SubscribeToRouteTemplate(RoutedHandler onRouted)
        {
            OnWiresRouted += onRouted;

            if (DebugCtrl.TryGetRouteTemplate(Mod.Name, this, out var wires))
            {
                PlaceWires(wires);
            }
        }

        public void PlaceNodes(PlacementInfo placements)
        {
            OnPlacedNodes?.Invoke(placements);
        }

        public void PlaceWires(List<WirePath> wires)
        {
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

        public void UpdateIOFromNode(FIRRTLNode node, List<DirectedIO> inputOffsets, List<DirectedIO> outputOffsets)
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
