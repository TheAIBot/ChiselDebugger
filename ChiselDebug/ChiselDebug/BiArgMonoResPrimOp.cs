using System;

namespace ChiselDebug
{

    public abstract class BiArgMonoResPrimOp : FIRRTLPrimOP
    {
        public readonly string OpName;
        public readonly Input A;
        public readonly Input B;


        public BiArgMonoResPrimOp(string opName, string outputName) : base(outputName)
        {
            this.OpName = opName;
            this.A = new Input("a");
            this.B = new Input("b");
        }

        public void ConnectFrom(Output a, Output b)
        {
            a.ConnectToInput(A);
            b.ConnectToInput(B);
        }

        public override void ConnectFrom(Span<Output> outputs)
        {
            if (outputs.Length != 2)
            {
                throw new ArgumentException($"Must connect all 2 inputs at once.", nameof(outputs));
            }

            ConnectFrom(outputs[0], outputs[1]);
        }
    }

    public class FIRAdd : BiArgMonoResPrimOp
    {
        public FIRAdd(string outputName) : base("+", outputName)
        { }


    }
}
