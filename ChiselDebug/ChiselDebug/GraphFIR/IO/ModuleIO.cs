using ChiselDebug.GraphFIR.Components;
using System.Collections.Generic;
using System.Linq;
#nullable enable

namespace ChiselDebug.GraphFIR.IO
{
    public sealed class ModuleIO : IOBundle
    {
        public readonly Module Mod;

        public ModuleIO(Module mod, string? name, List<FIRIO> io) : base(mod, name, io)
        {
            this.Mod = mod;
        }

        public override FIRIO ToFlow(FlowChange flow, FIRRTLNode? node)
        {
            return new ModuleIO(Mod, Name, GetIOInOrder().Select(x => x.ToFlow(flow, node)).ToList());
        }
    }
}
