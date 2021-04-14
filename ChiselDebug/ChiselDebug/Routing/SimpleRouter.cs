using ChiselDebug.GraphFIR;
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

        public List<WirePath> PathLines(PlacementInfo placements)
        {
            try
            {
                int rerouteCount = 0;
                Dictionary<Line, int> repathCounter = new Dictionary<Line, int>();
                PriorityQueue<LineInfo, int> linesPriority = new PriorityQueue<LineInfo, int>();
                foreach ((IOInfo start, IOInfo end) in Connections.GetAllConnectionLines(placements))
                {
                    LineInfo line = new LineInfo(start, end);
                    linesPriority.Enqueue(line, line.GetScore());
                    repathCounter.Add(line.GetLine(), 0);
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
                        if (oldPath.EndIO.DirIO.Position == path.EndIO.DirIO.Position ||
                            oldPath.StartIO.DirIO.Position == path.StartIO.DirIO.Position)
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
                        LineInfo line = new LineInfo(repath.EndIO, repath.StartIO);
                        if (repathCounter[line.GetLine()]++ >= 20)
                        {
                            continue;
                        }

                        paths.Remove(repath);
                        startPosPaths[repath.EndIO.DirIO.Position].Remove(repath);

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

        private WirePath PathLine(RouterBoard board, IOInfo start, IOInfo end, Rectangle? startRect, Rectangle? endRect, Dictionary<Point, List<WirePath>> allPaths)
        {
            board.ReloadCheckpoint();

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
                if (keyValue.Key == start.DirIO.Position || keyValue.Key == end.DirIO.Position)
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

            PriorityQueue<Point> toSee = new PriorityQueue<Point>();
            toSee.Enqueue(relativeStart, 0);

            MoveData[] moves = new MoveData[] 
            { 
                new MoveData(MoveDirs.Up),
                new MoveData(MoveDirs.Down),
                new MoveData(MoveDirs.Left),
                new MoveData(MoveDirs.Right)
            };

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
                for (int i = 0; i < moves.Length; i++)
                {
                    if (allowedMoves.HasFlag(moves[i].Dir))
                    {
                        Point neighborPos = current + moves[i].DirVec;
                        ref ScorePath neighborScore = ref board.GetCellScorePath(neighborPos);

                        if (neighborScore.DirFrom != MoveDirs.None)
                        {
                            continue;
                        }

                        //Penalty for turning while on another wire
                        bool isTurningOnEnemyWire = onEnemyWire && currentScorePath.DirFrom != MoveDirs.None && moves[i].Dir != currentScorePath.DirFrom.Reverse();

                        ScorePath neighborScoreFromCurrent = currentScorePath.Move(moves[i].RevDir, onEnemyWire, onWireCorner, isTurningOnEnemyWire);
                        if (neighborScoreFromCurrent.IsBetterScoreThan(neighborScore))
                        {
                            Point diff = (current - relativeEnd).Abs();
                            int dist = 0;// diff.X + diff.Y;

                            toSee.Enqueue(neighborPos, neighborScoreFromCurrent.GetTotalScore() + dist / 2);
                            neighborScore = neighborScoreFromCurrent;
                        }
                    }
                }
            }

            Debug.WriteLine(board.BoardAllowedMovesToString(relativeStart, relativeEnd));
            Debug.WriteLine(board.BoardStateToString(relativeStart, relativeEnd));
            throw new Exception("Failed to find a path.");
        }

        public bool IsReadyToRoute()
        {
            return MissingNodeIO.Count == 0;
        }
    }
}
