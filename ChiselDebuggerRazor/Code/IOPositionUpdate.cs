using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Utilities;
using System.Collections.Generic;

namespace ChiselDebuggerRazor.Code
{
    public class IOPositionUpdate
    {
        public readonly FIRRTLNode Node;
        public readonly List<Positioned<Input>> Inputs;
        public readonly List<Positioned<Output>> Outputs;

        public IOPositionUpdate(FIRRTLNode node, List<Positioned<Input>> inputs, List<Positioned<Output>> outputs)
        {
            this.Node = node;
            this.Inputs = inputs;
            this.Outputs = outputs;
        }
    }
}
