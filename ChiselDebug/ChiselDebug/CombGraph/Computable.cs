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
        public readonly bool IsBorderIO;
        private BinaryVarValue OldValue;

        public Computable(FIRRTLNode node)
        {
            this.Node = node;
            this.Con = null;
            this.IsBorderIO = false;
            this.OldValue = new BinaryVarValue();
        }

        public Computable(Output con)
        {
            this.Node = null;
            this.Con = con;
            this.IsBorderIO = con.Node is Module || con.Node is Wire;
            this.OldValue = new BinaryVarValue();
        }

        private void ComputeNode()
        {
            Node.Compute();
        }

        private void ComputeCon()
        {
            //Copy value from other side of module
            if (IsBorderIO)
            {
                Input input = Con.GetPaired();
                if (input.IsConnectedToAnything())
                {
                    Con.Value.UpdateValue(ref input.UpdateValueFromSourceFast());
                }
            }
        }

        private void ComputeConOptimized()
        {
            Input input = Con.GetPaired();
            Con.Value.UpdateValue(ref input.UpdateValueFromSourceFast());
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
                if (!OldValue.SameValue(ref Con.GetValue()))
                {
                    OldValue.SetBitsAndExtend(ref Con.GetValue(), false);
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

        public void ComputeFastOptimized()
        {
            if (Node != null)
            {
                ComputeNode();
            }
            else
            {
                ComputeConOptimized();
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

        public FIRRTLNode GetNode()
        {
            return Node;
        }

        public Output GetConnection()
        {
            return Con;
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
