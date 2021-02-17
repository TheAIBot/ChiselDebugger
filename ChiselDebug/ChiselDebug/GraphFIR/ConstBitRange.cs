using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.GraphFIR
{
    public class ConstBitRange : FIRRTLPrimOP
    {
        public readonly Input In;

        public ConstBitRange(Output arg1, IFIRType outType) : base(outType)
        {
            this.In = new Input(arg1.Type);
            arg1.ConnectToInput(In);
        }

        public override Input[] GetInputs()
        {
            return new Input[] { In };
        }
    }
}
