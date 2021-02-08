using ChiselDebug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerWebUI.Code
{
    public readonly struct IOPositions
    {
        public readonly List<Positioned<Input>> Inputs;
        public readonly List<Positioned<Output>> Outputs;

        public IOPositions(List<Positioned<Input>> inputs, List<Positioned<Output>> outputs)
        {
            this.Inputs = inputs;
            this.Outputs = outputs;
        }
    }
}
