using ChiselDebug.GraphFIR.Components;
using ChiselDebug.Routing;
using ChiselDebug.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code
{
    public class FIRComponentUpdate
    {
        public readonly FIRRTLNode Node;
        public readonly Point Size;
        public readonly DirectedIO[] InputOffsets;
        public readonly DirectedIO[] OutputOffsets;

        public FIRComponentUpdate(FIRRTLNode node, Point size, DirectedIO[] inputOffsets, DirectedIO[] outputOffsets)
        {
            this.Node = node;
            this.Size = size;
            this.InputOffsets = inputOffsets;
            this.OutputOffsets = outputOffsets;
        }
    }
}
