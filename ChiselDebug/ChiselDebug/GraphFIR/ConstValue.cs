using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;
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

            BinaryVarValue binValue = new BinaryVarValue(new BitState[value.Width], null);
            binValue.SetBits(value.Value);
            Result.Con.Value.UpdateValue(binValue);
        }

        public override ScalarIO[] GetInputs()
        {
            return Array.Empty<ScalarIO>();
        }

        public override ScalarIO[] GetOutputs()
        {
            return new ScalarIO[] { Result };
        }

        public override FIRIO[] GetIO()
        {
            return new FIRIO[] { Result };
        }

        public override void Compute()
        {
            throw new Exception("This node is not computable");
        }

        public override void InferType()
        { }
    }
}
