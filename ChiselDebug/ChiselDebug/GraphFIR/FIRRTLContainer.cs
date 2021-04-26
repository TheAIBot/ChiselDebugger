using FIRRTL;
using System;
using System.Linq;
using System.Collections.Generic;
using ChiselDebug.GraphFIR.IO;
using System.Diagnostics;

namespace ChiselDebug.GraphFIR
{
    public abstract class FIRRTLContainer : PairedIOFIRRTLNode, IContainerIO
    {
        private readonly List<FIRIO> AllIOInOrder = new List<FIRIO>();
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

            AddPairedIO(io, flipped);
        }

        internal void ReserveMemory(int extIntSize)
        {
            ExternalIO.EnsureCapacity(extIntSize);
            InternalIO.EnsureCapacity(extIntSize);
            AllIOInOrder.Capacity = extIntSize;
        }

        public override Input[] GetInputs()
        {
            return FlattenAndFilterIO<Input>(ExternalIO);
        }

        public override Output[] GetOutputs()
        {
            return FlattenAndFilterIO<Output>(ExternalIO);
        }

        public override Dictionary<string, FIRIO>.ValueCollection GetIO()
        {
            return ExternalIO.Values;
        }

        public FIRIO[] GetInternalIO()
        {
            return InternalIO.Values.ToArray();
        }

        public IEnumerable<Input> GetInternalInputs()
        {
            foreach (var intIO in InternalIO.Values)
            {
                if (intIO is ScalarIO)
                {
                    if (intIO is Input inT)
                    {
                        yield return inT;
                    }
                }
                else
                {
                    foreach (var flat in intIO.Flatten())
                    {
                        if (flat is Input inT)
                        {
                            yield return inT;
                        }
                    }
                }
            }
        }

        public IEnumerable<Output> GetInternalOutputs()
        {
            foreach (var intIO in InternalIO.Values)
            {
                if (intIO is ScalarIO)
                {
                    if (intIO is Output outT)
                    {
                        yield return outT;
                    }
                }
                else
                {
                    foreach (var flat in intIO.Flatten())
                    {
                        if (flat is Output outT)
                        {
                            yield return outT;
                        }
                    }
                }
            }
        }

        internal static T[] FlattenAndFilterIO<T>(Dictionary<string, FIRIO> io)
        {
            List<T> filtered = new List<T>(io.Values.Count);
            foreach (var value in io.Values)
            {
                if (value is ScalarIO)
                {
                    if (value is T tVal)
                    {
                        filtered.Add(tVal);
                    }
                }
                else
                {
                    foreach (var flatValue in value.Flatten())
                    {
                        if (flatValue is T tFlatVal)
                        {
                            filtered.Add(tFlatVal);
                        }
                    }
                }
            }

            return filtered.ToArray();
        }

        public virtual FIRIO[] GetAllIOOrdered()
        {
            return AllIOInOrder.SelectMany(x => x.Flatten()).ToArray();
        }

        internal override void InferType()
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
