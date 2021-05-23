﻿using System;

namespace ChiselDebug.GraphFIR.IO
{
    public interface IContainerIO
    {
        public bool TryGetIO(string ioName, out IContainerIO container);
        public IContainerIO GetIO(string ioName);


        public bool TryGetIO(ReadOnlySpan<string> names, out IContainerIO container)
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
        public IContainerIO GetIO(ReadOnlySpan<string> names)
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
