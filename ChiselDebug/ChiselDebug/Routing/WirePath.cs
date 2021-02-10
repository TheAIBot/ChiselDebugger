using System;
using System.Collections.Generic;

namespace ChiselDebug.Routing
{
    public class WirePath
    {
        private readonly List<Point> Path = new List<Point>();
        public readonly bool StartsFromWire;

        public WirePath(List<Point> path, bool startsFromWire)
        {
            this.Path = path;
            this.StartsFromWire = startsFromWire;
        }

        internal void PlaceOnBoard(RouterBoard board, MoveDirs move)
        {
            Point wireStart = board.GetRelativeBoardPos(Path[0]);
            for (int i = 1; i < Path.Count; i++)
            {
                Point wireEnd = board.GetRelativeBoardPos(Path[i]);
                int wireStartX = Math.Min(wireStart.X, wireEnd.X);
                int wireStartY = Math.Min(wireStart.Y, wireEnd.Y);

                int wireEndX = Math.Max(wireStart.X, wireEnd.X);
                int wireEndY = Math.Max(wireStart.Y, wireEnd.Y);
                for (int y = wireStartY; y <= wireEndY; y++)
                {
                    for (int x = wireStartX; x <= wireEndX; x++)
                    {
                        board.AddCellAllowedMoves(new Point(x, y), move);
                    }
                }

                wireStart = wireEnd;
            }
        }

        internal void PlaceCornersOnBoard(RouterBoard board)
        {
            foreach (var pathPos in Path)
            {
                Point pathPosRel = board.GetRelativeBoardPos(pathPos);
                board.RemoveAllIncommingMoves(pathPosRel);
            }
        }
    }
}
