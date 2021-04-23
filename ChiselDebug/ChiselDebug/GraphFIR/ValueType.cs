using FIRRTL;
using System;
using System.Diagnostics;
using VCDReader;

namespace ChiselDebug.GraphFIR
{
    public class ValueType
    {
        private readonly GroundType Type;
        private BinaryVarValue Value;
        private string ValueString = null;

        public ValueType(GroundType type)
        {
            this.Type = type;
            this.Value = new BinaryVarValue(Type.Width);
            Array.Fill(Value.Bits, BitState.X);
        }

        public bool UpdateValue(BinaryVarValue update)
        {
            if (Value.SameValueZeroExtend(update))
            {
                return false;
            }

            Value.SetBitsAndExtend(update, Type is SIntType);
            return true;
        }

        public bool UpdateFrom(ValueType copyFrom)
        {
            return UpdateValue(copyFrom.Value);
        }

        public void UpdateValueString()
        {
            ValueString = Value.BitsToString();
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
            ValueString = ValueString ?? Value.BitsToString();
            return ValueString;
        }
    }
}
