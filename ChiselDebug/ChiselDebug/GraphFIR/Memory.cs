using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.GraphFIR
{
    public class Memory : FIRRTLNode, IContainerIO
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
            if (!inputType.IsPassiveOfType<Input>())
            {
                throw new Exception("Input type must be a passive input type.");
            }

            this.Name = name;
            this.InputType = inputType.Copy(this);
            this.Size = size;
            this.ReadLatency = readLatency;
            this.WriteLatency = writeLatency;
            this.RUW = ruw;
            this.MemIO = new MemoryIO(this, name, new List<FIRIO>(), InputType, GetAddressWidth());
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
            return (int)Math.Round(Math.Log2(Size));
        }

        internal MemoryIO GetIOAsBundle()
        {
            return MemIO;
        }

        public override ScalarIO[] GetInputs()
        {
            return MemIO.Flatten().OfType<Input>().ToArray();
        }

        public override ScalarIO[] GetOutputs()
        {
            return MemIO.Flatten().OfType<Output>().ToArray();
        }

        public override FIRIO[] GetIO()
        {
            return new FIRIO[] { MemIO };
        }

        public override void Compute()
        {
            throw new Exception("This node is not computable");
        }

        internal override void InferType()
        {
            foreach (var input in GetInputs())
            {
                input.InferType();
            }
            foreach (var output in GetOutputs())
            {
                output.InferType();
            }
        }

        public bool TryGetIO(string ioName, bool modulesOnly, out IContainerIO container)
        {
            if (MemIO.TryGetIO(ioName, modulesOnly, out IContainerIO innerIO))
            {
                container = innerIO;
                return true;
            }

            container = null;
            return false;
        }

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
