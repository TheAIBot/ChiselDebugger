﻿using FIRRTL;
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

        public void SetValueString(string valueString)
        {
            ValueString = valueString;
        }

        public string ToBinaryString()
        {
            return ValueString;
        }
    }
}
