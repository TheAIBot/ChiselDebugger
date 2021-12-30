using ChiselDebug.GraphFIR.IO;
using FIRRTL;

namespace ChiselDebug.GraphFIR.Components
{
    public abstract class FIRRTLPrimOP : FIRRTLNode
    {
        public readonly Output Result;

        public FIRRTLPrimOP(IFIRType type, FirrtlNode defNode) : base(defNode)
        {
            Result = new Output(this, null, type);
        }

        public override Output[] GetOutputs()
        {
            return new Output[] { Result };
        }
    }
}
