using ChiselDebug.GraphFIR.IO;
using FIRRTL;
using System;

namespace ChiselDebug.GraphFIR
{
    public class ConstValue : FIRRTLNode, INoPlaceAndRoute
    {
        public readonly FIRRTL.Literal Value;
        public readonly Output Result;

        public ConstValue(string outputName, FIRRTL.Literal value)
        {
            this.Value = value;

            this.Result = new Output(this, outputName, value.GetFIRType());
            Result.Con.Value.SetValueString(value.Value.ToSignedBinaryString((FIRRTL.GroundType)value.GetFIRType()));
        }

        public override Input[] GetInputs()
        {
            return Array.Empty<Input>();
        }

        public override Output[] GetOutputs()
        {
            return new Output[] { Result };
        }

        public override void InferType()
        {
        }
    }
}
