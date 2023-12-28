using ChiselDebug.Utilities;
using System;

namespace ChiselDebug.Routing
{
    internal static class MoveExtensions
    {
        internal static bool IsVertical(this MoveDirs dir)
        {
            return dir.HasFlag(MoveDirs.Up) || dir.HasFlag(MoveDirs.Down);
        }

        internal static bool IsHorizontal(this MoveDirs dir)
        {
            return dir.HasFlag(MoveDirs.Left) || dir.HasFlag(MoveDirs.Right);
        }

        internal static Point MovePoint(this MoveDirs dir, Point pos)
        {
            return dir switch
            {
                MoveDirs.Up => new Point(pos.X, pos.Y - 1),
                MoveDirs.Down => new Point(pos.X, pos.Y + 1),
                MoveDirs.Left => new Point(pos.X - 1, pos.Y),
                MoveDirs.Right => new Point(pos.X + 1, pos.Y),
                _ => throw new Exception($"Can't move point in the direction: {dir}")
            };
        }

        internal static MoveDirs Reverse(this MoveDirs dir)
        {
            return dir switch
            {
                MoveDirs.Up => MoveDirs.Down,
                MoveDirs.Down => MoveDirs.Up,
                MoveDirs.Left => MoveDirs.Right,
                MoveDirs.Right => MoveDirs.Left,
                _ => throw new Exception($"Can't reverse this direction: {dir}")
            };
        }

        internal static int IsFriendlyWire(this MoveDirs dir)
        {
            return ((int)dir >> 4) & 1;
        }

        internal static int IsEnemyWire(this MoveDirs dir)
        {
            return ((int)dir >> 5) & 1;
        }

        internal static int IsWireCorner(this MoveDirs dir)
        {
            return ((int)dir >> 6) & 1;
        }
    }
}
