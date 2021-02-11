﻿using ChiselDebug.FIRRTL;
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

        internal DirectedIO WithOffsetPosition(Point offset)
        {
            return new DirectedIO(IO, Position + offset, InitialDir);
        }
    }
}
