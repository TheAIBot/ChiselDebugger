using FIRRTL;
using System;
using System.Diagnostics;
using System.Numerics;
using VCDReader;

namespace ChiselDebug.GraphFIR
{
    public class ValueType
    {
        public readonly bool IsSigned;
        internal BinaryVarValue Value;
        private string ValueString;

        public ValueType(GroundType type)
        {
            this.IsSigned = type is SIntType;
            this.Value = new BinaryVarValue(type.Width, false);
            Value.SetAllUnknown();

            this.ValueString = null;
        }

        public void OverrideValue(ref BinaryVarValue update)
        {
            Value = update;
        }

        public void UpdateValue(ref BinaryVarValue update)
        {
            Value.SetBitsAndExtend(ref update, IsSigned);
        }

        public void UpdateFrom(ValueType copyFrom)
        {
            UpdateValue(ref copyFrom.Value);
        }

        public void UpdateValueString()
        {
            ValueString = Value.BitsToString();
        }

        public bool IsTrue()
        {
            Debug.Assert(Value.Bits.Length == 1, "Connection condition must have a width of 1.");
            return Value.Bits[0] == BitState.One;
        }

        public BigInteger GetAsBigInt()
        {
            return Value.AsBigInteger(IsSigned);
        }

        public void SetFromBigInt(BigInteger newValue)
        {
            Value.SetBitsAndExtend(newValue, IsSigned);
        }

        public string ToBinaryString()
        {
            ValueString = ValueString ?? Value.BitsToString();
            return ValueString;
        }
    }
}
