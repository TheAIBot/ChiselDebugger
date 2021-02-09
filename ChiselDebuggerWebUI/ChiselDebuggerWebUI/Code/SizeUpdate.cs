using ChiselDebug;
using ChiselDebug.FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerWebUI.Code
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
