using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;

namespace ChiselDebug.GraphFIR.Components
{
    public sealed class DummySink : FIRRTLNode, INoPlaceAndRoute
    {
        public readonly Sink InIO;

        public DummySink(Source outIO) : base(null)
        {
            InIO = (Sink)outIO.Flip(this);

            outIO.ConnectToInput(InIO);
        }

        public override Sink[] GetSinks()
        {
            return new Sink[] { InIO };
        }

        public override Source[] GetSources()
        {
            return Array.Empty<Source>();
        }

        public override IEnumerable<FIRIO> GetIO()
        {
            yield return InIO;
        }

        public override void Compute()
        {
            InIO.UpdateValueFromSource();
        }

        internal override void InferType()
        {
            InIO.InferType();
        }
    }
}
