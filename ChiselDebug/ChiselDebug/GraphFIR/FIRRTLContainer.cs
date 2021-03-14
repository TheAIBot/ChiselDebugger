using FIRRTL;
using System;
using System.Linq;
using System.Collections.Generic;
using ChiselDebug.GraphFIR.IO;

namespace ChiselDebug.GraphFIR
{
    public abstract class FIRRTLContainer : FIRRTLNode, IContainerIO
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
            return FlattenAndFilterIO<Input>(ExternalIO);
        }

        public override Output[] GetOutputs()
        {
            return FlattenAndFilterIO<Output>(ExternalIO);
        }

        public Input[] GetInternalInputs()
        {
            return FlattenAndFilterIO<Input>(InternalIO);
        }

        public Output[] GetInternalOutputs()
        {
            return FlattenAndFilterIO<Output>(InternalIO);
        }

        internal static T[] FlattenAndFilterIO<T>(Dictionary<string, FIRIO> io)
        {
            return io.Values
                .SelectMany(x => x is IOBundle bundle ? bundle.Flatten() : new FIRIO[] { x })
                .OfType<T>()
                .ToArray();
        }

        public override void InferType()
        {
            throw new NotImplementedException();
        }

        public abstract bool TryGetIO(string ioName, bool modulesOnly, out IContainerIO container);

        public IContainerIO GetIO(string ioName, bool modulesOnly = false)
        {
            if (TryGetIO(ioName, modulesOnly, out var container))
            {
                return container;
            }

            throw new Exception($"Failed to find io. IO name: {ioName}");
        }
    }
}
