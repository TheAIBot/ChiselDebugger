using FIRRTL;
using System;
using System.Linq;
using System.Collections.Generic;
using ChiselDebug.GraphFIR.IO;
using System.Diagnostics;

namespace ChiselDebug.GraphFIR
{
    public abstract class FIRRTLContainer : FIRRTLNode, IContainerIO
    {
        private readonly List<FIRIO> AllIOInOrder = new List<FIRIO>();
        private readonly Dictionary<FIRIO, FIRIO> IOPairs = new Dictionary<FIRIO, FIRIO>();
        public readonly Dictionary<string, FIRIO> ExternalIO = new Dictionary<string, FIRIO>();
        public readonly Dictionary<string, FIRIO> InternalIO = new Dictionary<string, FIRIO>();   

        public FIRRTLContainer(FirrtlNode defNode) : base(defNode) { }

        public void AddExternalIO(FIRIO io)
        {
            Debug.Assert(io.Flatten().All(x => x.Node == this));

            FIRIO flipped = io.Flip(this);
            ExternalIO.Add(io.Name, io);
            InternalIO.Add(io.Name, flipped);
            AllIOInOrder.Add(flipped);

            FIRIO[] ioWalk = io.WalkIOTree().ToArray();
            FIRIO[] ioFlipWalk = flipped.WalkIOTree().ToArray();
            if (ioWalk.Length != ioFlipWalk.Length)
            {
                throw new Exception($"Internal and external io must be of the same size when added to a module. External: {ioWalk.Length}, Internal: {ioFlipWalk.Length}");
            }

            for (int i = 0; i < ioWalk.Length; i++)
            {
                IOPairs.Add(ioWalk[i], ioFlipWalk[i]);
                IOPairs.Add(ioFlipWalk[i], ioWalk[i]);
            }
        }

        public override ScalarIO[] GetInputs()
        {
            return FlattenAndFilterIO<Input>(ExternalIO);
        }

        public override ScalarIO[] GetOutputs()
        {
            return FlattenAndFilterIO<Output>(ExternalIO);
        }

        public override FIRIO[] GetIO()
        {
            return ExternalIO.Values.ToArray();
        }

        public FIRIO[] GetInternalIO()
        {
            return InternalIO.Values.ToArray();
        }

        public ScalarIO[] GetInternalInputs()
        {
            return FlattenAndFilterIO<Input>(InternalIO);
        }

        public ScalarIO[] GetInternalOutputs()
        {
            return FlattenAndFilterIO<Output>(InternalIO);
        }

        internal static T[] FlattenAndFilterIO<T>(Dictionary<string, FIRIO> io)
        {
            return io.Values
                .SelectMany(x => x.Flatten())
                .OfType<T>()
                .ToArray();
        }

        public virtual FIRIO[] GetAllIOOrdered()
        {
            return AllIOInOrder.SelectMany(x => x.Flatten()).ToArray();
        }

        public FIRIO GetPairedIO(FIRIO io)
        {
            return IOPairs[io];
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
