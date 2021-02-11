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
        public readonly List<DirectedIO> InputOffsets;
        public readonly List<DirectedIO> OutputOffsets;

        public FIRComponentUpdate(FIRRTLNode node, Point size, List<DirectedIO> inputOffsets, List<DirectedIO> outputOffsets)
        {
            this.Node = node;
            this.Size = size;
            this.InputOffsets = inputOffsets;
            this.OutputOffsets = outputOffsets;
        }
    }
}
