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
    public abstract class FIRLayout
    {
        protected readonly List<IFIRUINode> UINodes = new List<IFIRUINode>();

        public void AddUINode(IFIRUINode uiNode)
        {
            UINodes.Add(uiNode);
        }

        public void PrepareToRerenderLayout()
        {
            foreach (var uiNode in UINodes)
            {
                uiNode.PrepareForRender();
            }
        }

        public abstract void UpdateComponentInfo(FIRComponentUpdate updateData);
    }

    public class CondLayout : FIRLayout
    {
        private readonly Conditional Cond;
        private readonly Dictionary<FIRRTLNode, Point> ModuleSizes = new Dictionary<FIRRTLNode, Point>();
        private readonly Dictionary<FIRRTLNode, List<DirectedIO>> InputOffsets = new Dictionary<FIRRTLNode, List<DirectedIO>>();
        private readonly Dictionary<FIRRTLNode, List<DirectedIO>> OutputOffsets = new Dictionary<FIRRTLNode, List<DirectedIO>>();
        private Point CondSize = Point.Zero;

        public delegate void LayoutHandler(List<Positioned<Module>> positions, FIRComponentUpdate componentInfo);
        public event LayoutHandler OnLayoutUpdate;

        public CondLayout(Conditional cond)
        {
            this.Cond = cond;
        }

        public bool IsReadyToRender()
        {
            return ModuleSizes.Count == Cond.CondMods.Count;
        }

        private bool UpdateNodeSize(FIRRTLNode node, Point size)
        {
            //Data changed if this node didn't have a previous size
            if (ModuleSizes.TryAdd(node, size))
            {
                return true;
            }

            //Data only changed if old and now are not the same
            Point oldSize = ModuleSizes[node];
            if (oldSize != size)
            {
                ModuleSizes[node] = size;
                return true;
            }

            return false;
        }

        private bool UpdateOffsets(FIRRTLNode node, Dictionary<FIRRTLNode, List<DirectedIO>> nodeOffsets, List<DirectedIO> offsets)
        {
            //Data changed if this node didn't have previous offsets
            if (nodeOffsets.TryAdd(node, offsets))
            {
                return true;
            }

            //Only update if the offsets are different
            List<DirectedIO> oldOffsets = nodeOffsets[node];
            if (oldOffsets.Count != offsets.Count)
            {
                nodeOffsets[node] = offsets;
                return true;
            }

            for (int i = 0; i < oldOffsets.Count; i++)
            {
                if (oldOffsets[i] != offsets[i])
                {
                    nodeOffsets[node] = offsets;
                    return true;
                }
            }

            return false;
        }

        private (List<Positioned<Module>> modPoses, List<DirectedIO> inOffsets, List<DirectedIO> outOffsets) UpdateAndGetModPositions()
        {

            List<DirectedIO> inputOffsets = new List<DirectedIO>();
            List<DirectedIO> outputOffsets = new List<DirectedIO>();
            List<Positioned<Module>> positions = new List<Positioned<Module>>();
            int y = 0;
            foreach (var condMod in Cond.CondMods)
            {
                Module mod = condMod.Mod;
                Point offset = new Point(0, y);
                positions.Add(new Positioned<Module>(offset, mod));

                foreach (var inOffset in InputOffsets[mod])
                {
                    inputOffsets.Add(inOffset.WithOffsetPosition(offset));
                }
                foreach (var outOfset in OutputOffsets[mod])
                {
                    outputOffsets.Add(outOfset.WithOffsetPosition(offset));
                }

                y += ModuleSizes[condMod.Mod].Y;
            }

            CondSize = new Point(ModuleSizes.Values.Max(x => x.X), y);

            return (positions, inputOffsets, outputOffsets);
        }

        public override void UpdateComponentInfo(FIRComponentUpdate updateData)
        {
            lock (ModuleSizes)
            {
                //Keep track of data changes. Layout will only update if there has
                //been a data change and all data is available.
                bool dataChanged = false;

                dataChanged |= UpdateNodeSize(updateData.Node, updateData.Size);
                dataChanged |= UpdateOffsets(updateData.Node, InputOffsets, updateData.InputOffsets);
                dataChanged |= UpdateOffsets(updateData.Node, OutputOffsets, updateData.OutputOffsets);

                if (IsReadyToRender() && dataChanged)
                {
                    var layoutData = UpdateAndGetModPositions();
                    FIRComponentUpdate componentUpdate = new FIRComponentUpdate(Cond, CondSize, layoutData.inOffsets, layoutData.outOffsets);
                    OnLayoutUpdate?.Invoke(layoutData.modPoses, componentUpdate);
                }
            }
        }
    }

    public class ModuleLayout : FIRLayout
    {
        private readonly DebugController DebugCtrl;
        private readonly Module Mod;
        private readonly ModuleUI ModUI;
        private readonly SimpleRouter WireRouter;
        private readonly SimplePlacer NodePlacer;

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
    }
}
