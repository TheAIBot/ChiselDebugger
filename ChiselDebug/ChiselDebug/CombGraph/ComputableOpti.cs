using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using System;

namespace ChiselDebug.CombGraph
{
    public readonly struct ComputableOpti : ICompute
    {
        private readonly FIRRTLNode? Node;
        private readonly Source? Con;

        public ComputableOpti(FIRRTLNode node)
        {
            this.Node = node;
            this.Con = null;
        }

        public ComputableOpti(Source con)
        {
            this.Node = null;
            this.Con = con;
        }

        public ComputableOpti(ICompute comp)
        {
            this.Node = comp.GetNode();
            this.Con = comp.GetConnection();
        }

        private void ComputeNode()
        {
            if (Node == null)
            {
                throw new InvalidOperationException("Attempted to compute a null node.");
            }

            Node.Compute();
        }

        private void ComputeCon()
        {
            if (Con == null)
            {
                throw new InvalidOperationException("Attempted to compute a null connection.");
            }

            if (Con.Value == null)
            {
                throw new InvalidOperationException("Attempted to compute with a connection with null value.");
            }

            Sink input = Con.GetPairedThrowIfNull();
            Con.Value.OverrideValue(ref input.UpdateValueFromSourceFast());
        }

        public Source? ComputeGetIfChanged()
        {
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
        }

        public FIRRTLNode? GetNode()
        {
            return Node;
        }

        public Source? GetConnection()
        {
            return Con;
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
