using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
#nullable enable

namespace ChiselDebug.GraphFIR.Components
{
    public sealed class DummyPassthrough : FIRRTLNode, INoPlaceAndRoute
    {
        public readonly Sink InIO;
        public readonly Source Result;

        public DummyPassthrough(Source outIO) : base(null)
        {
            InIO = (Sink)outIO.Flip(this);
            Result = (Source)outIO.Copy(this);

            outIO.ConnectToInput(InIO);
        }

        public override Sink[] GetSinks()
        {
            return new Sink[] { InIO };
        }

        public override Source[] GetSources()
        {
            return new Source[] { Result };
        }

        public override IEnumerable<FIRIO> GetIO()
        {
            yield return InIO;
            yield return Result;
        }

        public override void Compute()
        {
            if (Result.Value == null)
            {
                throw new InvalidOperationException($"{nameof(Result)} value was not initialized.");
            }

            Result.Value.UpdateValue(ref InIO.UpdateValueFromSourceFast());
        }

        internal override void InferType()
        {
            InIO.InferType();

            Result.SetType(InIO.Type);
        }
    }
}
