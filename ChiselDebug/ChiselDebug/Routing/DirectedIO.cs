using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChiselDebug.Routing
{
    public class DirectedIO
    {
        public readonly FIRIO IO;
        public readonly Point Position;
        public readonly MoveDirs InitialDir;

        public DirectedIO(FIRIO io, Point pos, MoveDirs initialDir)
        {
            IO = io;
            Position = pos;
            InitialDir = initialDir;
        }

        public DirectedIO WithOffsetPosition(Point offset)
        {
            return new DirectedIO(IO, Position + offset, InitialDir);
        }

        public override bool Equals(object obj)
        {
            return obj is DirectedIO dirIO &&
                IO == dirIO.IO &&
                Position == dirIO.Position &&
                InitialDir == dirIO.InitialDir;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IO, Position, InitialDir);
        }
    }
}
