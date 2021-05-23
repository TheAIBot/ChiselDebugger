using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChiselDebug.Routing
{
    public class WirePath
    {
        internal readonly IOInfo StartIO;
        internal readonly IOInfo EndIO;
        private readonly List<Point> Path = new List<Point>();
        private readonly List<Point> BoardPositions = new List<Point>();
        private readonly List<Point> BoardPosTurns = new List<Point>();
        public readonly bool StartsFromWire;
        private readonly Output ConnectionCondition;

        internal WirePath(IOInfo startIO, IOInfo endIO, List<Point> path, List<Point> boardPositions, List<Point> boardPosTurns, bool startsFromWire)
        {
            this.StartIO = startIO;
            this.EndIO = endIO;
            this.Path = path;
            this.BoardPositions = boardPositions;
            this.BoardPosTurns = boardPosTurns;
            this.StartsFromWire = startsFromWire;
            this.ConnectionCondition = GetConCondition();
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

        internal void RemoveCornersFromBoard(RouterBoard board)
        {
            foreach (var pathPos in Path)
            {
                Point pathPosRel = board.GetRelativeBoardPos(pathPos);
                //board.RemoveAllIncommingMoves(pathPosRel);
                board.AddCellAllowedMoves(pathPosRel, MoveDirs.WireCorner);
            }
        }

        internal void RefineWireStartAndEnd()
        {
            if (Path[0].Y == Path[1].Y)
            {
                Path[1] = new Point(Path[1].X, StartIO.DirIO.Position.Y);
            }
            else
            {
                Path[1] = new Point(StartIO.DirIO.Position.X, Path[1].Y);
            }
            Path[0] = StartIO.DirIO.Position;

            if (!StartsFromWire)
            {
                if (Path[^1].Y == Path[^2].Y)
                {
                    Path[^2] = new Point(Path[^2].X, EndIO.DirIO.Position.Y);
                }
                else
                {
                    Path[^2] = new Point(EndIO.DirIO.Position.X, Path[^2].Y);
                }
                Path[^1] = EndIO.DirIO.Position;
            }
        }

        internal bool CanCoexist(WirePath other)
        {
            HashSet<Point> ownCorners = new HashSet<Point>(BoardPositions);
            if (ownCorners.Overlaps(other.BoardPosTurns))
            {
                return false;
            }

            return true;
        }

        public WirePath CopyWithNewNodes(FIRRTLNode start, FIRIO startIO, FIRRTLNode end, FIRIO endIO)
        {
            DirectedIO startDir = new DirectedIO(startIO, StartIO.DirIO.Position, StartIO.DirIO.InitialDir);
            DirectedIO endDir = new DirectedIO(endIO, EndIO.DirIO.Position, EndIO.DirIO.InitialDir);

            IOInfo startInfo = new IOInfo(start, startDir);
            IOInfo endInfo = new IOInfo(end, endDir);

            return new WirePath(startInfo, endInfo, new List<Point>(Path), new List<Point>(BoardPositions), new List<Point>(BoardPosTurns), StartsFromWire);
        }

        public Point GetEndPos()
        {
            return Path[^1];
        }

        public FIRIO GetStartIO()
        {
            return StartIO.DirIO.IO;
        }

        public FIRIO GetEndIO()
        {
            return EndIO.DirIO.IO;
        }

        public FIRRTLNode GetStartNode()
        {
            return StartIO.Node;
        }

        public FIRRTLNode GetEndNode()
        {
            return EndIO.Node;
        }

        public Output[] GetConnections()
        {
            List<Output> outputCons = new List<Output>();
            foreach (var io in StartIO.DirIO.IO.Flatten())
            {
                if (io is Output output)
                {
                    outputCons.Add(output);
                }
            }
            foreach (var io in EndIO.DirIO.IO.Flatten())
            {
                if (io is Output output)
                {
                    outputCons.Add(output);
                }
            }

            return outputCons.ToArray();
        }

        private Output GetConCondition()
        {
            ScalarIO startScalar = StartIO.DirIO.IO.Flatten().First();
            ScalarIO endScalar = EndIO.DirIO.IO.Flatten().First();
            if (startScalar is Output startOut && endScalar is Input endInput)
            {
                return endInput.GetConnectionCondition(startOut);
            }
            else if (startScalar is Input startInput && endScalar is Output endOutput)
            {
                return startInput.GetConnectionCondition(endOutput);
            }

            return null;
        }

        public bool IsEnabled()
        {
            return ConnectionCondition == null || ConnectionCondition.Value.IsTrue();
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
