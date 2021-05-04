using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
using System.Collections.Generic;
using System.Numerics;
using VCDReader;

namespace ChiselDebug.GraphFIR
{
    public class ConstValue : FIRRTLNode, INoPlaceAndRoute
    {
        public readonly Literal Value;
        public readonly Output Result;

        public ConstValue(string outputName, Literal value) : base(value)
        {
            this.Value = value;
            this.Result = new Output(this, outputName, value.GetFIRType());
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
            BinaryVarValue binValue = new BinaryVarValue(Value.Width);
            binValue.SetBitsAndExtend(Value.Value, false);
            Result.Value.UpdateValue(binValue);
        }

        internal override void InferType()
        { }
    }
}
