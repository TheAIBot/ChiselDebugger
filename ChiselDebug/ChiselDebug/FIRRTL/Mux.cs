using System;
using System.Collections.Generic;

namespace ChiselDebug.FIRRTL
{
    public class Mux : FIRRTLPrimOP
    {
        public List<Input> Choises = new List<Input>();
        public Input Decider = new Input("Selector");

        public Mux(string outputName) : base(outputName)
        { }

        public override void ConnectFrom(Span<Output> outputs)
        {
            if (outputs.Length != Choises.Count + 1)
            {
                throw new ArgumentException($"Must connect all {Choises.Count} inputs at once.", nameof(outputs));
            }

            outputs[0].ConnectToInput(Decider);
            for (int i = 0; i < Choises.Count; i++)
            {
                outputs[i + 1].ConnectToInput(Choises[i]);
            }
        }
    }
}
