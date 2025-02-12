﻿using ChiselDebug.GraphFIR.Components;
using ChiselDebug.Routing;
using ChiselDebug.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code.Controllers
{
    public sealed class CondLayout : FIRLayout
    {
        private readonly Conditional Cond;
        private readonly Dictionary<FIRRTLNode, Point> ModuleSizes = new Dictionary<FIRRTLNode, Point>();
        private readonly Dictionary<FIRRTLNode, DirectedIO[]> InputOffsets = new Dictionary<FIRRTLNode, DirectedIO[]>();
        private readonly Dictionary<FIRRTLNode, DirectedIO[]> OutputOffsets = new Dictionary<FIRRTLNode, DirectedIO[]>();
        private Point CondSize = Point.Zero;

        public delegate Task LayoutHandler(List<Positioned<Module>> positions, FIRComponentUpdate componentInfo);
        public event LayoutHandler? OnLayoutUpdate;

        public CondLayout(Conditional cond)
        {
            Cond = cond;
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

        private bool UpdateOffsets(FIRRTLNode node, Dictionary<FIRRTLNode, DirectedIO[]> nodeOffsets, DirectedIO[] offsets)
        {
            //Data changed if this node didn't have previous offsets
            if (nodeOffsets.TryAdd(node, offsets))
            {
                return true;
            }

            //Only update if the offsets are different
            DirectedIO[] oldOffsets = nodeOffsets[node];
            if (oldOffsets.Length != offsets.Length)
            {
                nodeOffsets[node] = offsets;
                return true;
            }

            for (int i = 0; i < oldOffsets.Length; i++)
            {
                if (oldOffsets[i] != offsets[i])
                {
                    nodeOffsets[node] = offsets;
                    return true;
                }
            }

            return false;
        }

        private (List<Positioned<Module>> modPoses, DirectedIO[] inOffsets, DirectedIO[] outOffsets) UpdateAndGetModPositions()
        {

            List<DirectedIO> inputOffsets = new List<DirectedIO>();
            List<DirectedIO> outputOffsets = new List<DirectedIO>();
            List<Positioned<Module>> positions = new List<Positioned<Module>>();
            int y = 0;
            foreach (var condMod in Cond.CondMods)
            {
                Module mod = condMod;
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

                y += ModuleSizes[condMod].Y;
            }

            CondSize = new Point(ModuleSizes.Values.Max(x => x.X), y);

            return (positions, inputOffsets.ToArray(), outputOffsets.ToArray());
        }

        public override Task UpdateComponentInfoAsync(FIRComponentUpdate updateData)
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
                    return OnLayoutUpdate?.Invoke(layoutData.modPoses, componentUpdate) ?? Task.CompletedTask;
                }
            }

            return Task.CompletedTask;
        }

        public override void UpdateLayoutDisplay(float scaling)
        {
            foreach (var childLayout in ChildLayouts)
            {
                childLayout.UpdateLayoutDisplay(scaling);
            }
        }

        public void StatehasChanged()
        {
            foreach (var uiNode in UINodes)
            {
                uiNode.PrepareForRender();
            }
        }
    }
}
