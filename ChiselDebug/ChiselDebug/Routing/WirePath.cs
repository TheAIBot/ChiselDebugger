using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Utilities;
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
        internal readonly List<Point> Path;
        private readonly List<int> BoardPositions;
        internal readonly List<int> BoardPosTurns;
        public readonly bool StartsFromWire;
        private readonly Source FromIO;
        private readonly Sink ToIO;

        internal WirePath(IOInfo startIO, IOInfo endIO, List<Point> path, List<int> boardPositions, List<int> boardPosTurns, bool startsFromWire)
        {
            this.StartIO = startIO;
            this.EndIO = endIO;
            this.Path = path;
            this.BoardPositions = boardPositions;
            this.BoardPosTurns = boardPosTurns;
            this.StartsFromWire = startsFromWire;
            (this.FromIO, this.ToIO) = GetConCondition();
        }

        internal void PlaceOnBoard(RouterBoard board, MoveDirs move, Point startPos, ref Point closestFriend, ref int closestFriendDistance)
        {
            if (move == MoveDirs.FriendWire)
            {
                foreach (var posIndex in BoardPositions)
                {
                    board.AddCellAllowedMoves(posIndex, move);
                    Point pos = board.CellFromIndex(posIndex);
                    int distance = Point.ManhattanDistance(startPos, pos);
                    if (distance < closestFriendDistance)
                    {
                        closestFriend = pos;
                        closestFriendDistance = distance;
                    }
                }
            }
            else
            {
                foreach (var posIndex in BoardPositions)
                {
                    board.AddCellAllowedMoves(posIndex, move);
                }

                foreach (var pos in BoardPosTurns)
                {
                    board.AddCellAllowedMoves(pos, MoveDirs.WireCorner);
                }
            }
        }

        internal void RefineWireStartAndEnd()
        {
            Point RefineEndPoint(Point pathBeforeEnd, Point pathEnd, Point pathEndGoal)
            {
                if (pathEnd.Y == pathBeforeEnd.Y)
                {
                    return new Point(pathBeforeEnd.X, pathEndGoal.Y);
                }
                else
                {
                    return new Point(pathEndGoal.X, pathBeforeEnd.Y);
                }
            }

            if (Path.Count > 2)
            {
                Path[1] = RefineEndPoint(Path[1], Path[0], EndIO.DirIO.Position);
            }
            Path[0] = EndIO.DirIO.Position;

            if (!StartsFromWire)
            {
                if (Path.Count > 2)
                {
                    Path[^2] = RefineEndPoint(Path[^2], Path[^1], StartIO.DirIO.Position);
                }
                Path[^1] = StartIO.DirIO.Position;
            }
        }

        internal bool CanCoexist(WirePath other)
        {
            if (EndIO.DirIO.Position == other.EndIO.DirIO.Position ||
                StartIO.DirIO.Position == other.StartIO.DirIO.Position)
            {
                return true;
            }

            HashSet<int> ownPoses = new HashSet<int>(BoardPositions);

            //Test if part of path follow each other
            bool prevCollided = false;
            bool anyCollision = false;
            foreach (var pos in other.BoardPositions)
            {
                if (ownPoses.Contains(pos))
                {
                    if (prevCollided)
                    {
                        return false;
                    }

                    prevCollided = true;
                    anyCollision = true;
                }
                else
                {
                    prevCollided = false;
                }
            }

            //If paths do not cross each other once then there can be no problem
            if (!anyCollision)
            {
                return true;
            }

            //Test if other path turns on this path
            foreach (var turn in other.BoardPosTurns)
            {
                if (ownPoses.Contains(turn))
                {
                    return false;
                }
            }

            //Test if this path has turns on the others path
            HashSet<int> otherPoses = new HashSet<int>(other.BoardPositions);
            foreach (var turn in BoardPosTurns)
            {
                if (otherPoses.Contains(turn))
                {
                    return false;
                }
            }

            return true;
        }

        public WirePath CopyWithNewNodes(FIRRTLNode start, FIRIO startIO, FIRRTLNode end, FIRIO endIO)
        {
            DirectedIO startDir = new DirectedIO(startIO, StartIO.DirIO.Position, StartIO.DirIO.InitialDir);
            DirectedIO endDir = new DirectedIO(endIO, EndIO.DirIO.Position, EndIO.DirIO.InitialDir);

            IOInfo startInfo = new IOInfo(start, startDir);
            IOInfo endInfo = new IOInfo(end, endDir);

            return new WirePath(startInfo, endInfo, new List<Point>(Path), new List<int>(BoardPositions), new List<int>(BoardPosTurns), StartsFromWire);
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

        private (Source, Sink) GetConCondition()
        {
            ScalarIO startScalar = StartIO.DirIO.IO.Flatten().First();
            ScalarIO endScalar = EndIO.DirIO.IO.Flatten().First();
            if (startScalar is Source startOut && endScalar is Sink endInput)
            {
                return (startOut, endInput);
            }
            else if (startScalar is Sink startInput && endScalar is Source endOutput)
            {
                return (endOutput, startInput);
            }
            else
            {
                throw new Exception();
            }
        }

        public bool IsEnabled()
        {
            return ToIO.GetEnabledSource() == FromIO;
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
                else if (start.Y == Path[i].Y)
                {
                    sBuilder.AppendFormat("H {0}", Path[i].X);
                }
                else
                {
                    sBuilder.AppendFormat("L {0} {1}", Path[i].X, Path[i].Y);
                }
                start = Path[i];
            }

            return sBuilder.ToString();
        }
    }
}
