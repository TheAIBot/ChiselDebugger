using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ChiselDebug.GraphFIR.Components
{
    public abstract class FIRRTLContainer : PairedIOFIRRTLNode, IContainerIO
    {
        private readonly List<FIRIO> AllIOInOrder = new List<FIRIO>();
        public readonly Dictionary<string, FIRIO> ExternalIO = new Dictionary<string, FIRIO>();
        public readonly Dictionary<string, FIRIO> InternalIO = new Dictionary<string, FIRIO>();
        private int UniqueNameGen = 0;

        public FIRRTLContainer(FirrtlNode defNode) : base(defNode) { }

        public void AddExternalIO(FIRIO io)
        {
            FIRIO flipped = io.Flip(this);
            AddIO(io, flipped);
        }

        public void AddInternalIO(FIRIO io)
        {
            FIRIO flipped = io.Flip(this);
            AddIO(flipped, io);
        }

        private void AddIO(FIRIO externalIO, FIRIO internalIO)
        {
            Debug.Assert(externalIO.Flatten().All(x => x.Node == this));
            Debug.Assert(internalIO.Flatten().All(x => x.Node == this));

            ExternalIO.Add(externalIO.Name, externalIO);
            InternalIO.Add(internalIO.Name, internalIO);
            AllIOInOrder.Add(internalIO);

            AddPairedIO(externalIO, internalIO);
        }

        public void AddAnonymousExternalIO(FIRIO io)
        {
            string uniqueName = $"~${UniqueNameGen++}";
            io.SetName(uniqueName);

            AddExternalIO(io);
        }

        public void AddAnonymousInternalIO(FIRIO io)
        {
            string uniqueName = $"~${UniqueNameGen++}";
            io.SetName(uniqueName);

            AddInternalIO(io);
        }

        public bool IsAnonymousExtIntIO(FIRIO io)
        {
            if (io.IsAnonymous)
            {
                return false;
            }

            return io.Name.StartsWith("~$");
        }

        public override Sink[] GetSinks()
        {
            return FlattenAndFilterIOPaired<Source, Sink>(AllIOInOrder);
        }

        public override Source[] GetSources()
        {
            return FlattenAndFilterIOPaired<Sink, Source>(AllIOInOrder);
        }

        public override Dictionary<string, FIRIO>.ValueCollection GetIO()
        {
            return ExternalIO.Values;
        }

        public FIRIO[] GetInternalIO()
        {
            return InternalIO.Values.ToArray();
        }

        public IEnumerable<Sink> GetInternalSinks()
        {
            List<ScalarIO> scalars = new List<ScalarIO>();
            foreach (var intIO in InternalIO.Values)
            {
                scalars.Clear();
                foreach (var flat in intIO.Flatten(scalars))
                {
                    if (flat is Sink inT)
                    {
                        yield return inT;
                    }
                }
            }
        }

        public IEnumerable<Source> GetInternalSources()
        {
            List<ScalarIO> scalars = new List<ScalarIO>();
            foreach (var intIO in InternalIO.Values)
            {
                scalars.Clear();
                foreach (var flat in intIO.Flatten(scalars))
                {
                    if (flat is Source outT)
                    {
                        yield return outT;
                    }
                }
            }
        }

        internal static To[] FlattenAndFilterIOPaired<From, To>(IEnumerable<FIRIO> io) where From : ScalarIO where To : ScalarIO
        {
            List<To> filtered = new List<To>();
            List<ScalarIO> scalars = new List<ScalarIO>();
            foreach (var value in io)
            {
                scalars.Clear();
                foreach (var flatValue in value.Flatten(scalars))
                {
                    if (flatValue is From tFlatVal)
                    {
                        filtered.Add((To)tFlatVal.GetPaired());
                    }
                }
            }

            return filtered.ToArray();
        }

        protected List<ScalarIO> GetAllInternalIOOrdered()
        {
            List<ScalarIO> ordered = new List<ScalarIO>();
            foreach (var io in AllIOInOrder)
            {
                io.Flatten(ordered);
            }

            return ordered;
        }

        internal override void InferType()
        {
            throw new NotImplementedException();
        }

        public abstract bool TryGetIO(string ioName, out IContainerIO container);

        public IContainerIO GetIO(string ioName)
        {
            if (TryGetIO(ioName, out var container))
            {
                return container;
            }

            throw new Exception($"Failed to find io. IO name: {ioName}");
        }
    }
}
