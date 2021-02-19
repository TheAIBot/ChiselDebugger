using FIRRTL;
using System;

namespace ChiselDebug.GraphFIR
{
    public abstract class BiArgMonoResPrimOp : FIRRTLPrimOP
    {
        public readonly string OpName;
        public readonly Input A;
        public readonly Input B;

        public BiArgMonoResPrimOp(string opName, Output aIn, Output bIn, IFIRType outType) : base(outType)
        {
            this.OpName = opName;
            this.A = new Input("a", aIn.Type);
            this.B = new Input("b", bIn.Type);

            aIn.ConnectToInput(A);
            bIn.ConnectToInput(B);
        }

        public override Input[] GetInputs()
        {
            return new Input[] { A, B };
        }
    }

    public class FIRAdd : BiArgMonoResPrimOp
    {
        public FIRAdd(Output aIn, Output bIn, IFIRType outType) : base("+", aIn, bIn, outType) { }
    }

    public class FIRSub : BiArgMonoResPrimOp
    {
        public FIRSub(Output aIn, Output bIn, IFIRType outType) : base("-", aIn, bIn, outType) { }
    }

    public class FIRMul : BiArgMonoResPrimOp
    {
        public FIRMul(Output aIn, Output bIn, IFIRType outType) : base("*", aIn, bIn, outType) { }
    }

    public class FIRDiv : BiArgMonoResPrimOp
    {
        public FIRDiv(Output aIn, Output bIn, IFIRType outType) : base("/", aIn, bIn, outType) { }
    }

    public class FIREq : BiArgMonoResPrimOp
    {
        public FIREq(Output aIn, Output bIn, IFIRType outType) : base("=", aIn, bIn, outType){ }
    }
    
    public class FIRNeq : BiArgMonoResPrimOp
    {
        public FIRNeq(Output aIn, Output bIn, IFIRType outType) : base("≠", aIn, bIn, outType) { }
    }
    
    public class FIRGeq : BiArgMonoResPrimOp
    {
        public FIRGeq(Output aIn, Output bIn, IFIRType outType) : base("≥", aIn, bIn, outType) { }
    }
    
    public class FIRLeq : BiArgMonoResPrimOp
    {
        public FIRLeq(Output aIn, Output bIn, IFIRType outType) : base("≤", aIn, bIn, outType) { }
    }
    
    public class FIRGt : BiArgMonoResPrimOp
    {
        public FIRGt(Output aIn, Output bIn, IFIRType outType) : base(">", aIn, bIn, outType) { }
    }
    
    public class FIRLt : BiArgMonoResPrimOp
    {
        public FIRLt(Output aIn, Output bIn, IFIRType outType) : base("<", aIn, bIn, outType) { }
    }
}
