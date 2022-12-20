using System;
using System.Diagnostics.CodeAnalysis;
#nullable enable

namespace ChiselDebug.GraphFIR.IO
{
    public interface IContainerIO
    {
        public bool TryGetIO(string ioName, [NotNullWhen(true)] out IContainerIO? container);
        public IContainerIO GetIO(string ioName);


        public bool TryGetIO(ReadOnlySpan<string> names, [NotNullWhen(true)] out IContainerIO? container)
        {
            if (names.Length == 0)
            {
                container = this;
                return true;
            }

            if (TryGetIO(names[0], out var innerContainer))
            {
                if (innerContainer is ModuleIO modIO)
                {
                    innerContainer = modIO.Mod;
                }

                return innerContainer.TryGetIO(names.Slice(1), out container);
            }

            container = null;
            return false;
        }
    }
}
