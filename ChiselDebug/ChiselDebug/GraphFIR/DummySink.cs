using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR
{
    public class DummySink : FIRRTLNode, INoPlaceAndRoute
    {
        public readonly Input InIO;
        public readonly Output Result;

        public DummySink(Output outIO) : base(null)
        {
            this.InIO = (Input)outIO.Flip(this);
            InIO.SetName(null);

            this.Result = (Output)outIO.Copy(this);
            Result.SetName(null);

            outIO.ConnectToInput(InIO);
        }

        public override Input[] GetInputs()
        {
            return new Input[] { InIO };
        }

        public override Output[] GetOutputs()
        {
            return new Output[] { Result };
        }

        public override IEnumerable<FIRIO> GetIO()
        {
            yield return InIO;
            yield return Result;
        }

        public override void Compute()
        {
            Result.Value.UpdateFrom(InIO.Value);
        }

        internal override void InferType()
        {
            InIO.InferType();

            Result.SetType(InIO.Type);
        }
    }
}
