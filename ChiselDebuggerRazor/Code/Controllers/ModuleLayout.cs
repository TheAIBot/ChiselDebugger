using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Placing;
using ChiselDebug.Routing;
using ChiselDebug.Utilities;
using ChiselDebuggerRazor.Pages.FIRRTLUI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code.Controllers
{
    public sealed class ModuleLayout : FIRLayout
    {
        private readonly DebugController DebugCtrl;
        private readonly Module Mod;
        private readonly ModuleUI ModUI;
        private readonly INodePlacer NodePlacer;
        private readonly SimpleRouter WireRouter;
        private WiresUI WireUI;

        private readonly FIRRTLNode[] ModuleNodes;
        private readonly FIRRTLNode[] ModuleNodesWithModule;
        private readonly FIRIO[] ModuleIO;

        public delegate Task PlacedHandler(PlacementInfo placements);
        private event PlacedHandler OnPlacedNodes;

        public delegate Task RoutedHandler(List<WirePath> wirePaths);
        private event RoutedHandler OnWiresRouted;

        public ModuleLayout(DebugController debugCtrl, Module mod, ModuleUI modUI, INodePlacer nodePlacer)
        {
            DebugCtrl = debugCtrl;
            Mod = mod;
            ModUI = modUI;
            NodePlacer = nodePlacer;
            WireRouter = new SimpleRouter(Mod);
            ModuleNodes = Mod.GetAllNodes();
            ModuleNodesWithModule = Mod.GetAllNodesIncludeModule();

            List<FIRIO> allShownIO = new List<FIRIO>();
            allShownIO.AddRange(Mod.GetAllIOOrdered());
            allShownIO.AddRange(allShownIO.Select(x => x.ParentIO).Where(x => x != null).Distinct().ToArray());
            ModuleIO = allShownIO.ToArray();
        }

        public Task ConnectToDebugControllerAsync()
        {
            return DebugCtrl.AddModCtrlAsync(Mod.Name, this, ModuleNodes, ModuleNodesWithModule, ModuleIO);
        }

        public override void PrepareToRerenderLayout()
        {
            base.PrepareToRerenderLayout();

            ModUI.PrepareForRender();
            WireUI?.PrepareForRender();
        }

        public Task RenderAsync()
        {
            return ModUI.InvokeStateHasChangedAsync();
        }

        public bool IsReadyToRender()
        {
            return NodePlacer.IsReadyToPlace();
        }

        public bool IsEmpty()
        {
            return !ModuleNodes.Where(x => x is not INoPlaceAndRoute).Any();
        }

        public Task SubscribeToPlaceTemplateAsync(PlacedHandler onPlaced)
        {
            DebugCtrl.PlaceRouteStats.IncrementNeedToPlace();
            OnPlacedNodes += onPlaced;

            if (DebugCtrl.TryGetPlaceTemplate(Mod.Name, this, out var placement))
            {
                return PlaceNodesAsync(placement);
            }

            return Task.CompletedTask;
        }

        public Task SubscribeToRouteTemplateAsync(RoutedHandler onRouted)
        {
            DebugCtrl.PlaceRouteStats.IncrementNeedToRoute();
            OnWiresRouted += onRouted;

            if (DebugCtrl.TryGetRouteTemplate(Mod.Name, this, out var wires))
            {
                return PlaceWiresAsync(wires);
            }

            return Task.CompletedTask;
        }

        public Task PlaceNodesAsync(PlacementInfo placements)
        {
            DebugCtrl.PlaceRouteStats.PlaceDone(this);
            return OnPlacedNodes?.Invoke(placements) ?? Task.CompletedTask;
        }

        public Task PlaceWiresAsync(List<WirePath> wires)
        {
            DebugCtrl.PlaceRouteStats.RouteDone(this);
            return OnWiresRouted?.Invoke(wires) ?? Task.CompletedTask;
        }

        public Task RouteWiresAsync(PlacementInfo placements)
        {
            if (WireRouter.IsReadyToRoute())
            {
                return DebugCtrl.AddRouteTemplateParametersAsync(Mod.Name, WireRouter, placements, ModuleNodesWithModule, ModuleIO);
            }

            return Task.CompletedTask;
        }

        public override Task UpdateComponentInfoAsync(FIRComponentUpdate updateData)
        {
            WireRouter.UpdateIOFromNode(updateData.Node, updateData.InputOffsets, updateData.OutputOffsets);
            NodePlacer.SetNodeSize(updateData.Node, updateData.Size, updateData.InputOffsets, updateData.OutputOffsets);

            if (IsReadyToRender())
            {
                return DebugCtrl.AddPlaceTemplateParametersAsync(Mod.Name, NodePlacer, ModuleNodes);
            }

            return Task.CompletedTask;
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
