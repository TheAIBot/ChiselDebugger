using FIRRTL;
using System;
using System.Collections.Generic;

namespace ChiselDebug.GraphFIR
{
    public class Mux : FIRRTLPrimOP
    {
        public List<Input> Choises = new List<Input>();
        public Input Decider = new Input("Selector", new FIRRTL.UIntType(1));

        public Mux(IFIRType outType) : base(outType)
        { }
    }
}
