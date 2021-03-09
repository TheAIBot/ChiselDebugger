using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR
{
    public class Mux : FIRRTLPrimOP
    {
        public readonly Input[] Choises;
        public readonly Input Decider;

        public Mux(List<IFIRType> choiseTypes, IFIRType outType) : base(outType)
        {
            this.Choises = choiseTypes.Select(x => new Input(this, x)).ToArray();
            this.Decider = new Input(this, new FIRRTL.UIntType(1));
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
