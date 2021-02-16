using System;

namespace ChiselDebug.GraphFIR
{
    public abstract class FIRRTLPrimOP : FIRRTLNode
    {
        public readonly Output Result;

        public FIRRTLPrimOP(string outputName)
        {
            this.Result = new Output(outputName);
        }

        public void ConnectTo(Input input)
        {
            if (input.IsConnected())
            {
                throw new ArgumentException("Not allowed to connect two ouputs to one input.", nameof(input));
            }

            Result.ConnectToInput(input);
        }

        public abstract void ConnectFrom(Span<Output> outputs);
    }
}
