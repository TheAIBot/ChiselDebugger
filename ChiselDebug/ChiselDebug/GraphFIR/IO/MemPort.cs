using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System.Collections.Generic;

namespace ChiselDebug.GraphFIR.IO
{
    public abstract class MemPort : IOBundle, IPreserveDuplex
    {
        internal readonly FIRIO Address;
        internal readonly FIRIO Enabled;
        internal readonly FIRIO Clock;
        internal bool FromHighLevelFIRRTL = false;

        public MemPort(FIRRTLNode node, string name, List<FIRIO> io) : base(node, name, io)
        {
            this.Address = (FIRIO)GetIO("addr");
            this.Enabled = (FIRIO)GetIO("en");
            this.Clock = (FIRIO)GetIO("clk");
        }

        protected static void AsMaskType(FIRIO maskFrom)
        {
            foreach (ScalarIO scalar in maskFrom.Flatten())
            {
                scalar.SetType(new UIntType(1));
            }
        }

        internal abstract bool HasMask();
        internal abstract FIRIO GetMask();
    }
}
