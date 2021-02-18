using FIRRTL;

namespace ChiselDebug.GraphFIR
{
    public class Output : FIRIO
    {
        public string Name { get; private set; }
        public IFIRType Type { get; private set; }
        public readonly Connection Con;

        public Output()
        { }

        public Output(IFIRType type) : this(string.Empty, type)
        { }

        public Output(string name, IFIRType type)
        {
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
        }

        public void ConnectToInput(Input input)
        {
            Con.ConnectToInput(input);
        }
    }
}
