using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Utilities;
using System;

namespace ChiselDebug.Routing
{
    public sealed record DirectedIO(FIRIO IO, Point Position, MoveDirs InitialDir)
    {
        public DirectedIO WithOffsetPosition(Point offset)
        {
            return new DirectedIO(IO, Position + offset, InitialDir);
        }
    }
}
