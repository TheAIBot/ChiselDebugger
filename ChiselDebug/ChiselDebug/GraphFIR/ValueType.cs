using FIRRTL;
using System;
using System.Diagnostics;
using VCDReader;

namespace ChiselDebug.GraphFIR
{
    public class ValueType
    {
        private readonly IFIRType Type;
        private BinaryVarValue Value;
        string ValueString = string.Empty;

        public ValueType(IFIRType type)
        {
            this.Type = type;
            if (Type is FIRRTL.GroundType ground && ground.IsWidthKnown)
            {
                this.Value = new BinaryVarValue(ground.Width);
                this.ValueString = new string(BitState.X.ToChar(), ground.Width);
            }
        }

        public bool UpdateValue(BinaryVarValue update)
        {
            if (Value.SameValueZeroExtend(update))
            {
                return false;
            }

            Value.SetBitsZeroExtend(update);
            ValueString = Value.BitsToString();
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

        public BinaryVarValue GetValue()
        {
            return Value;
        }

        public bool IsTrue()
        {
            if (Value.Bits.Length > 1)
            {
                throw new Exception("Value must be a single bit when asking if it's true.");
            }

            return Value.Bits[0] == BitState.One;
        }

        public string ToBinaryString()
        {
            return ValueString;
        }
    }
}
