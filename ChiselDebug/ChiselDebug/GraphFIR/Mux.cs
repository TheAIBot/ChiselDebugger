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
    }
}
