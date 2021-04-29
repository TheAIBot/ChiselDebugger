using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using System;
using VCDReader;

namespace ChiselDebug.CombGraph
{
    public struct Computable
    {
        private readonly FIRRTLNode Node;
        private readonly Output Con;
        private BinaryVarValue OldValue;

        public Computable(FIRRTLNode node)
        {
            this.Node = node;
            this.Con = null;
            this.OldValue = null;
        }

        public Computable(Output con)
        {
            this.Node = null;
            this.Con = con;
            this.OldValue = null;// new BinaryVarValue(Con.Value.GetValue().Bits.Length);
            //Array.Fill(OldValue.Bits, BitState.X);
        }

        public Output Compute()
        {
            if (Node != null)
            {
                Node.Compute();
            }
            else
            {
                //Copy value from other side of module
                if (Con.Node is Module mod)
                {
                    Input input = (Input)mod.GetPairedIO(Con);
                    if (input.IsConnectedToAnything())
                    {
                        input.UpdateValueFromSource();
                        Con.Value.UpdateFrom(input.Value);
                    }
                }

                //Did connection value change?
                if (!OldValue.SameValue(Con.Value.GetValue()))
                {
                    OldValue.SetBitsAndExtend(Con.Value.GetValue(), false);
                    Con.Value.UpdateValueString();
                    return Con;
                }
            }

            return null;
        }

        public void InferType()
        {
            if (Node != null)
            {
                Node.InferType();
            }
            else
            {
                Con.InferType();
                Con.SetDefaultvalue();
                OldValue = new BinaryVarValue(Con.Value.GetValue().Bits.Length);
                Array.Fill(OldValue.Bits, BitState.X);

                foreach (var input in Con.GetConnectedInputs())
                {
                    input.InferType();
                    input.SetDefaultvalue();
                }
            }
        }

        public override string ToString()
        {
            if (Con!= null)
            {
                return $"Con: {Con.Node}";
            }
            else
            {
                return $"Node: {Node}";
            }
        }
    }
}
