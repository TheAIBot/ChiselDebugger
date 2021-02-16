using ChiselDebug.GraphFIR;

namespace ChiselDebug
{
    internal class IOInfo
    {
        internal readonly FIRRTLNode Node;
        internal readonly DirectedIO DirIO;

        public IOInfo(FIRRTLNode node, DirectedIO dirIO)
        {
            this.Node = node;
            this.DirIO = dirIO;
        }
    }
}
