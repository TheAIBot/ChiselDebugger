using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using VCDReader;

namespace ChiselDebug.GraphFIR.Components
{
    public sealed class ConstValue : FIRRTLNode, INoPlaceAndRoute
    {
        public readonly Literal Value;
        public readonly Output Result;
        private bool FirstCompute = true;

        public ConstValue(Literal value) : base(value)
        {
            Value = value;
            Result = new Output(this, null, value.GetFIRType());
        }

        public override Input[] GetInputs()
        {
            return Array.Empty<Input>();
        }

        public override Output[] GetOutputs()
        {
            return new Output[] { Result };
        }

        public override IEnumerable<FIRIO> GetIO()
        {
            yield return Result;
        }

        public override void Compute()
        {
            if (FirstCompute)
            {
                FirstCompute = false;

                BinaryVarValue binValue = new BinaryVarValue(Value.Width, true);
                binValue.SetBitsAndExtend(Value.Value);
                Result.Value.UpdateValue(ref binValue);
            }
        }

        internal override void InferType()
        { }
    }
}
