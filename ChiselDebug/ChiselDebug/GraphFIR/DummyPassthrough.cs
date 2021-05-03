using ChiselDebug.GraphFIR.IO;
using System.Collections.Generic;

namespace ChiselDebug.GraphFIR
{
    public class DummyPassthrough : FIRRTLNode, INoPlaceAndRoute
    {
        public readonly Input InIO;
        public readonly Output Result;

        public DummyPassthrough(Output outIO) : base(null)
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
