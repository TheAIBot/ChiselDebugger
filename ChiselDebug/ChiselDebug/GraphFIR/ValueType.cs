using FIRRTL;
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

        public void SetValue(VarValue value)
        {
            Value = value;
            ValueString = ((BinaryVarValue)Value).BitsToString();
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
