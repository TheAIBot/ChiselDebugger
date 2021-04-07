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

        public MemPort(string name, List<FIRIO> io) : base(name, io)
        {
            this.Address = (FIRIO)GetIO("addr");
            this.Enabled = (FIRIO)GetIO("en");
            this.Clock = (FIRIO)GetIO("clk");
        }

        protected static void AsMaskType(FIRIO maskFrom)
        {
            if (maskFrom is IOBundle bundle)
            {
                foreach (ScalarIO scalar in bundle.Flatten())
                {
                    scalar.SetType(new UIntType(1));
                }
            }
            else if (maskFrom is ScalarIO scalar)
            {
                scalar.SetType(new UIntType(1));
            }
        }

        internal abstract bool HasMask();
        internal abstract FIRIO GetMask();
    }
}
