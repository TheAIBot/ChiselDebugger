using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR
{
    public class Mux : FIRRTLPrimOP
    {
        public List<Input> Choises = new List<Input>();
        public Input Decider = new Input("Selector", new FIRRTL.UIntType(1));

        public Mux(List<IFIRType> choiseTypes, IFIRType outType) : base(outType)
        {
            Choises = choiseTypes.Select(x => new Input(x)).ToList();
        }

        public override Input[] GetInputs()
        {
            List<Input> inputs = new List<Input>();
            inputs.AddRange(Choises);
            inputs.Add(Decider);
            return inputs.ToArray();
        }

        public override void InferType()
        {
            if (Choises.First().Type is not FIRRTL.UnknownType)
            {
                return;
            }

            Choises.First().InferType();

            Result.SetType(Choises.First().Type);
        }
    }
}
