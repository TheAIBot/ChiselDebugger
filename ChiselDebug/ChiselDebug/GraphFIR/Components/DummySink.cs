using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;

namespace ChiselDebug.GraphFIR.Components
{
    public sealed class DummySink : FIRRTLNode, INoPlaceAndRoute
    {
        public readonly Input InIO;

        public DummySink(Output outIO) : base(null)
        {
            InIO = (Input)outIO.Flip(this);

            outIO.ConnectToInput(InIO);
        }

        public override Input[] GetInputs()
        {
            return new Input[] { InIO };
        }

        public override Output[] GetOutputs()
        {
            return Array.Empty<Output>();
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
