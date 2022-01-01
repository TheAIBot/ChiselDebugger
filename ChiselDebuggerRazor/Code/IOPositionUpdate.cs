using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Utilities;
using System.Collections.Generic;

namespace ChiselDebuggerRazor.Code
{
    public class IOPositionUpdate
    {
        public readonly FIRRTLNode Node;
        public readonly List<Positioned<Sink>> Inputs;
        public readonly List<Positioned<Source>> Outputs;

        public IOPositionUpdate(FIRRTLNode node, List<Positioned<Sink>> inputs, List<Positioned<Source>> outputs)
        {
            this.Node = node;
            this.Inputs = inputs;
            this.Outputs = outputs;
        }
    }
}
