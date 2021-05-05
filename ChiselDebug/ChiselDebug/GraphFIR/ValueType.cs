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

        public void UpdateValue(in BinaryVarValue update)
        {
            Value.SetBitsAndExtend(in update, Type is SIntType);
        }

        public void UpdateFrom(ref ValueType copyFrom)
        {
            UpdateValue(in copyFrom.Value);
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
