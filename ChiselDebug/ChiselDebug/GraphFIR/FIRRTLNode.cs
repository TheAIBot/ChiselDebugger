using FIRRTL;

namespace ChiselDebug.GraphFIR
{
    public abstract class FIRRTLNode
    {
        public abstract Input[] GetInputs();
        public abstract Output[] GetOutputs();

        public abstract void InferType();
    }
}
