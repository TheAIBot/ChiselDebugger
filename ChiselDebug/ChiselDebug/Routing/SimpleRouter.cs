using ChiselDebug.GraphFIR;
using ChiselDebug.GraphFIR.IO;
using PriorityQueue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChiselDebug.Routing
{
    public class SimpleRouter
    {
        private readonly ConnectionsHandler Connections;
        private readonly HashSet<FIRRTLNode> MissingNodeIO;

        public SimpleRouter(Module mod)
        {
            this.Connections = new ConnectionsHandler(mod);
            this.MissingNodeIO = new HashSet<FIRRTLNode>(mod.GetAllNodesIncludeModule());
            MissingNodeIO.RemoveWhere(x => x is INoPlaceAndRoute);
        }

        private record LineInfo(IOInfo Start, IOInfo End)
        {
            public int GetScore()
            {
                return new Line(Start.DirIO.Position, End.DirIO.Position).GetManhattanDistance();
            }

            public Line GetLine()
            {
                return new Line(Start.DirIO.Position, End.DirIO.Position);
            }
        }

        public void UpdateIOFromNode(FIRRTLNode node, List<DirectedIO> inputOffsets, List<DirectedIO> outputOffsets)
        {
            Connections.UpdateIOFromNode(node, inputOffsets, outputOffsets);

            lock (MissingNodeIO)
            {
                MissingNodeIO.Remove(node);
            }
        }

        public bool IsReadyToRoute()
        {
            return MissingNodeIO.Count == 0;
        }

        public List<WirePath> PathLines(PlacementInfo placements)
        {
            try
            {
                int rerouteCount = 0;
                Dictionary<Line, int> repathCounter = new Dictionary<Line, int>();

                
                List<LineInfo> sortedLines = new List<LineInfo>();
                foreach ((IOInfo start, IOInfo end) in Connections.GetAllConnectionLines(placements))
                {
                    LineInfo line = new LineInfo(start, end);
                    sortedLines.Add(line);
                    repathCounter.Add(line.GetLine(), 0);
                }

                Queue<LineInfo> linesPriority = new Queue<LineInfo>(sortedLines.OrderBy(x => x.GetScore()));

                RouterBoard board = new RouterBoard(placements.SpaceNeeded);
                board.PrepareBoard(placements.UsedSpace.Values.ToList());
                board.CreateCheckpoint();

                List<WirePath> paths = new List<WirePath>();
                while (linesPriority.Count > 0)
                {
                    //Debug.WriteLine(linesPriority.Count);
                    (IOInfo start, IOInfo end) = linesPriority.Dequeue();

                    Rectangle? startRect = null;
                    Rectangle? endRect = null;
                    if (placements.UsedSpace.ContainsKey(start.Node))
                    {
                        startRect = placements.UsedSpace[start.Node];
                    }
                    if (placements.UsedSpace.ContainsKey(end.Node))
                    {
                        endRect = placements.UsedSpace[end.Node];
                    }

                    WirePath path = PathLine(board, start, end, startRect, endRect, paths);
                    Debug.Assert(path.StartIO == start && path.EndIO == end);

                    List<WirePath> needsRepathing = new List<WirePath>();
                    foreach (var oldPath in paths)
                    {
                        if (!path.CanCoexist(oldPath))
                        {
                            needsRepathing.Add(oldPath);
                        }
                    }
                    foreach (var repath in needsRepathing)
                    {
                        LineInfo line = new LineInfo(repath.StartIO, repath.EndIO);
                        if (repathCounter[line.GetLine()]++ >= 20)
                        {
                            continue;
                        }

                        paths.Remove(repath);

                        linesPriority.Enqueue(line);
                    }
                    paths.Add(path);
                }

                RefineWirePaths(board, paths);
                return paths;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
                throw;
            }
        }

        private readonly struct MoveData
        {
            public readonly MoveDirs Dir;
            public readonly MoveDirs RevDir;
            public readonly Point DirVec;
            public readonly Point RevDirVec;

            public MoveData(MoveDirs dir)
            {
                this.Dir = dir;
                this.RevDir = dir.Reverse();
                this.DirVec = Dir.MovePoint(Point.Zero);
                this.RevDirVec = RevDir.MovePoint(Point.Zero);
            }
        }

        private WirePath PathLine(RouterBoard board, IOInfo start, IOInfo end, Rectangle? startRect, Rectangle? endRect, List<WirePath> allPaths)
        {
            board.ReloadCheckpoint();

            Point relativeStart = board.GetRelativeBoardPos(start.DirIO.Position);
            Point relativeEnd = board.GetRelativeBoardPos(end.DirIO.Position);

            //Start position may lie inside the component rectangle
            //and therefore the is no way to reach it.
            //This here makes a straight path out of the component
            //so it's possible to find a path.
            if (endRect.HasValue)
            {
                Rectangle endRectRelative = board.GetRelativeBoard(endRect.Value);
                Point endGo = relativeEnd;
                MoveDirs allowedDir = end.DirIO.InitialDir.Reverse();
                do
                {
                    board.SetCellAllowedMoves(endGo, allowedDir);
                    endGo = allowedDir.MovePoint(endGo);
                } while (endRectRelative.Within(endGo));
            }
            else
            {
                board.SetCellAllowedMoves(relativeEnd, end.DirIO.InitialDir.Reverse());
            }

            if (startRect.HasValue)
            {
                Rectangle startRectRelative = board.GetRelativeBoard(startRect.Value);
                Point startGo = relativeStart;
                MoveDirs allowedDir = start.DirIO.InitialDir.Reverse();
                do
                {
                    board.SetCellAllowedMoves(startGo, allowedDir);
                    startGo = allowedDir.Reverse().MovePoint(startGo);
                } while (startRectRelative.Within(startGo));
            }

            foreach (var path in allPaths)
            {
                MoveDirs wireType;
                if (path.StartIO.DirIO.Position == start.DirIO.Position || 
                    path.EndIO.DirIO.Position == end.DirIO.Position)
                {
                    wireType = MoveDirs.FriendWire;
                }
                else
                {
                    wireType = MoveDirs.EnemyWire;
                }

                path.PlaceOnBoard(board, wireType);
            }

            ref ScorePath startScore = ref board.GetCellScorePath(relativeEnd);
            startScore = new ScorePath(0, MoveDirs.None);

            PriorityQueue<int> toSee = new PriorityQueue<int>();
            toSee.Enqueue(board.CellIndex(relativeEnd), 0);

            ReadOnlySpan<MoveData> moves = new MoveData[] 
            { 
                new MoveData(MoveDirs.Up),
                new MoveData(MoveDirs.Down),
                new MoveData(MoveDirs.Left),
                new MoveData(MoveDirs.Right)
            };

            bool canEndEarly = ((Input)end.DirIO.IO).GetConnections().Length == 1;
            while (toSee.Count > 0)
            {
                int currentIndex = toSee.Dequeue();
                Point current = board.CellFromIndex(currentIndex);

                if (current == relativeStart)
                {
                    return board.GetPath(relativeEnd, relativeStart, start, end, false);
                }

                ScorePath currentScorePath = board.GetCellScorePath(currentIndex);
                MoveDirs allowedMoves = board.GetCellMoves(currentIndex);

                if (allowedMoves.HasFlag(MoveDirs.FriendWire) && canEndEarly)
                {
                    return board.GetPath(relativeEnd, current, start, end, true);
                }

                bool onEnemyWire = allowedMoves.HasFlag(MoveDirs.EnemyWire);
                bool onFriendWire = allowedMoves.HasFlag(MoveDirs.FriendWire);
                bool onWireCorner = allowedMoves.HasFlag(MoveDirs.WireCorner);
                for (int i = 0; i < moves.Length; i++)
                {
                    if (allowedMoves.HasFlag(moves[i].Dir))
                    {
                        Point neighborPos = current + moves[i].DirVec;
                        int neighbordIndex = board.CellIndex(neighborPos);
                        ref ScorePath neighborScore = ref board.GetCellScorePath(neighbordIndex);

                        //Penalty for turning while on another wire
                        bool isTurningOnEnemyWire = onEnemyWire && currentScorePath.DirFrom != MoveDirs.None && moves[i].Dir != currentScorePath.DirFrom.Reverse();

                        ScorePath neighborScoreFromCurrent = currentScorePath.Move(moves[i].RevDir, onEnemyWire, onFriendWire, onWireCorner, isTurningOnEnemyWire);
                        if (neighborScoreFromCurrent.IsBetterScoreThan(neighborScore))
                        {
                            Point diff = (neighborPos - relativeStart).Abs();
                            int dist = diff.X + diff.Y;

                            toSee.Enqueue(neighbordIndex, neighborScoreFromCurrent.GetTotalScore() + dist);
                            neighborScore = neighborScoreFromCurrent;
                        }
                    }
                }
            }

            Debug.WriteLine(board.BoardAllowedMovesToString(relativeEnd, relativeStart));
            Debug.WriteLine(board.BoardStateToString(relativeEnd, relativeStart));
            throw new Exception("Failed to find a path.");
        }

        private void RefineWirePaths(RouterBoard board, List<WirePath> paths)
        {
            //The start and end point of a wire path will not end at their correct
            //positions. This is because the path follows the center of the cells
            //in the board, but the start and end may not be position in the middle
            //of a cell. Therefore the start and end points are moved to their correct
            //positions and the points just before start/end are also moved so the
            //lines are only horizontal and vertical.

            //Problem is that moving these points will sometimes make the lines
            //overlap. The solution here is to detect when wires are too close and
            //in those cases allow for diagonal lines.

            //1. Place each refined wire path position on a board
            //2. Detect wires that are too close and separate them
            //3. Update wire path with new separated positions


            foreach (var path in paths)
            {
                path.RefineWireStartAndEnd();
            }

            Point[] wirePosBoard = new Point[board.CellsWide * board.CellsHigh];
            foreach (var path in paths)
            {
                for (int i = 0; i < path.BoardPosTurns.Count; i++)
                {
                    wirePosBoard[path.BoardPosTurns[i]] = path.Path[i + 1];
                }
            }

            ReadOnlySpan<MoveData> moves = new MoveData[]
            {
                new MoveData(MoveDirs.Up),
                new MoveData(MoveDirs.Down),
                new MoveData(MoveDirs.Left),
                new MoveData(MoveDirs.Right)
            };

            foreach (var path in paths)
            {
                for (int i = 0; i < path.BoardPosTurns.Count; i++)
                {
                    int index = path.BoardPosTurns[i];
                    Point wirePos = wirePosBoard[index];
                    if (wirePos == Point.Zero)
                    {
                        continue;
                    }

                    foreach (var moveDir in moves)
                    {
                        int neighborIndex = index + board.CellIndex(moveDir.DirVec.X, moveDir.DirVec.Y);
                        if (neighborIndex < 0 || neighborIndex >= wirePosBoard.Length)
                        {
                            continue;
                        }
                        if (path.BoardPosTurns.Contains(neighborIndex))
                        {
                            continue;
                        }

                        Point neighborWirePos = wirePosBoard[neighborIndex];
                        if (neighborWirePos == Point.Zero)
                        {
                            continue;
                        }

                        Point distance = (wirePos - neighborWirePos).Abs();
                        if (moveDir.Dir.IsHorizontal() && distance.X < RouterBoard.MinSeparation)
                        {
                            int toSep = Math.Max(1, (RouterBoard.MinSeparation - distance.X) / 2);
                            wirePosBoard[index].X -= moveDir.DirVec.X * toSep;
                            wirePosBoard[neighborIndex].X += moveDir.DirVec.X * toSep;
                        }
                        else if (moveDir.Dir.IsVertical() && distance.Y < RouterBoard.MinSeparation)
                        {
                            int toSep = Math.Max(1, (RouterBoard.MinSeparation - distance.Y) / 2);
                            wirePosBoard[index].Y -= moveDir.DirVec.Y * toSep;
                            wirePosBoard[neighborIndex].Y += moveDir.DirVec.Y * toSep;
                        }
                    }
                }
            }

            foreach (var path in paths)
            {
                for (int i = 1; i < path.BoardPosTurns.Count - 1; i++)
                {
                    path.Path[i + 1] = wirePosBoard[path.BoardPosTurns[i]];
                }
            }
        }
    }
}
