using System;
using System.Collections.Generic;
using System.Text;

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

        public Point GetEndPos()
        {
            return Path[^1];
        }

        public string ToSVGPathString()
        {
            StringBuilder sBuilder = new StringBuilder();

            Point start = Path[0];
            sBuilder.AppendFormat("M {0} {1}", start.X, start.Y);

            for (int i = 1; i < Path.Count; i++)
            {
                //sBuilder.AppendFormat(" L {0} {1}", Path[i].X, Path[i].Y);
                sBuilder.Append(' ');
                if (start.X == Path[i].X)
                {
                    sBuilder.AppendFormat("V {0}", Path[i].Y);
                }
                else
                {
                    sBuilder.AppendFormat("H {0}", Path[i].X);
                }
                start = Path[i];
            }

            return sBuilder.ToString();
        }
    }
}
