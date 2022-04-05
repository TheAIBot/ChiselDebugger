using ChiselDebug.GraphFIR.Components;
using ChiselDebug.GraphFIR.IO;
using ChiselDebug.Placing;
using ChiselDebug.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

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

        public void UpdateIOFromNode(FIRRTLNode node, DirectedIO[] inputOffsets, DirectedIO[] outputOffsets)
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

        public bool TryPathLines(PlacementInfo placements, CancellationToken cancelToken, out List<WirePath> wiresPaths)
        {
            Dictionary<Line, int> repathCounter = new Dictionary<Line, int>();

            List<LineInfo> sortedLines = new List<LineInfo>();
            foreach (LineInfo line in Connections.GetAllConnectionLines(placements))
            {
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
                if (cancelToken.IsCancellationRequested)
                {
                    wiresPaths = null;
                    return false;
                }

                LineInfo line = linesPriority.Dequeue();

                Rectangle? startRect = null;
                Rectangle? endRect = null;
                if (placements.UsedSpace.ContainsKey(line.Start.Node))
                {
                    startRect = placements.UsedSpace[line.Start.Node];
                }
                if (placements.UsedSpace.ContainsKey(line.End.Node))
                {
                    endRect = placements.UsedSpace[line.End.Node];
                }

                WirePath path;
                if (!TryPathLine(board, line.Start, line.End, startRect, endRect, paths, out path))
                {
                    Debug.WriteLine("Failed to find a path:");
                    wiresPaths = null;
                    return false;
                }

                Debug.Assert(path.StartIO == line.Start && path.EndIO == line.End);
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
                    LineInfo repathline = new LineInfo(repath.StartIO, repath.EndIO);
                    if (repathCounter[repathline.GetLine()]++ >= 20)
                    {
                        continue;
                    }

                    paths.Remove(repath);

                    linesPriority.Enqueue(repathline);
                }
                paths.Add(path);
            }

            RefineWirePaths(board, paths);
            wiresPaths = paths;
            return true;
        }

        private readonly struct MoveData
        {
            public readonly MoveDirs Dir;
            public readonly MoveDirs RevDir;
            public readonly Point DirVec;

            public MoveData(MoveDirs dir)
            {
                this.Dir = dir;
                this.RevDir = dir.Reverse();
                this.DirVec = Dir.MovePoint(Point.Zero);
            }
        }

        private bool TryPathLine(RouterBoard board, IOInfo start, IOInfo end, Rectangle? startRect, Rectangle? endRect, List<WirePath> allPaths, out WirePath wirePath)
        {
            board.ReloadCheckpoint();

            Point relativeStart = board.GetRelativeBoardPos(start.DirIO.Position);
            int relativeStartIndex = board.CellIndex(relativeStart);
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

            int closestFriendDistance = int.MaxValue;
            Point closestFriend = new Point(int.MaxValue, int.MaxValue);
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

                path.PlaceOnBoard(board, wireType, relativeEnd, ref closestFriend, ref closestFriendDistance);
            }

            ref ScorePath startScore = ref board.GetCellScorePath(relativeEnd);
            startScore = new ScorePath(0, MoveDirs.None);

            PriorityQueue<int, int> toSee = new PriorityQueue<int, int>();
            toSee.Enqueue(board.CellIndex(relativeEnd), 0);

            ReadOnlySpan<MoveData> moves = new MoveData[]
            {
                new MoveData(MoveDirs.Up),
                new MoveData(MoveDirs.Left),
                new MoveData(MoveDirs.Right),
                new MoveData(MoveDirs.Down)
            };

            bool canEndEarly = end.DirIO.IO is Sink endInput && endInput.GetConnections().Length == 1;
            while (toSee.Count > 0)
            {
                int currentIndex = toSee.Dequeue();

                if (currentIndex == relativeStartIndex)
                {
                    wirePath = board.GetPath(relativeEnd, relativeStart, start, end, false);
                    return true;
                }

                ScorePath currentScorePath = board.GetCellScorePath(currentIndex);
                MoveDirs allowedMoves = board.GetCellMoves(currentIndex);

                if (canEndEarly && allowedMoves.HasFlag(MoveDirs.FriendWire))
                {
                    wirePath = board.GetPath(relativeEnd, board.CellFromIndex(currentIndex), start, end, true);
                    return true;
                }

                bool onEnemyWire = allowedMoves.HasFlag(MoveDirs.EnemyWire);
                for (int i = 0; i < moves.Length; i++)
                {
                    ref readonly MoveData move = ref moves[i];
                    if (allowedMoves.HasFlag(move.Dir))
                    {
                        int neighborIndex = currentIndex + board.CellIndex(move.DirVec);
                        ref ScorePath neighborScore = ref board.GetCellScorePath(neighborIndex);

                        MoveDirs neighborMoves = board.GetCellMoves(neighborIndex);
                        bool moveToEnemyWire = neighborMoves.HasFlag(MoveDirs.EnemyWire);
                        bool moveToFriendWire = neighborMoves.HasFlag(MoveDirs.FriendWire);
                        bool moveToWireCorner = neighborMoves.HasFlag(MoveDirs.WireCorner);
                        bool isTurning = currentScorePath.DirFrom != MoveDirs.None && move.RevDir != currentScorePath.DirFrom;

                        ScorePath neighborScoreFromCurrent = currentScorePath.Move(move.RevDir, onEnemyWire, moveToEnemyWire, moveToFriendWire, moveToWireCorner, isTurning);
                        if (neighborScoreFromCurrent.IsBetterScoreThan(neighborScore))
                        {
                            Point neighborPos = board.CellFromIndex(neighborIndex);
                            int distToGoal = Point.ManhattanDistance(in neighborPos, in relativeStart);
                            int distToClosestFriend = Point.ManhattanDistance(in neighborPos, in closestFriend);
                            int dist = Math.Min(distToGoal, distToClosestFriend);

                            toSee.Enqueue(neighborIndex, neighborScoreFromCurrent.GetTotalScore() + dist);
                            neighborScore = neighborScoreFromCurrent;
                        }
                    }
                }
            }

            Debug.WriteLine(board.BoardAllowedMovesToString(relativeEnd, relativeStart));
            Debug.WriteLine(board.BoardStateToString(relativeEnd, relativeStart));
            wirePath = null;
            return false;
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
