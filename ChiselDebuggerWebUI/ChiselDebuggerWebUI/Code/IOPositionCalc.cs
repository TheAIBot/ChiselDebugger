using ChiselDebug;
using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebuggerWebUI.Code
{
    internal static class IOPositionCalc
    {
        private const int MaxSpaceBetweenIO = 40;

        internal static List<DirectedIO> EvenVertical<T>(int height, T[] io, int fixedX, int startY) where T : FIRIO
        {
            int usableHeight = height - startY * 2;
            int spaceBetweenIO;
            if (io.Length <= 1)
            {
                startY += usableHeight / 2;
                spaceBetweenIO = 0;
            }
            else
            {
                int spacersNeeded = io.Length - 1;
                int possibleSpaceBetweenIO = usableHeight / spacersNeeded;
                spaceBetweenIO = Math.Min(MaxSpaceBetweenIO, possibleSpaceBetweenIO);
                int usedSpace = spaceBetweenIO * spacersNeeded;

                startY += (usableHeight - usedSpace) / 2;
            }

            List<DirectedIO> posIO = new List<DirectedIO>();
            for (int i = 0; i < io.Length; i++)
            {
                int y = startY + spaceBetweenIO * i;

                Point pos = new Point(fixedX, y);
                posIO.Add(new DirectedIO(io[i], pos, MoveDirs.Right));
            }

            return posIO;
        }
    }
}
