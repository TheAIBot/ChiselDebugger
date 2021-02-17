using FIRRTL;
using System;

namespace ChiselDebug.GraphFIR
{
    public abstract class BiArgMonoResPrimOp : FIRRTLPrimOP
    {
        public readonly string OpName;
        public readonly Input A;
        public readonly Input B;


        public BiArgMonoResPrimOp(string opName, IFIRType aType, IFIRType bType, IFIRType outType) : base(outType)
        {
            this.OpName = opName;
            this.A = new Input("a", aType);
            this.B = new Input("b", bType);
        }

        public override Input[] GetInputs()
        {
            return new Input[] { A, B };
        }
    }

    public class FIRAdd : BiArgMonoResPrimOp
    {
        public FIRAdd(Output aIn, Output bIn, IFIRType outType) : base("+", aIn.Type, bIn.Type, outType)
        {
            aIn.ConnectToInput(A);
            bIn.ConnectToInput(B);
        }
    }

    public class FIRSub : BiArgMonoResPrimOp
    {
        public FIRSub(Output aIn, Output bIn, IFIRType outType) : base("-", aIn.Type, bIn.Type, outType)
        {
            aIn.ConnectToInput(A);
            bIn.ConnectToInput(B);
        }
    }

    public class FIRMul : BiArgMonoResPrimOp
    {
        public FIRMul(Output aIn, Output bIn, IFIRType outType) : base("*", aIn.Type, bIn.Type, outType)
        {
            aIn.ConnectToInput(A);
            bIn.ConnectToInput(B);
        }
    }

    public class FIRDiv : BiArgMonoResPrimOp
    {
        public FIRDiv(Output aIn, Output bIn, IFIRType outType) : base("/", aIn.Type, bIn.Type, outType)
        {
            aIn.ConnectToInput(A);
            bIn.ConnectToInput(B);
        }
    }
}
