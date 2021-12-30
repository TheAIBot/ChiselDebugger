using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;

namespace ChiselDebug.CombGraph
{
    public struct ComputableOpti : ICompute
    {
        private readonly FIRRTLNode Node;
        private readonly Output Con;

        public ComputableOpti(FIRRTLNode node)
        {
            this.Node = node;
            this.Con = null;
        }

        public ComputableOpti(Output con)
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
            Node.Compute();
        }

        private void ComputeCon()
        {
            Input input = Con.GetPaired();
            Con.Value.OverrideValue(ref input.UpdateValueFromSourceFast());
        }

        public Output ComputeGetIfChanged()
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
            if (Con != null)
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
