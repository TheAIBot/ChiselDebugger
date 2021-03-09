using FIRRTL;
using System;
using System.Linq;
using System.Collections.Generic;

namespace ChiselDebug.GraphFIR
{
    public class FIRRTLContainer : FIRRTLNode, IContainerIO
    {
        public readonly Dictionary<string, FIRIO> ExternalIO = new Dictionary<string, FIRIO>();

        public readonly Dictionary<string, FIRIO> InternalIO = new Dictionary<string, FIRIO>();   

        public void AddExternalIO(FIRIO io)
        {
            ExternalIO.Add(io.Name, io);
            InternalIO.Add(io.Name, io.Flip());
        }

        public override Input[] GetInputs()
        {
            return ExternalIO.Values
                .SelectMany(x => x is IOBundle bundle ? bundle.Flatten() : new FIRIO[] { x })
                .Where(x => x is Input)
                .Cast<Input>()
                .ToArray();
        }

        public override Output[] GetOutputs()
        {
            return ExternalIO.Values
                .SelectMany(x => x is IOBundle bundle ? bundle.Flatten() : new FIRIO[] { x })
                .Where(x => x is Output)
                .Cast<Output>()
                .ToArray();
        }

        public Input[] GetInternalInputs()
        {
            return InternalIO.Values
                .SelectMany(x => x is IOBundle bundle ? bundle.Flatten() : new FIRIO[] { x })
                .Where(x => x is Input)
                .Cast<Input>()
                .ToArray();
        }

        public Output[] GetInternalOutputs()
        {
            return InternalIO.Values
                .SelectMany(x => x is IOBundle bundle ? bundle.Flatten() : new FIRIO[] { x })
                .Where(x => x is Output)
                .Cast<Output>()
                .ToArray();
        }

        public override void InferType()
        {
            throw new NotImplementedException();
        }

        public IContainerIO GetIO(string ioName)
        {
            if (InternalIO.TryGetValue(ioName, out FIRIO innerIO))
            {
                return innerIO;
            }

            throw new Exception($"Failed to find io. IO name: {ioName}");
        }
    }
}
