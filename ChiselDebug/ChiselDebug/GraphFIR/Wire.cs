using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChiselDebug.GraphFIR
{
    public class Wire : FIRRTLNode
    {
        public readonly string Name;
        public readonly FIRIO In;
        public readonly FIRIO Result;
        private readonly Dictionary<FIRIO, FIRIO> IOPairs = new Dictionary<FIRIO, FIRIO>();

        public Wire(string name, FIRIO inputType, FirrtlNode defNode) : base(defNode)
        {
            if (!inputType.IsPassiveOfType<Input>())
            {
                throw new Exception("Wire input type must be a passive input type.");
            }

            this.Name = name;
            this.In = inputType.Copy(this);
            this.Result = inputType.Flip(this);
            IOHelper.PairIO(IOPairs, In, Result);

            In.SetName(Name + "/in");
            Result.SetName(Name);
        }

        internal void AddPairedIO(FIRIO io, FIRIO ioFlipped)
        {
            IOHelper.PairIO(IOPairs, io, ioFlipped);
        }

        public FIRIO GetPairedIO(FIRIO io)
        {
            return IOPairs[io];
        }

        public bool IsPartOfPair(FIRIO io)
        {
            return IOPairs.ContainsKey(io);
        }

        internal void BypassWireIO()
        {
            IOHelper.BypassIO(In, Result);
        }

        internal DuplexIO GetAsDuplex()
        {
            return new DuplexIO(this, Name, In, Result);
        }

        public override ScalarIO[] GetInputs()
        {
            return In.Flatten().ToArray();
        }

        public override ScalarIO[] GetOutputs()
        {
            return Result.Flatten().ToArray();
        }

        public override FIRIO[] GetIO()
        {
            return new FIRIO[] { In, Result };
        }

        public override void InferType()
        {
            foreach (Input input in GetInputs())
            {
                input.InferType();
            }
        }
    }
}
