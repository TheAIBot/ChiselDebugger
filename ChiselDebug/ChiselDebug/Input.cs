namespace ChiselDebug
{
    public class Input
    {
        public readonly string Name;
        public Connection Con = null;

        public Input(string name)
        {
            this.Name = name;
        }

        public bool IsConnected()
        {
            return Con != null;
        }
    }
}
