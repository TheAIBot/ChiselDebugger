using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Diagnostics.CodeAnalysis;
using VCDReader;

namespace ChiselDebug.CombGraph
{
    public struct Computable : ICompute
    {
        private readonly FIRRTLNode? Node;
        private readonly Source? Con;
        public readonly bool IsBorderIO;
        private BinaryVarValue OldValue;

        public Computable(FIRRTLNode node)
        {
            this.Node = node;
            this.Con = null;
            this.IsBorderIO = false;
            this.OldValue = new BinaryVarValue();
        }

        public Computable(Source con)
        {
            this.Node = null;
            this.Con = con;
            this.IsBorderIO = con.Node is Module || con.Node is Wire;
            this.OldValue = new BinaryVarValue();
        }

        private void ComputeNode()
        {
            if (Node == null)
            {
                throw new InvalidOperationException($"{nameof(Node)} is null.");
            }

            Node.Compute();
        }

        private void ComputeCon()
        {
            //Copy value from other side of module
            if (IsBorderIO)
            {
                if (Con == null)
                {
                    throw new InvalidOperationException($"{nameof(Con)} is null.");
                }

                if (Con.Value == null)
                {
                    throw new InvalidOperationException($"{nameof(Con)} value was not initialized.");
                }

                Sink input = Con.GetPairedThrowIfNull();
                if (input.IsConnectedToAnything())
                {
                    Con.Value.OverrideValue(ref input.UpdateValueFromSourceFast());
                }
            }
        }

        public Source? ComputeGetIfChanged()
        {
            if (Node != null)
            {
                ComputeNode();
            }
            else
            {
                if (Con == null)
                {
                    throw new InvalidOperationException($"{nameof(Con)} is null.");
                }

                if (Con.Value == null)
                {
                    throw new InvalidOperationException($"{nameof(Con)} value was not initialized.");
                }

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

        public void Compute()
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
                if (Con == null)
                {
                    throw new InvalidOperationException($"{nameof(Con)} is null.");
                }

                Con.InferType();
                if (Con.Type == null)
                {
                    throw new InvalidOperationException($"{nameof(Con)} type is null.");
                }

                Con.SetDefaultvalue();

                OldValue = new BinaryVarValue(Con.Type.Width, false);
                OldValue.SetAllUnknown();

                foreach (var input in Con.GetConnectedInputs())
                {
                    input.InferType();
                    input.SetDefaultvalue();
                }
            }
        }

        public FIRRTLNode? GetNode()
        {
            return Node;
        }

        public Source? GetConnection()
        {
            return Con;
        }

        [MemberNotNullWhen(true, nameof(Node))]
        [MemberNotNullWhen(false, nameof(Con))]
        public bool TryGetNode([NotNullWhen(true)] out FIRRTLNode? node)
        {
            node = Node;
            return Node != null;
        }

        public override string ToString()
        {
            if (Con != null)
            {
                return $"Con: {Con.Node}, Module: {Con.GetModResideIn()?.Name}, Name: {Con.GetFullName()}";
            }
            else
            {
                return $"Node: {Node}";
            }
        }
    }
}
