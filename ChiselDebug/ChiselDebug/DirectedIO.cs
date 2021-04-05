using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChiselDebug
{
    public class DirectedIO
    {
        public readonly FIRIO IO;
        public readonly Point Position;
        public readonly MoveDirs InitialDir;

        public DirectedIO(FIRIO io, Point pos, MoveDirs initialDir)
        {
            this.IO = io;
            this.Position = pos;
            this.InitialDir = initialDir;
        }

        public DirectedIO WithOffsetPosition(Point offset)
        {
            return new DirectedIO(IO, Position + offset, InitialDir);
        }

        public static bool operator ==(DirectedIO a, DirectedIO b)
        {
            return a.IO == b.IO &&
                   a.Position == b.Position &&
                   a.InitialDir == b.InitialDir;
        }

        public static bool operator !=(DirectedIO a, DirectedIO b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is DirectedIO dirIO && this == dirIO;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IO, Position, InitialDir);
        }
    }
}
