using System.Collections.Generic;

namespace ChiselDebug.GraphFIR.IO
{
    internal interface IHiddenPorts
    {
        internal bool HasHiddenPorts();

        internal FIRIO[] GetHiddenPorts();

        internal List<FIRIO> CopyHiddenPortsFrom(IHiddenPorts otherWithPorts);

        internal void MakePortsVisible();
    }
}
