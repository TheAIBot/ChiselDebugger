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

        public SimpleRouter(ConnectionsHandler connections)
        {
            this.Connections = connections;
        }

        private record LineInfo(IOInfo Start, IOInfo End)
        {
            public int GetScore()
            {
                return new Line(Start.DirIO.Position, End.DirIO.Position).GetManhattanDistance();
            }
        }

        public List<WirePath> PathLines(PlacementInfo placements)
        {
            try
            {
                PriorityQueue<LineInfo, int> linesPriority = new PriorityQueue<LineInfo, int>();
                foreach ((IOInfo start, IOInfo end) in Connections.GetAllConnectionLines(placements))
                {
                    LineInfo line = new LineInfo(start, end);
                    linesPriority.Enqueue(line, line.GetScore());
                }

                RouterBoard board = new RouterBoard(placements.SpaceNeeded);
                board.PrepareBoard(placements.UsedSpace.Values.ToList());
                board.CreateCheckpoint();

                Dictionary<Point, List<WirePath>> startPosPaths = new Dictionary<Point, List<WirePath>>();
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



                    WirePath path = PathLine(board, start, end, startRect, endRect, startPosPaths);

                    List<WirePath> needsRepathing = new List<WirePath>();
                    foreach (var oldPath in paths)
                    {
                        if (oldPath.EndIO.DirIO.Position == path.EndIO.DirIO.Position)
                        {
                            continue;
                        }
                        if (!path.CanCoexist(oldPath))
                        {
                            needsRepathing.Add(oldPath);
                        }
                    }
                    foreach (var repath in needsRepathing)
                    {
                        paths.Remove(repath);
                        startPosPaths[repath.EndIO.DirIO.Position].Remove(repath);

                        LineInfo line = new LineInfo(repath.EndIO, repath.StartIO);
                        linesPriority.Enqueue(line, line.GetScore());
                    }
                    paths.Add(path);

                    if (startPosPaths.TryGetValue(start.DirIO.Position, out List<WirePath> startPaths))
                    {
                        startPaths.Add(path);
                    }
                    else
                    {
                        List<WirePath> startPdwaaths = new List<WirePath>();
                        startPdwaaths.Add(path);
                        startPosPaths.Add(start.DirIO.Position, startPdwaaths);
                    }
                }

                foreach (var path in paths)
                {
                    path.RefineWireStartAndEnd();
                }

                return paths;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
                throw;
            }
        }

        private WirePath PathLine(RouterBoard board, IOInfo start, IOInfo end, Rectangle? startRect, Rectangle? endRect, Dictionary<Point, List<WirePath>> allPaths)
        {
            board.PrepareForSearchFromCheckpoint();

            Point relativeStart = board.GetRelativeBoardPos(end.DirIO.Position);
            Point relativeEnd = board.GetRelativeBoardPos(start.DirIO.Position);

            //Start position may lie inside the component rectangle
            //and therefore the is no way to reach it.
            //This here makes a straight path out of the component
            //so it's possible to find a path.
            if (endRect.HasValue)
            {
                Rectangle endRectRelative = board.GetRelativeBoard(endRect.Value);
                Point endGo = relativeStart;
                MoveDirs allowedDir = end.DirIO.InitialDir.Reverse();
                do
                {
                    board.SetCellAllowedMoves(endGo, allowedDir);
                    endGo = allowedDir.MovePoint(endGo);
                } while (endRectRelative.Within(endGo));
            }
            else
            {
                board.SetCellAllowedMoves(relativeStart, end.DirIO.InitialDir.Reverse());
            }

            if (startRect.HasValue)
            {
                Rectangle endRectRelative = board.GetRelativeBoard(startRect.Value).ResizeCentered(1);
                Point endGo = relativeEnd;
                MoveDirs allowedDir = start.DirIO.InitialDir.Reverse();
                do
                {
                    board.SetCellAllowedMoves(endGo, allowedDir);
                    endGo = allowedDir.Reverse().MovePoint(endGo);
                } while (endRectRelative.Within(endGo));
            }

            foreach (var keyValue in allPaths)
            {
                MoveDirs wireType;
                if (keyValue.Key == start.DirIO.Position)
                {
                    wireType = MoveDirs.FriendWire;
                }
                else
                {
                    wireType = MoveDirs.EnemyWire;
                }

                foreach (var wirePath in keyValue.Value)
                {
                    wirePath.PlaceOnBoard(board, wireType);

                    if (wireType == MoveDirs.EnemyWire)
                    {
                        wirePath.RemoveCornersFromBoard(board);
                    }
                }
            }

            ref ScorePath startScore = ref board.GetCellScorePath(relativeStart);
            startScore = new ScorePath(0, 0, MoveDirs.None);

            PriorityQueue<Point, int> toSee = new PriorityQueue<Point, int>();
            toSee.Enqueue(relativeStart, 0);

            MoveDirs[] moves = new MoveDirs[] { MoveDirs.Up, MoveDirs.Down, MoveDirs.Left, MoveDirs.Right };

            while (toSee.Count > 0)
            {
                Point current = toSee.Dequeue();

                if (current == relativeEnd)
                {
                    return board.GetPath(relativeStart, relativeEnd, end, start, false);
                }

                ScorePath currentScorePath = board.GetCellScorePath(current);
                MoveDirs allowedMoves = board.GetCellMoves(current);

                if (allowedMoves.HasFlag(MoveDirs.FriendWire))
                {
                    return board.GetPath(relativeStart, current, end, start, true);
                }

                bool onEnemyWire = allowedMoves.HasFlag(MoveDirs.EnemyWire);
                bool onWireCorner = allowedMoves.HasFlag(MoveDirs.WireCorner);
                foreach (var move in moves)
                {
                    //Penalty for turning while on another wire
                    bool isTurningOnEnemyWire = onEnemyWire && currentScorePath.DirFrom != MoveDirs.None && move != currentScorePath.DirFrom.Reverse();
                    if (allowedMoves.HasFlag(move))
                    {
                        ScorePath neighborScoreFromCurrent = currentScorePath.Move(move, onEnemyWire, onWireCorner, isTurningOnEnemyWire);

                        Point neighborPos = move.MovePoint(current);
                        ref ScorePath neighborScore = ref board.GetCellScorePath(neighborPos);

                        if (neighborScoreFromCurrent.IsBetterScoreThan(neighborScore))
                        {
                            toSee.Enqueue(neighborPos, neighborScoreFromCurrent.GetTotalScore());
                            neighborScore = neighborScoreFromCurrent;
                        }
                    }
                }
            }

            Debug.WriteLine(board.BoardAllowedMovesToString(relativeStart, relativeEnd));
            Debug.WriteLine(board.BoardStateToString(relativeStart, relativeEnd));
            throw new Exception("Failed to find a path.");
        }
    }
}
