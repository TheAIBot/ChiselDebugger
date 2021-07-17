using FIRRTL;
using System;
using System.Diagnostics;
using VCDReader;

namespace ChiselDebug.GraphFIR
{
    public struct ValueType
    {
        public readonly bool IsSigned;
        private readonly bool HasInitialized;
        internal BinaryVarValue Value;
        private string ValueString;

        public ValueType(GroundType type)
        {
            this.IsSigned = type is SIntType;
            this.HasInitialized = true;
            this.Value = new BinaryVarValue(type.Width, false);
            Value.SetAllUnknown();

            this.ValueString = null;
        }

        public void UpdateValue(ref BinaryVarValue update)
        {
            Value.SetBitsAndExtend(ref update, IsSigned);
        }

        public void UpdateFrom(ref ValueType copyFrom)
        {
            UpdateValue(ref copyFrom.Value);
        }

        public void UpdateValueString()
        {
            ValueString = Value.BitsToString();
        }

        public bool IsInitialized()
        {
            return HasInitialized;
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
