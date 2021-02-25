namespace ChiselDebug.GraphFIR
{
    public struct ValueType
    {
        string ValueString;
        SizedType ValueT;

        public ValueType(string valueString, SizedType type)
        {
            ValueString = valueString;
            ValueT = type;
        }

        public string ToBinaryString()
        {
            return ValueString;
        }
    }
}
