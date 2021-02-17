using FIRRTL;

namespace ChiselDebug.GraphFIR
{
    public class Input : FIRIO
    {
        public string Name { get; private set; }
        public IFIRType Type { get; private set; }
        public Connection Con = null;

        public Input()
        { }

        public Input(IFIRType type) : this(string.Empty, type)
        { }

        public Input(string name, IFIRType type)
        {
            this.Name = name;
            this.Type = type;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetType(IFIRType type)
        {
            Type = type;
        }

        public bool IsConnected()
        {
            return Con != null;
        }
    }
}
