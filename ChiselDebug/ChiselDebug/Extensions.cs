using ChiselDebug.GraphFIR.IO;
using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ChiselDebug
{
    public static class Extensions
    {
        public static bool IsVertical(this MoveDirs dir)
        {
            return dir.HasFlag(MoveDirs.Up) || dir.HasFlag(MoveDirs.Down);
        }

        public static bool IsHorizontal(this MoveDirs dir)
        {
            return dir.HasFlag(MoveDirs.Left) || dir.HasFlag(MoveDirs.Right);
        }
    }
}
