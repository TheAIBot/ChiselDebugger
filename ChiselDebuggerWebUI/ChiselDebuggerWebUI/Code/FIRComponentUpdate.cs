using ChiselDebug;
using ChiselDebug.FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerWebUI.Code
{
    public class FIRComponentUpdate
    {
        public readonly FIRRTLNode Node;
        public readonly Point Size;
        public readonly List<Positioned<Input>> InputOffsets;
        public readonly List<Positioned<Output>> OutputOffsets;

        public FIRComponentUpdate(FIRRTLNode node, Point size, List<Positioned<Input>> inputOffsets, List<Positioned<Output>> outputOffsets)
        {
            this.Node = node;
            this.Size = size;
            this.InputOffsets = inputOffsets;
            this.OutputOffsets = outputOffsets;
        }
    }
}
