using FIRRTL;
using System;
using VCDReader;

namespace ChiselDebug.GraphFIR
{
    public class ValueType
    {
        string ValueString = string.Empty;
        private readonly IFIRType Type;
        private VarValue Value;

        public ValueType(IFIRType type)
        {
            this.Type = type;
            if (Type is FIRRTL.GroundType ground && ground.IsWidthKnown)
            {
                this.ValueString = new string(BitState.X.ToChar(), ground.Width);
            }
        }

        public bool UpdateValue(VarValue value)
        {
            if (Value != null && Value.SameValue(value))
            {
                return false;
            }

            Value = value;
            ValueString = ((BinaryVarValue)Value).BitsToString();
            return true;
        }

        public bool UpdateFrom(ValueType copyFrom)
        {
            return UpdateValue(copyFrom.Value);
        }

        public void SetValueString(string valueString)
        {
            ValueString = valueString;
        }

        public bool IsTrue()
        {
            var binValue = (BinaryVarValue)Value;
            if (binValue.Bits.Length > 1)
            {
                throw new Exception("Value must be a single bit when asking if it's true.");
            }

            return binValue.Bits[0] == BitState.One;
        }

        public string ToBinaryString()
        {
            return ValueString;
        }
    }
}
