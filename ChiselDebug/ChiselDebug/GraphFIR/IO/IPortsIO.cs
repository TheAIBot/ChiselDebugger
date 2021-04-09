using System.Collections.Generic;

namespace ChiselDebug.GraphFIR.IO
{
    internal interface IPortsIO
    {
        internal FIRIO[] GetAllPorts();

        internal FIRIO[] GetOrMakeFlippedPortsFrom(FIRIO[] otherPorts);
    }
}
