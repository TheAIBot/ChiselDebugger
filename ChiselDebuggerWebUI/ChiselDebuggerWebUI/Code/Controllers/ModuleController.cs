using ChiselDebug;
using ChiselDebug.GraphFIR;
using ChiselDebug.Routing;
using System;
using System.Collections.Generic;
using static ChiselDebug.SimplePlacer;

namespace ChiselDebuggerWebUI.Code
{
    public class ModuleController
    {
        private readonly Module Mod;
        private readonly ConnectionsHandler ConHandler;
        private readonly SimpleRouter WireRouter;
        private readonly SimplePlacer NodePlacer;

        public event PlacedHandler OnPlacedNodes;

        public delegate void RoutedHandler(List<WirePath> wirePaths);
        public event RoutedHandler OnWiresRouted;


        public ModuleController(Module mod)
        {
            this.Mod = mod;
            this.ConHandler = new ConnectionsHandler(Mod);
            this.WireRouter = new SimpleRouter(ConHandler);
            this.NodePlacer = new SimplePlacer(Mod);
            NodePlacer.OnPlacedNodes += PropagateOnPlacedEvent;
        }

        private void PropagateOnPlacedEvent(PlacementInfo placements)
        {
            OnPlacedNodes?.Invoke(placements);
        }

        public void RouteWires(PlacementInfo placements)
        {
            OnWiresRouted?.Invoke(WireRouter.PathLines(placements));
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
