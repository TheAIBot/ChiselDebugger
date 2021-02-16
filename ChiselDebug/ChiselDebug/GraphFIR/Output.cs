namespace ChiselDebug.GraphFIR
{
    public class Output : FIRIO
    {
        public readonly string Name;
        public readonly Connection Con;

        public Output(string name)
        {
            this.Name = name;
            this.Con = new Connection(this);
        }

        public void ConnectToInput(Input input)
        {
            Con.ConnectToInput(input);
        }
    }
}
