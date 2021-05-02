using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System.Collections.Generic;

namespace ChiselDebug.GraphFIR
{
    public abstract class FIRRTLNode
    {
        public readonly FirrtlNode FirDefNode;
        public Module ResideIn { get; private set; }

        public FIRRTLNode(FirrtlNode defNode)
        {
            this.FirDefNode = defNode;
        }

        public void SetModResideIn(Module mod)
        {
            ResideIn = mod;
        }

        public abstract Input[] GetInputs();
        public abstract Output[] GetOutputs();

        public abstract IEnumerable<FIRIO> GetIO();

        public abstract void Compute();

        internal abstract void InferType();
    }
}
