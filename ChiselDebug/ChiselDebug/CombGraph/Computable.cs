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
            this.OldValue = new BinaryVarValue();
        }

        public Computable(Output con)
        {
            this.Node = null;
            this.Con = con;
            this.OldValue = new BinaryVarValue();
        }

        private void ComputeNode()
        {
            Node.Compute();
        }

        private void ComputeCon()
        {
            //Copy value from other side of module
            if (Con.Node is Module mod)
            {
                Input input = (Input)mod.GetPairedIO(Con);
                if (input.IsConnectedToAnything())
                {
                    input.UpdateValueFromSource();
                    Con.Value.UpdateFrom(ref input.Value);
                }
            }
        }

        public Output ComputeGetIfChanged()
        {
            if (Node != null)
            {
                ComputeNode();
            }
            else
            {
                ComputeCon();

                //Did connection value change?
                if (!OldValue.SameValue(in Con.GetValue()))
                {
                    OldValue.SetBitsAndExtend(in Con.GetValue(), false);
                    Con.Value.UpdateValueString();
                    return Con;
                }
            }

            return null;
        }

        public void ComputeFast()
        {
            if (Node != null)
            {
                ComputeNode();
            }
            else
            {
                ComputeCon();
            }
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
                OldValue = new BinaryVarValue(Con.GetValue().Bits.Length, false);
                OldValue.SetAllUnknown();

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
                return $"Con: {Con.Node}, Module: {Con.GetModResideIn().Name}, Name: {Con.GetFullName()}";
            }
            else
            {
                return $"Node: {Node}";
            }
        }
    }
}
