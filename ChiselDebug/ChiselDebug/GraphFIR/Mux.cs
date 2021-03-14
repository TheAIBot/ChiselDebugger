using ChiselDebug.GraphFIR.IO;
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

        public override ScalarIO[] GetInputs()
        {
            List<ScalarIO> inputs = new List<ScalarIO>();
            inputs.AddRange(Choises);
            inputs.Add(Decider);
            return inputs.ToArray();
        }

        public override FIRIO[] GetIO()
        {
            List<FIRIO> io = new List<FIRIO>();
            io.AddRange(Choises);
            io.Add(Decider);
            io.Add(Result);

            return io.ToArray();
        }

        public override void InferType()
        {
            foreach (var input in Choises)
            {
                input.InferType();
            }
            Decider.InferType();

            Result.SetType(Choises.First().Type);
        }
    }
}
