using ChiselDebug;
using ChiselDebug.GraphFIR;
using ChiselDebug.Routing;
using System;
using System.Collections.Generic;

namespace ChiselDebuggerWebUI.Code
{
    public class ModuleController
    {
        private readonly Module Mod;
        private readonly ConnectionsHandler ConHandler;
        public readonly SimpleRouter WireRouter;
        private readonly Placer NodePlacer;


        public ModuleController(Module mod, Action<PlacementInfo> onPlacedNodes)
        {
            this.Mod = mod;
            this.ConHandler = new ConnectionsHandler(Mod);
            this.WireRouter = new SimpleRouter(ConHandler);
            this.NodePlacer = new Placer(Mod);
            NodePlacer.OnPlacedNodes += x => onPlacedNodes(x);
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
