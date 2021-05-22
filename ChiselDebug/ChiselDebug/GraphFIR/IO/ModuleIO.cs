using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.GraphFIR.IO
{
    public class ModuleIO : IOBundle
    {
        public readonly Module Mod;

        public ModuleIO(Module mod, string name, List<FIRIO> io) : base(mod, name, io, true)
        {
            this.Mod = mod;
        }

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode node)
        {
            return new ModuleIO(Mod, Name, GetIOInOrder().Select(x => x.ToFlow(flow, node)).ToList());
        }
    }
}
