using FIRRTL;
using System;
using System.Diagnostics;
using VCDReader;

namespace ChiselDebug.GraphFIR
{
    public struct ValueType
    {
        private readonly GroundType Type;
        private readonly BinaryVarValue Value;
        private string ValueString;

        public ValueType(GroundType type)
        {
            this.Type = type;
            this.Value = new BinaryVarValue(Type.Width);
            Value.Bits.Fill(BitState.X);

            this.ValueString = null;
        }

        public bool UpdateValue(BinaryVarValue update)
        {
            if (Value.SameValue(update, Type is SIntType))
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

        public bool IsInitialized()
        {
            return Type != null;
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
