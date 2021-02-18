using FIRRTL;
using System;

namespace ChiselDebug.GraphFIR
{
    public abstract class FIRRTLPrimOP : FIRRTLNode
    {
        public readonly Output Result;

        public FIRRTLPrimOP(IFIRType type)
        {
            this.Result = new Output(type);
        }

        public override Output[] GetOutputs()
        {
            return new Output[] { Result };
        }
    }
}
