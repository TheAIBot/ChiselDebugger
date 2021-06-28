using ChiselDebug;
using ChiselDebug.GraphFIR;
using ChiselDebuggerRazor.Components;
using ChiselDebuggerRazor.Pages.FIRRTLUI;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebuggerRazor.Code
{
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

        public override void UpdateLayoutDisplay(float scaling)
        {
            foreach (var childLayout in ChildLayouts)
            {
                childLayout.UpdateLayoutDisplay(scaling);
            }
        }
    }
}
