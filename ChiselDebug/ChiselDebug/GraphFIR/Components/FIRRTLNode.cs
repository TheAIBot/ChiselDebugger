using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System.Collections.Generic;
#nullable enable

namespace ChiselDebug.GraphFIR.Components
{
    public abstract class FIRRTLNode
    {
        public readonly IFirrtlNode? FirDefNode;
        public Module? ResideIn { get; private set; }

        public FIRRTLNode(IFirrtlNode? defNode)
        {
            FirDefNode = defNode;
        }

        public void SetModResideIn(Module? mod)
        {
            ResideIn = mod;
        }

        public void Disconnect()
        {
            foreach (var input in GetSinks())
            {
                input.DisconnectAll();
            }

            foreach (var output in GetSources())
            {
                output.DisconnectAll();
            }
        }

        public abstract Sink[] GetSinks();
        public abstract Source[] GetSources();
        public abstract IEnumerable<FIRIO> GetIO();

        public virtual IEnumerable<FIRIO> GetVisibleIO()
        {
            yield break;
        }

        public abstract void Compute();

        internal abstract void InferType();
    }
}
