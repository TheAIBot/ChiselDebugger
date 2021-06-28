using ChiselDebug;
using ChiselDebug.GraphFIR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerRazor.Code
{
    public class SizeUpdate
    {
        public readonly FIRRTLNode Node;
        public readonly Point Size;

        public SizeUpdate(FIRRTLNode node, Point size)
        {
            this.Node = node;
            this.Size = size;
        }
    }
}
