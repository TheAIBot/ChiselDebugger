﻿using ChiselDebug.GraphFIR.Components;
using ChiselDebug.Routing;
using ChiselDebug.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ChiselDebug.Placing
{
    public abstract class PlacingBase : INodePlacer
    {
        private readonly Module Mod;
        private readonly HashSet<FIRRTLNode> MissingNodeDims;
        private readonly Dictionary<FIRRTLNode, Point> SizeChanges = new Dictionary<FIRRTLNode, Point>();
        private readonly Dictionary<FIRRTLNode, Point> NodeSizes = new Dictionary<FIRRTLNode, Point>();

        private readonly Dictionary<FIRRTLNode, DirectedIO[]> NodeInputOffsets = new Dictionary<FIRRTLNode, DirectedIO[]>();
        private readonly Dictionary<FIRRTLNode, DirectedIO[]> NodeOutputOffsets = new Dictionary<FIRRTLNode, DirectedIO[]>();

        public PlacingBase(Module mod)
        {
            Mod = mod;
            MissingNodeDims = new HashSet<FIRRTLNode>(Mod.GetAllNodes());
            MissingNodeDims.RemoveWhere(x => x is INoPlaceAndRoute);
        }

        public void SetNodeSize(FIRRTLNode node, Point size, DirectedIO[] inputOffsets, DirectedIO[] outputOffsets)
        {
            lock (SizeChanges)
            {
                //If the size hasn't changed then there is no need to
                //do anything at all as the result will be the same
                if (NodeSizes.TryGetValue(node, out var oldSize) && oldSize == size)
                {
                    return;
                }

                SizeChanges[node] = size;
                MissingNodeDims.Remove(node);

                NodeInputOffsets[node] = inputOffsets;
                NodeOutputOffsets[node] = outputOffsets;
            }
        }

        public bool IsReadyToPlace()
        {
            return MissingNodeDims.Count == 0 && SizeChanges.Count > 0;
        }

        public PlacementInfo PositionModuleComponents()
        {
            lock (NodeSizes)
            {
                lock (SizeChanges)
                {
                    foreach (var change in SizeChanges)
                    {
                        NodeSizes[change.Key] = change.Value;
                    }

                    //Changes transferred, now there are no more changed left
                    SizeChanges.Clear();
                }

                try
                {
                    return PositionComponents(NodeSizes, Mod, NodeInputOffsets, NodeOutputOffsets);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    Debug.WriteLine(e.StackTrace);
                    throw;
                }
            }
        }

        protected abstract PlacementInfo PositionComponents(Dictionary<FIRRTLNode, Point> nodeSizes,
                                                            Module mod,
                                                            Dictionary<FIRRTLNode, DirectedIO[]> nodeInputOffsets,
                                                            Dictionary<FIRRTLNode, DirectedIO[]> nodeOutputOffsets);
    }
}
