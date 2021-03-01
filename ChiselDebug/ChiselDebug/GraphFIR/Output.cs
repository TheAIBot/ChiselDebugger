using FIRRTL;

namespace ChiselDebug.GraphFIR
{
    public class Output : FIRIO
    {
        public readonly FIRRTLNode Node;
        public string Name { get; private set; }
        public IFIRType Type { get; private set; }
        public readonly Connection Con;

        public Output(FIRRTLNode node, IFIRType type) : this(node, string.Empty, type)
        { }

        public Output(FIRRTLNode node, string name, IFIRType type)
        {
            this.Node = node;
            this.Name = name;
            this.Type = type;
            this.Con = new Connection(this);
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetType(IFIRType type)
        {
            Type = type;
            Con.Value = new ValueType(type);
        }

        public void ConnectToInput(Input input)
        {
            Con.ConnectToInput(input);
        }

        public void InferType()
        {
            if (Node != null && Type is UnknownType)
            {
                Node.InferType();
            }
        }
    }
}
