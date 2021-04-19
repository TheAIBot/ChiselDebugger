using ChiselDebug.GraphFIR.IO;
using System;
using System.Linq;

namespace ChiselDebug.GraphFIR
{
    public class DummySink : FIRRTLNode, INoPlaceAndRoute
    {
        public readonly FIRIO InIO;

        public DummySink(Output outIO) : base(null)
        {
            this.InIO = outIO.Flip(this);
            InIO.SetName(null);

            outIO.ConnectToInput(InIO);
        }

        public override ScalarIO[] GetInputs()
        {
            return InIO.Flatten().ToArray();
        }

        public override ScalarIO[] GetOutputs()
        {
            return Array.Empty<ScalarIO>();
        }

        public override FIRIO[] GetIO()
        {
            return new FIRIO[] { InIO };
        }

        public override void Compute()
        {
            throw new Exception("This node is not computable");
        }

        public override void InferType()
        {
        }
    }
}
