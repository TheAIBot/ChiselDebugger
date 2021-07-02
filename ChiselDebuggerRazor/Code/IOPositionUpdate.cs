using ChiselDebug;
using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
