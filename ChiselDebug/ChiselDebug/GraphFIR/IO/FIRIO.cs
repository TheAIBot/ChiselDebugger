using System.Text;

namespace ChiselDebug.GraphFIR.IO
{
    public abstract class FIRIO : IContainerIO
    {
        public string Name { get; private set; }
        public bool IsAnonymous => Name == string.Empty;
        public IOBundle Bundle { get; private set; } = null;
        public bool IsPartOfBundle => Bundle != null;

        public FIRIO(string name)
        {
            this.Name = name;
        }

        public void SetName(string name)
        {
            Name = name;
        }

        public void SetBundle(IOBundle bundle)
        {
            Bundle = bundle;
        }

        public abstract void ConnectToInput(FIRIO input, bool allowPartial = false, bool asPassive = false);
        public abstract FIRIO Flip();
        public abstract IContainerIO GetIO(string ioName, bool modulesOnly = false);
    }
}
