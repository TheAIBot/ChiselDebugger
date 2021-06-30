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
                        LineInfo line = new LineInfo(repath.StartIO, repath.EndIO);
                        if (repathCounter[line.GetLine()]++ >= 20)
                        {
                            continue;
                        }

                        paths.Remove(repath);

                        linesPriority.Enqueue(line, line.GetScore());
                    }
                    paths.Add(path);
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
                Rectangle startRectRelative = board.GetRelativeBoard(startRect.Value).ResizeCentered(1);
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

                ScorePath currentScorePath = board.GetCellScorePath(current);
                MoveDirs allowedMoves = board.GetCellMoves(current);

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
                        ref ScorePath neighborScore = ref board.GetCellScorePath(neighborPos);

                        if (neighborScore.DirFrom != MoveDirs.None)
                        {
                            continue;
                        }

                        //Penalty for turning while on another wire
                        bool isTurningOnEnemyWire = onEnemyWire && currentScorePath.DirFrom != MoveDirs.None && moves[i].Dir != currentScorePath.DirFrom.Reverse();

                        ScorePath neighborScoreFromCurrent = currentScorePath.Move(moves[i].RevDir, onEnemyWire, onFriendWire, onWireCorner, isTurningOnEnemyWire);
                        if (neighborScoreFromCurrent.IsBetterScoreThan(neighborScore))
                        {
                            Point diff = (current - relativeStart).Abs();
                            int dist = diff.X + diff.Y;

                            toSee.Enqueue(board.CellIndex(neighborPos), neighborScoreFromCurrent.GetTotalScore() + dist);
                            neighborScore = neighborScoreFromCurrent;
                        }
                    }
                }
            }

            Debug.WriteLine(board.BoardAllowedMovesToString(relativeEnd, relativeStart));
            Debug.WriteLine(board.BoardStateToString(relativeEnd, relativeStart));
            throw new Exception("Failed to find a path.");
        }

        public bool IsReadyToRoute()
        {
            return MissingNodeIO.Count == 0;
        }
    }
}
