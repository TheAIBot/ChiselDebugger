using ChiselDebug.FIRRTL;
using ChiselDebug.Routing;
using PriorityQueue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ChiselDebug
{
    public class ConnectionsHandler
    {
        private readonly struct IOInfo
        {
            internal readonly Point Position;
            internal readonly FIRRTLNode Node;

            public IOInfo(Point pos, FIRRTLNode node)
            {
                this.Position = pos;
                this.Node = node;
            }
        }

        private readonly Module Mod;
        private readonly List<Connection> UsedModuleConnections;
        private readonly Dictionary<Input, IOInfo> InputOffsets = new Dictionary<Input, IOInfo>();
        private readonly Dictionary<Output, IOInfo> OutputOffsets = new Dictionary<Output, IOInfo>();

        public ConnectionsHandler(Module mod)
        {
            this.Mod = mod;
            this.UsedModuleConnections = Mod.GetAllModuleConnections();
        }

        public void UpdateIOFromNode(FIRRTLNode node, List<Positioned<Input>> inputOffsets, List<Positioned<Output>> outputOffsets)
        {
            foreach (var inputPos in inputOffsets)
            {
                InputOffsets[inputPos.Value] = new IOInfo(inputPos.Position, node);
            }
            foreach (var outputPos in outputOffsets)
            {
                OutputOffsets[outputPos.Value] = new IOInfo(outputPos.Position, node);
            }
        }

        private List<Line> GetAllConnectionLines(PlacementInfo placements)
        {
            Dictionary<FIRRTLNode, Point> nodePoses = new Dictionary<FIRRTLNode, Point>();
            foreach (var nodePos in placements.NodePositions)
            {
                nodePoses.Add(nodePos.Value, nodePos.Position);
            }
            nodePoses.Add(Mod, new Point(0, 0));

            List<Line> lines = new List<Line>();
            foreach (var connection in UsedModuleConnections)
            {
                if (OutputOffsets.TryGetValue(connection.From, out IOInfo outputInfo))
                {
                    foreach (var input in connection.To)
                    {
                        if (InputOffsets.TryGetValue(input, out IOInfo inputInfo))
                        {
                            Point startOffset = nodePoses[outputInfo.Node];
                            Point endOffset = nodePoses[inputInfo.Node];

                            Point start = startOffset + outputInfo.Position;
                            Point end = endOffset + inputInfo.Position;
                            lines.Add(new Line(start, end));
                        }
                    }
                }
            }

            return lines;
        }

        public List<WirePath> PathLines(PlacementInfo placements)
        {
            try
            {
                List<Line> lines = GetAllConnectionLines(placements);
                lines.Sort((x, y) => y.GetManhattanDistance() - x.GetManhattanDistance());

                RouterBoard board = new RouterBoard(placements.SpaceNeeded);
                board.PrepareBoard(placements.UsedSpace);
                board.CreateCheckpoint();

                Dictionary<Point, List<WirePath>> startPosPaths = new Dictionary<Point, List<WirePath>>();
                List<WirePath> paths = new List<WirePath>();
                for (int i = 0; i < lines.Count; i++)
                {
                    Line line = lines[i];
                    MoveDirs outDir = MoveDirs.None;
                    Rectangle endRect = new Rectangle();
                    bool isWithin = false;
                    foreach (var usedSpace in placements.UsedSpace)
                    {
                        if (usedSpace.WithinButExcludeRectEdge(line.End))
                        {
                            outDir = MoveDirs.Up;
                            endRect = usedSpace;
                            isWithin = true;
                            break;
                        }
                        if (line.End.X == usedSpace.LeftX)
                        {
                            outDir = MoveDirs.Left;
                            break;
                        }
                        else if (line.End.X == usedSpace.RightX)
                        {
                            outDir = MoveDirs.Right;
                            break;
                        }
                        else if (line.End.Y == usedSpace.TopY)
                        {
                            outDir = MoveDirs.Up;
                            break;
                        }
                        else if (line.End.Y == usedSpace.BottomY)
                        {
                            outDir = MoveDirs.Down;
                            break;
                        }
                    }
                    if (outDir == MoveDirs.None && line.Start.X < 0)
                    {
                        outDir = MoveDirs.Right;
                    }
                    else if (outDir == MoveDirs.None)
                    {
                        outDir = MoveDirs.Left;
                    }


                    WirePath path = PathLine(board, lines[i], outDir, endRect, isWithin, startPosPaths);
                    paths.Add(path);

                    if (startPosPaths.TryGetValue(line.Start, out List<WirePath> startPaths))
                    {
                        startPaths.Add(path);
                    }
                    else
                    {
                        List<WirePath> startPdwaaths = new List<WirePath>();
                        startPdwaaths.Add(path);
                        startPosPaths.Add(line.Start, startPdwaaths);
                    }

                    //for (int z = 1; z < path.Count - 1; z++)
                    //{
                    //    board.SetCheckpointCellMoves(path[z], Move)
                    //}
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

        private WirePath PathLine(RouterBoard board, Line line, MoveDirs outDir, Rectangle endRect, bool endsWithin, Dictionary<Point, List<WirePath>> allPaths)
        {
            board.PrepareForSearchFromCheckpoint();

            Point relativeStart = board.GetRelativeBoardPos(line.End);
            Point relativeEnd = board.GetRelativeBoardPos(line.Start);

            if (endsWithin)
            {
                Rectangle endRectRelative = board.GetRelativeBoard(endRect);
                Point endGo = relativeStart;
                do
                {
                    board.SetCellAllowedMoves(endGo, MoveDirs.Up);
                    endGo = MoveDirs.Up.MovePoint(endGo);
                } while (endRectRelative.Within(endGo));
            }



            //board.SetAllOutgoingMoves(relativeStart);
            board.SetCellAllowedMoves(relativeStart, outDir);
            board.SetAllIncommingMoves(relativeEnd);

            foreach (var keyValue in allPaths)
            {
                MoveDirs wireType;
                if (keyValue.Key == line.Start)
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
                        wirePath.PlaceCornersOnBoard(board);
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
                    return board.GetPath(relativeStart, relativeEnd, line.End, line.Start, false);
                }

                ScorePath currentScorePath = board.GetCellScorePath(current);
                MoveDirs allowedMoves = board.GetCellMoves(current);

                if (allowedMoves.HasFlag(MoveDirs.FriendWire))
                {
                    //Debug.WriteLine(board.BoardStateToString(relativeStart, relativeEnd));
                    return board.GetPath(relativeStart, current, line.End, line.Start, true);
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
