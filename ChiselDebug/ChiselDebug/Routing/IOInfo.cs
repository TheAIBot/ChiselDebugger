using ChiselDebug.GraphFIR.Components;

namespace ChiselDebug.Routing
{
    internal class IOInfo
    {
        internal readonly FIRRTLNode Node;
        internal readonly DirectedIO DirIO;

        public IOInfo(FIRRTLNode node, DirectedIO dirIO)
        {
            Node = node;
            DirIO = dirIO;
        }
    }
}
