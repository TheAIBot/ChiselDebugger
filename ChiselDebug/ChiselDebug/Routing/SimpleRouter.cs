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

        public List<WirePath> PathLines(PlacementInfo placements)
        {
            try
            {
                List<(IOInfo start, IOInfo end)> lines = Connections.GetAllConnectionLines(placements);
                lines.Sort((x, y) => new Line(y.start.DirIO.Position, y.end.DirIO.Position).GetManhattanDistance() - new Line(x.start.DirIO.Position, x.end.DirIO.Position).GetManhattanDistance());

                RouterBoard board = new RouterBoard(placements.SpaceNeeded);
                board.PrepareBoard(placements.UsedSpace.Values.ToList());
                board.CreateCheckpoint();

                Dictionary<Point, List<WirePath>> startPosPaths = new Dictionary<Point, List<WirePath>>();
                List<WirePath> paths = new List<WirePath>();
                for (int i = 0; i < lines.Count; i++)
                {
                    (IOInfo start, IOInfo end) = lines[i];

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

                return paths;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Debug.WriteLine(e.StackTrace);
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
            board.SetCellAllowedMoves(relativeStart, end.DirIO.InitialDir.Reverse());

            //Only allow connection from the correct direction
            board.RemoveAllIncommingMoves(relativeEnd);
            board.AddCellAllowedMoves(relativeEnd + start.DirIO.InitialDir.MovePoint(new Point(0, 0)), start.DirIO.InitialDir.Reverse());

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

            Debug.WriteLine(board.BoardAllowedMovesToString());

            while (toSee.Count > 0)
            {
                Point current = toSee.Dequeue();

                if (current == relativeEnd)
                {
                    Debug.WriteLine(board.BoardStateToString(relativeStart, relativeEnd));
                    return board.GetPath(relativeStart, relativeEnd, end, start, false);
                }

                ScorePath currentScorePath = board.GetCellScorePath(current);
                MoveDirs allowedMoves = board.GetCellMoves(current);

                if (allowedMoves.HasFlag(MoveDirs.FriendWire))
                {
                    Debug.WriteLine(board.BoardStateToString(relativeStart, relativeEnd));
                    return board.GetPath(relativeStart, current, end, start, true);
                }

                bool onEnemyWire = allowedMoves.HasFlag(MoveDirs.EnemyWire);
                foreach (var move in moves)
                {
                    if (onEnemyWire && currentScorePath.DirFrom != MoveDirs.None && move != currentScorePath.DirFrom.Reverse())
                    {
                        continue;
                    }
                    if (allowedMoves.HasFlag(move))
                    {
                        ScorePath neighborScoreFromCurrent = currentScorePath.Move(move, onEnemyWire);

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

            Debug.WriteLine(board.BoardStateToString(relativeStart, relativeEnd));
            throw new Exception("Failed to find a path.");
        }
    }
}
