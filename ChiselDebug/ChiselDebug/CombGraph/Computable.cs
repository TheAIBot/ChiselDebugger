using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;

namespace ChiselDebug.CombGraph
{
    public readonly struct Computable
    {
        private readonly FIRRTLNode Node;
        private readonly Connection Con;

        public Computable(FIRRTLNode node)
        {
            this.Node = node;
            this.Con = null;
        }

        public Computable(Connection con)
        {
            this.Node = null;
            this.Con = con;
        }

        public Connection Compute()
        {
            if (Node != null)
            {
                //Compute node
            }
            else
            {
                if (Con.From.Node is Module mod)
                {
                    Input input = (Input)mod.GetPairedIO(Con.From);
                    if (input.IsConnectedToAnything())
                    {
                        Connection copyFrom = input.GetEnabledCon();
                        Con.Value.UpdateFrom(copyFrom.Value);

                        return Con;
                    }
                }
            }

            return null;
        }
    }
}
