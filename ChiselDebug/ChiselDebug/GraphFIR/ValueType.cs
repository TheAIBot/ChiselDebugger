using FIRRTL;
using System;
using System.Diagnostics;
using VCDReader;

namespace ChiselDebug.GraphFIR
{
    public struct ValueType
    {
        private readonly GroundType Type;
        internal readonly BinaryVarValue Value;
        private string ValueString;

        public ValueType(GroundType type)
        {
            this.Type = type;
            this.Value = new BinaryVarValue(Type.Width);
            Value.Bits.Fill(BitState.X);

            this.ValueString = null;
        }

        public bool UpdateValue(in BinaryVarValue update)
        {
            if (Value.SameValue(in update, Type is SIntType))
            {
                return false;
            }

            Value.SetBitsAndExtend(in update, Type is SIntType);
            return true;
        }

        public bool UpdateFrom(ValueType copyFrom)
        {
            return UpdateValue(in copyFrom.Value);
        }

        public void UpdateValueString()
        {
            ValueString = Value.BitsToString();
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
