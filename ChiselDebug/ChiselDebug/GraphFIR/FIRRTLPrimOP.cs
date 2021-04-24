using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;

namespace ChiselDebug.GraphFIR
{
    public abstract class FIRRTLPrimOP : FIRRTLNode
    {
        public readonly Output Result;

        public FIRRTLPrimOP(IFIRType type, FirrtlNode defNode) : base(defNode)
        {
            this.Result = new Output(this, null, type);
        }

        public override Output[] GetOutputs()
        {
            return new Output[] { Result };
        }
    }
}
