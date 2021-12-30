using ChiselDebug.GraphFIR.Components;
using ChiselDebug.Utilities;

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
