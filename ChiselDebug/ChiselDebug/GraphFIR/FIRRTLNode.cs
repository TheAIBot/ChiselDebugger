using ChiselDebug.GraphFIR.IO;
using FIRRTL;

namespace ChiselDebug.GraphFIR
{
    public abstract class FIRRTLNode
    {
        public readonly FirrtlNode FirDefNode;

        public FIRRTLNode(FirrtlNode defNode)
        {
            this.FirDefNode = defNode;
        }

        public abstract ScalarIO[] GetInputs();
        public abstract ScalarIO[] GetOutputs();

        public abstract FIRIO[] GetIO();

        public abstract void InferType();
    }
}
