using ChiselDebug.GraphFIR.IO;
using System;
using System.Linq;

namespace ChiselDebug.GraphFIR
{
    public class DummySink : FIRRTLNode, INoPlaceAndRoute
    {
        public readonly Input InIO;

        public DummySink(Output outIO) : base(null)
        {
            this.InIO = (Input)outIO.Flip(this);
            InIO.SetName(null);

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

        public override FIRIO[] GetIO()
        {
            return new FIRIO[] { InIO };
        }

        public override void Compute()
        {
            throw new Exception("This node is not computable");
        }

        internal override void InferType()
        {
        }
    }
}
