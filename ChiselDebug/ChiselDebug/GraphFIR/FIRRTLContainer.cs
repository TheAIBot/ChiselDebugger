﻿using FIRRTL;
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

        public void AddAnonymousExternalIO(ScalarIO io)
        {
            string uniqueName = $"~${UniqueNameGen++}";
            io.SetName(uniqueName);

            AddExternalIO(io);
        }

        public void AddAnonymousInternalIO(ScalarIO io)
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

        public bool IsExternalModIO(FIRIO io)
        {
            FIRIO rootIO = io.GetRootIO();
            Debug.Assert(rootIO.Node == this && (ExternalIO.ContainsKey(rootIO.Name) || InternalIO.ContainsKey(rootIO.Name)));
            return ExternalIO.ContainsKey(rootIO.Name);
        }

        public bool IsInternalModIO(FIRIO io)
        {
            FIRIO rootIO = io.GetRootIO();
            Debug.Assert(rootIO.Node == this && (ExternalIO.ContainsKey(rootIO.Name) || InternalIO.ContainsKey(rootIO.Name)));
            return InternalIO.ContainsKey(rootIO.Name);
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
            List<ScalarIO> scalars = new List<ScalarIO>();
            foreach (var intIO in InternalIO.Values)
            {
                scalars.Clear();
                foreach (var flat in intIO.Flatten(scalars))
                {
                    if (flat is Input inT)
                    {
                        yield return inT;
                    }
                }
            }
        }

        public IEnumerable<Output> GetInternalOutputs()
        {
            List<ScalarIO> scalars = new List<ScalarIO>();
            foreach (var intIO in InternalIO.Values)
            {
                scalars.Clear();
                foreach (var flat in intIO.Flatten(scalars))
                {
                    if (flat is Output outT)
                    {
                        yield return outT;
                    }
                }
            }
        }

        internal static T[] FlattenAndFilterIO<T>(Dictionary<string, FIRIO> io)
        {
            List<T> filtered = new List<T>(io.Values.Count);
            List<ScalarIO> scalars = new List<ScalarIO>();
            foreach (var value in io.Values)
            {
                scalars.Clear();
                foreach (var flatValue in value.Flatten(scalars))
                {
                    if (flatValue is T tFlatVal)
                    {
                        filtered.Add(tFlatVal);
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
