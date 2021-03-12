using System;

namespace ChiselDebug.GraphFIR.IO
{
    public interface IContainerIO
    {
        public IContainerIO GetIO(string ioName, bool modulesOnly = false);

        public IContainerIO GetIO(Span<string> names, bool modulesOnly = false)
        {
            if (names.Length == 0)
            {
                return this;
            }

            return GetIO(names[0], modulesOnly).GetIO(names.Slice(1), modulesOnly);
        }
    }
}
