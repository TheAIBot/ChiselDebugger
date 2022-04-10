using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;

namespace ChiselDebug.GraphFIR.Components
{
    public sealed class Memory : FIRRTLNode, IContainerIO, IStatePreserving
    {
        public readonly string Name;
        public readonly FIRIO InputType;
        public readonly ulong Size;
        public readonly int ReadLatency;
        public readonly int WriteLatency;
        public readonly ReadUnderWrite RUW;
        private readonly MemoryIO MemIO;

        public Memory(string name, FIRIO inputType, ulong size, int readLatency, int writeLatency, ReadUnderWrite ruw, FirrtlNode defNode) : base(defNode)
        {
            if (!inputType.IsPassiveOfType<Sink>())
            {
                throw new Exception("Input type must be a passive input type.");
            }

            Name = name;
            InputType = inputType.Copy(this);
            Size = size;
            ReadLatency = readLatency;
            WriteLatency = writeLatency;
            RUW = ruw;
            MemIO = new MemoryIO(this, name, new List<FIRIO>(), InputType, GetAddressWidth());
        }

        internal MemReadPort AddReadPort(string portName)
        {
            return MemIO.AddReadPort(portName);
        }

        internal MemWritePort AddWritePort(string portName)
        {
            return MemIO.AddWritePort(portName);
        }

        internal MemRWPort AddReadWritePort(string portName)
        {
            return MemIO.AddReadWritePort(portName);
        }

        internal int GetAddressWidth()
        {
            if (Size == 1)
            {
                return 1;
            }
            return (int)Math.Ceiling(Math.Log2(Size));
        }

        internal MemoryIO GetIOAsBundle()
        {
            return MemIO;
        }

        public override Sink[] GetSinks()
        {
            return MemIO.FlattenOnly<Sink>();
        }

        public override Source[] GetSources()
        {
            return MemIO.FlattenOnly<Source>();
        }

        public override IEnumerable<FIRIO> GetIO()
        {
            yield return MemIO;
        }

        public override IEnumerable<FIRIO> GetVisibleIO()
        {
            yield return GetIOAsBundle();
        }

        public override void Compute()
        {
            throw new Exception("This node is not computable");
        }

        internal override void InferType()
        {
            foreach (var input in GetSinks())
            {
                input.InferType();
            }
            foreach (var output in GetSources())
            {
                output.InferType();
            }
        }

        public bool TryGetIO(string ioName, out IContainerIO container)
        {
            if (MemIO.TryGetIO(ioName, out IContainerIO innerIO))
            {
                container = innerIO;
                return true;
            }

            container = null;
            return false;
        }

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
