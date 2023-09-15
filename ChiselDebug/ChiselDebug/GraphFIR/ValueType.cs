using FIRRTL;
using System.Diagnostics;
using System.Numerics;
using VCDReader;

namespace ChiselDebug.GraphFIR
{
    public sealed class ValueType
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

        public bool SameValueAs(ValueType other)
        {
            return Value.SameValue(ref other.Value, IsSigned);
        }

        public BigInteger GetAsBigInt()
        {
            return Value.AsBigInteger(IsSigned);
        }

        public void SetFromBigInt(in BigInteger newValue)
        {
            Value.SetBitsAndExtend(in newValue);
        }

        public ref BinaryVarValue GetValue()
        {
            return ref Value;
        }

        public string ToBinaryString()
        {
            ValueString ??= Value.BitsToString();
            return ValueString;
        }
    }
}
