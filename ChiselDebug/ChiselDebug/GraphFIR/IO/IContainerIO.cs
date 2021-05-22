using System;

namespace ChiselDebug.GraphFIR.IO
{
    public interface IContainerIO
    {
        public bool TryGetIO(string ioName, out IContainerIO container);
        public IContainerIO GetIO(string ioName);

        public IContainerIO GetIO(Span<string> names)
        {
            if (names.Length == 0)
            {
                return this;
            }
            IContainerIO container = GetIO(names[0]);
            if (container is ModuleIO modIO)
            {
                container = modIO.Mod;
            }

            return container.GetIO(names.Slice(1));
        }
    }
}
