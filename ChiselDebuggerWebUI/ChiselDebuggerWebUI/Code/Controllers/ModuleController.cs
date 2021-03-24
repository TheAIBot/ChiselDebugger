using ChiselDebug;
using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Routing;
using ChiselDebuggerWebUI.Components;
using ChiselDebuggerWebUI.Pages.FIRRTLUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using static ChiselDebug.SimplePlacer;

namespace ChiselDebuggerWebUI.Code
{
    public class ModuleController : IDisposable
    {
        private readonly DebugController DebugCtrl;
        private readonly Module Mod;
        private readonly ModuleUI ModUI;
        private readonly ModuleController ParentModCtrl;
        private readonly SimpleRouter WireRouter;
        private readonly SimplePlacer NodePlacer;
        private readonly List<IFIRUINode> UINodes = new List<IFIRUINode>();
        private readonly FIRRTLNode[] ModuleNodes;
        private readonly FIRRTLNode[] ModuleNodesWithModule;
        private readonly FIRIO[] ModuleIO;

        public delegate void PlacedHandler(PlacementInfo placements);
        public event PlacedHandler OnPlacedNodes;

        public delegate void RoutedHandler(List<WirePath> wirePaths);
        public event RoutedHandler OnWiresRouted;

        public ModuleController(DebugController debugCtrl, Module mod, ModuleUI modUI, ModuleController parentModCtrl)
        {
            this.DebugCtrl = debugCtrl;
            this.Mod = mod;
            this.ModUI = modUI;
            this.ParentModCtrl = parentModCtrl;
            this.WireRouter = new SimpleRouter(Mod);
            this.NodePlacer = new SimplePlacer(Mod);
            this.ModuleNodes = Mod.GetAllNodes();
            this.ModuleNodesWithModule = Mod.GetAllNodesIncludeModule();
            this.ModuleIO = Mod.GetAllIOOrdered();

            DebugCtrl.AddModCtrl(Mod.Name, this, ModuleNodes, ModuleNodesWithModule, ModuleIO);
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

        public void UpdateComponentInfo(FIRComponentUpdate updateData)
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

        public void Dispose()
        {
        }
    }
}
