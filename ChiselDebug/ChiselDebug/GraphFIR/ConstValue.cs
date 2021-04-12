using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;

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
            Result.SetType(value.GetFIRType());
            Result.Con.Value.SetValueString(value.Value.ToSignedBinaryString((FIRRTL.GroundType)value.GetFIRType()));
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

        public override void InferType()
        { }
    }
}
