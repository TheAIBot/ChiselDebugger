using ChiselDebug.FIRRTL;
using PriorityQueue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        public List<List<Point>> PathLines(PlacementInfo placements)
        {
            try
            {
                List<Line> lines = GetAllConnectionLines(placements);
                lines.Sort((x, y) => y.GetManhattanDistance() - x.GetManhattanDistance());

                RouterBoard board = new RouterBoard(placements.SpaceNeeded);
                board.PrepareBoard(placements.UsedSpace);
                board.CreateCheckpoint();

                Dictionary<Point, List<List<Point>>> startPosPaths = new Dictionary<Point, List<List<Point>>>();
                List<List<Point>> paths = new List<List<Point>>();
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


                    List<Point> path = PathLine(board, lines[i], outDir, endRect, isWithin, startPosPaths);
                    paths.Add(path);

                    if (startPosPaths.TryGetValue(line.Start, out List<List<Point>> startPaths))
                    {
                        startPaths.Add(path);
                    }
                    else
                    {
                        List<List<Point>> startPdwaaths = new List<List<Point>>();
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

        private List<Point> PathLine(RouterBoard board, Line line, MoveDirs outDir, Rectangle endRect, bool endsWithin, Dictionary<Point, List<List<Point>>> allPaths)
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
                    Point wireStart = board.GetRelativeBoardPos(wirePath[0]);
                    for (int i = 1; i < wirePath.Count; i++)
                    {
                        Point wireEnd = board.GetRelativeBoardPos(wirePath[i]);
                        int wireStartX = Math.Min(wireStart.X, wireEnd.X);
                        int wireStartY = Math.Min(wireStart.Y, wireEnd.Y);

                        int wireEndX = Math.Max(wireStart.X, wireEnd.X);
                        int wireEndY = Math.Max(wireStart.Y, wireEnd.Y);
                        for (int y = wireStartY; y <= wireEndY; y++)
                        {
                            for (int x = wireStartX; x <= wireEndX; x++)
                            {
                                board.AddCellAllowedMoves(new Point(x, y), wireType);
                            }
                        }

                        wireStart = wireEnd;
                    }

                    if (wireType == MoveDirs.EnemyWire)
                    {
                        foreach (var pathPos in wirePath)
                        {
                            Point pathPosRel = board.GetRelativeBoardPos(pathPos);
                            board.RemoveAllIncommingMoves(pathPosRel);
                        }
                    }
                    else
                    {
                        //??
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


        private readonly struct ScorePath
        {
            public readonly ushort TraveledDist;
            public readonly byte TurnsTaken;
            public readonly MoveDirs DirFrom;

            public ScorePath(int travled, int turns, MoveDirs fromDir)
            {
                this.TraveledDist = (ushort)travled;
                this.TurnsTaken = (byte)turns;
                this.DirFrom = fromDir;
            }

            public ScorePath Move(MoveDirs dir, bool toEnemyWire)
            {
                //If the direction we are moving in is not the
                //opposite of the direction we came from,
                //then we must be turning unless we started
                //the path from this point. If we didn't come
                //from any direction then we must've started
                //the path from here.
                bool isTurning = DirFrom != dir.Reverse() && DirFrom != MoveDirs.None;
                int turns = TurnsTaken;
                turns += isTurning ? 1 : 0;
                turns += toEnemyWire ? 1 : 0;

                return new ScorePath(TraveledDist + 1, turns, dir.Reverse());
            }

            public bool IsBetterScoreThan(ScorePath score)
            {
                if (TraveledDist < score.TraveledDist)
                {
                    return true;
                }

                return TraveledDist == score.TraveledDist &&
                       TurnsTaken < score.TurnsTaken;
            }

            public int GetTotalScore()
            {
                return TraveledDist + TurnsTaken;
            }

            public static ScorePath NotReachedYet()
            {
                return new ScorePath(ushort.MaxValue, byte.MaxValue, MoveDirs.None);
            }
        }



        private class RouterBoard
        {
            private readonly Point TopLeft;
            private readonly Point BottomRight;
            private readonly int CellsWide;
            private readonly int CellsHigh;
            private readonly MoveDirs[] CellAllowedDirs;
            private readonly MoveDirs[] CheckpointAllowedDirs;
            private readonly ScorePath[] CellScoreAndPath;

            private const int CellSize = 10;

            public RouterBoard(Point neededBoardSize)
            {
                Point extra = new Point(20, 20);

                this.TopLeft = -extra;
                this.BottomRight = neededBoardSize + extra;

                Point boardSize = BottomRight - TopLeft;
                this.CellsWide = CeilDiv(boardSize.X, CellSize) + 1;
                this.CellsHigh = CeilDiv(boardSize.Y, CellSize) + 1;

                this.CellAllowedDirs = new MoveDirs[CellsWide * CellsHigh];
                this.CheckpointAllowedDirs = new MoveDirs[CellAllowedDirs.Length];
                this.CellScoreAndPath = new ScorePath[CellAllowedDirs.Length];
            }

            private static int CeilDiv(int x, int y)
            {
                return (x + (y - 1)) / y;
            }

            internal int CellIndex(int x, int y)
            {
                return y * CellsWide + x;
            }

            internal int CellIndex(Point pos)
            {
                return CellIndex(pos.X, pos.Y);
            }

            internal MoveDirs GetCellMoves(Point pos)
            {
                return CellAllowedDirs[CellIndex(pos)];
            }

            internal ref ScorePath GetCellScorePath(Point pos)
            {
                return ref CellScoreAndPath[CellIndex(pos)];
            }

            internal Point GetRelativeBoardPos(Point pos)
            {
                return new Point((pos.X - TopLeft.X) / CellSize, (pos.Y - TopLeft.Y) / CellSize);
            }

            internal void PrepareBoard(List<Rectangle> usedSpace)
            {
                //Start with all moves are legal
                MoveDirs allDirs = MoveDirs.All;
                Array.Fill(CellAllowedDirs, allDirs);

                //Moves that go outside the board are not allowed
                for (int x = 0; x < CellsWide; x++)
                {
                    CellAllowedDirs[CellIndex(x, 0)] &= MoveDirs.None;
                    CellAllowedDirs[CellIndex(x, CellsHigh - 1)] &= MoveDirs.None;
                }
                for (int y = 0; y < CellsHigh; y++)
                {
                    CellAllowedDirs[CellIndex(0, y)] &= MoveDirs.None;
                    CellAllowedDirs[CellIndex(CellsWide - 1, y)] &= MoveDirs.None;
                }

                //for (int x = 0; x < CellsWide; x++)
                //{
                //    CellAllowedDirs[CellIndex(x, 0)] &= MoveDirs.ExceptUp;
                //    CellAllowedDirs[CellIndex(x, CellsHigh - 1)] &= MoveDirs.ExceptDown;
                //}
                //for (int y = 0; y < CellsHigh; y++)
                //{
                //    CellAllowedDirs[CellIndex(0, y)] &= MoveDirs.ExceptLeft;
                //    CellAllowedDirs[CellIndex(CellsWide - 1, y)] &= MoveDirs.ExceptRight;
                //}

                //Moves that go into used space are not allowed.
                //Moves inside used space are not removed because
                //they shouldn't be reachable.
                for (int i = 0; i < usedSpace.Count; i++)
                {
                    Rectangle space = usedSpace[i];
                    Point spaceTopLeft = space.Pos - TopLeft;
                    Point spaceBottomRight = space.Pos + space.Size - TopLeft;

                    //Need to remove moves not at the edge of the used space
                    //but at the outer neighbors to the uses space. That's
                    //why is subtracts 1 for the start coords and adds 1
                    //to the end coords. Min/Max is used to keep the coords
                    //inside the board after the subtraction/addition.
                    int startX = Math.Max(0, (spaceTopLeft.X / CellSize) - 1);
                    int startY = Math.Max(0, (spaceTopLeft.Y / CellSize) - 1);
                    
                    int endX = Math.Min(CellsWide - 1, CeilDiv(spaceBottomRight.X, CellSize) + 1);
                    int endY = Math.Min(CellsHigh - 1, CeilDiv(spaceBottomRight.Y, CellSize) + 1);

                    for (int x = startX + 1; x < endX; x++)
                    {
                        CellAllowedDirs[CellIndex(x, startY)] &= MoveDirs.ExceptDown;
                        CellAllowedDirs[CellIndex(x, endY)] &= MoveDirs.ExceptUp;
                    }
                    for (int y = startY + 1; y < endY; y++)
                    {
                        CellAllowedDirs[CellIndex(startX, y)] &= MoveDirs.ExceptRight;
                        CellAllowedDirs[CellIndex(endX, y)] &= MoveDirs.ExceptLeft;
                    }
                }
            }

            internal Rectangle GetRelativeBoard(Rectangle rect)
            {
                Point spaceTopLeft = rect.Pos - TopLeft;
                Point spaceBottomRight = rect.Pos + rect.Size - TopLeft;
                return new Rectangle(
                    new Point(
                        spaceTopLeft.X / CellSize, 
                        spaceTopLeft.Y / CellSize), 
                    new Point(
                        CeilDiv(spaceBottomRight.X, CellSize), 
                        CeilDiv(spaceBottomRight.Y, CellSize)));
            }

            internal void CreateCheckpoint()
            {
                Array.Copy(CellAllowedDirs, CheckpointAllowedDirs, CellAllowedDirs.Length);
            }

            internal void ReloadCheckpoint()
            {
                Array.Copy(CheckpointAllowedDirs, CellAllowedDirs, CellAllowedDirs.Length);
            }

            internal void PrepareForSearchFromCheckpoint()
            {
                ReloadCheckpoint();
                Array.Fill(CellScoreAndPath, ScorePath.NotReachedYet());
            }

            internal void SetAllOutgoingMoves(Point pos)
            {
                int cellPosX = pos.X;
                int cellPosY = pos.Y;

                //Not allowed to make move that goes
                //outside the bounds of the board
                MoveDirs allowedMoved = MoveDirs.None;
                if (cellPosX - 1 >= 0)
                {
                    allowedMoved |= MoveDirs.Left;
                }
                if (cellPosX + 1 < CellsWide)
                {
                    allowedMoved |= MoveDirs.Right;
                }
                if (cellPosY - 1 >= 0)
                {
                    allowedMoved |= MoveDirs.Up;
                }
                if (cellPosY + 1 < CellsHigh)
                {
                    allowedMoved |= MoveDirs.Down;
                }

                CellAllowedDirs[CellIndex(cellPosX, cellPosY)] = allowedMoved;
            }

            internal void SetCellAllowedMoves(Point pos, MoveDirs moves)
            {
                CellAllowedDirs[CellIndex(pos)] = moves;
            }

            internal void AddCellAllowedMoves(Point pos, MoveDirs moves)
            {
                CellAllowedDirs[CellIndex(pos)] |= moves;
            }

            internal void SetAllIncommingMoves(Point pos)
            {
                int cellPosX = pos.X;
                int cellPosY = pos.Y;

                if (cellPosX - 1 >= 0)
                {
                    CellAllowedDirs[CellIndex(cellPosX - 1, cellPosY)] |= MoveDirs.Right;
                }
                if (cellPosX + 1 < CellsWide)
                {
                    CellAllowedDirs[CellIndex(cellPosX + 1, cellPosY)] |= MoveDirs.Left;
                }
                if (cellPosY - 1 >= 0)
                {
                    CellAllowedDirs[CellIndex(cellPosX, cellPosY - 1)] |= MoveDirs.Down;
                }
                if (cellPosY + 1 < CellsHigh)
                {
                    CellAllowedDirs[CellIndex(cellPosX, cellPosY + 1)] |= MoveDirs.Up;
                }
            }

            internal void RemoveAllIncommingMoves(Point pos)
            {
                int cellPosX = pos.X;
                int cellPosY = pos.Y;

                if (cellPosX - 1 >= 0)
                {
                    CellAllowedDirs[CellIndex(cellPosX - 1, cellPosY)] &= MoveDirs.ExceptRight;
                }
                if (cellPosX + 1 < CellsWide)
                {
                    CellAllowedDirs[CellIndex(cellPosX + 1, cellPosY)] &= MoveDirs.ExceptLeft;
                }
                if (cellPosY - 1 >= 0)
                {
                    CellAllowedDirs[CellIndex(cellPosX, cellPosY - 1)] &= MoveDirs.ExceptDown;
                }
                if (cellPosY + 1 < CellsHigh)
                {
                    CellAllowedDirs[CellIndex(cellPosX, cellPosY + 1)] &= MoveDirs.ExceptUp;
                }
            }

            internal List<Point> GetPath(Point start, Point end, Point actualStart, Point actualEnd, bool pathToWire)
            {
                List<Point> pathAsTurns = new List<Point>();

                Point zero = new Point(0, 0);
                MoveDirs prevDir = MoveDirs.None;
                Point boardPos = end;
                Point actualPos = end * CellSize + TopLeft;
                while (boardPos != start)
                {
                    ScorePath path = GetCellScorePath(boardPos);
                    if (path.DirFrom != prevDir)
                    {
                        pathAsTurns.Add(actualPos);
                    }
                    prevDir = path.DirFrom;
                    boardPos = path.DirFrom.MovePoint(boardPos);
                    actualPos = actualPos + path.DirFrom.MovePoint(zero) * CellSize;
                }

                pathAsTurns.Add(actualPos);
                pathAsTurns.Reverse();

                if (pathAsTurns.Count > 0)
                {
                    if (pathAsTurns[0].Y == pathAsTurns[1].Y)
                    {
                        pathAsTurns[0] = new Point(pathAsTurns[0].X, actualStart.Y);
                        pathAsTurns[1] = new Point(pathAsTurns[1].X, actualStart.Y);
                    }
                    else
                    {
                        pathAsTurns[0] = new Point(actualStart.X, pathAsTurns[0].Y);
                        pathAsTurns[1] = new Point(actualStart.X, pathAsTurns[1].Y);
                    }
                }

                if (!pathToWire)
                {
                    if (pathAsTurns.Count > 0)
                    {
                        if (pathAsTurns[^1].Y == pathAsTurns[^2].Y)
                        {
                            pathAsTurns[^1] = new Point(pathAsTurns[^1].X, actualEnd.Y);
                            pathAsTurns[^2] = new Point(pathAsTurns[^2].X, actualEnd.Y);
                        }
                        else
                        {
                            pathAsTurns[^1] = new Point(actualEnd.X, pathAsTurns[^1].Y);
                            pathAsTurns[^2] = new Point(actualEnd.X, pathAsTurns[^2].Y);
                        }
                    }
                }

                return pathAsTurns;
            }

            internal string BoardStateToString(Point start, Point end)
            {
                StringBuilder sBuilder = new StringBuilder();
                for (int y = 0; y < CellsHigh; y++)
                {
                    for (int x = 0; x < CellsWide; x++)
                    {
                        Point pos = new Point(x, y);
                        ScorePath scorePath = GetCellScorePath(pos);
                        MoveDirs allowedDirs = GetCellMoves(pos);
                        if (pos == start)
                        {
                            sBuilder.Append('S');
                        }
                        else if (pos == end)
                        {
                            sBuilder.Append('E');
                        }
                        else if (allowedDirs.HasFlag(MoveDirs.FriendWire))
                        {
                            sBuilder.Append('%');
                        }
                        else if (scorePath.DirFrom == MoveDirs.Up)
                        {
                            sBuilder.Append("↑");
                        }
                        else if (scorePath.DirFrom == MoveDirs.Down)
                        {
                            sBuilder.Append("↓");
                        }
                        else if (scorePath.DirFrom == MoveDirs.Left)
                        {
                            sBuilder.Append("←");
                        }
                        else if (scorePath.DirFrom == MoveDirs.Right)
                        {
                            sBuilder.Append("→");
                        }
                        else if (scorePath.DirFrom == MoveDirs.None)
                        {
                            sBuilder.Append(' ');
                        }
                    }
                    sBuilder.AppendLine();
                }

                return sBuilder.ToString();
            }
        }
    }

    [Flags]
    internal enum MoveDirs : byte
    {
        None = 0b0000,
        Up = 0b0001,
        Down = 0b0010,
        Left = 0b0100,
        Right = 0b1000,
        All = 0b1111,
        ExceptUp = unchecked((byte)~Up),
        ExceptDown = unchecked((byte)~Down),
        ExceptLeft = unchecked((byte)~Left),
        ExceptRight = unchecked((byte)~Right),
        FriendWire = 0b0001_0000,
        EnemyWire  = 0b0010_0000
    }

    internal static class MoveExtensions
    {
        internal static Point MovePoint(this MoveDirs dir, Point pos)
        {
            return dir switch
            {
                MoveDirs.Up => new Point(pos.X, pos.Y - 1),
                MoveDirs.Down => new Point(pos.X, pos.Y + 1),
                MoveDirs.Left => new Point(pos.X - 1, pos.Y),
                MoveDirs.Right => new Point(pos.X + 1, pos.Y),
                _ => throw new Exception($"Can't move point in the direction: {dir}")
            };
        }

        internal static MoveDirs Reverse(this MoveDirs dir)
        {
            return dir switch
            {
                MoveDirs.Up => MoveDirs.Down,
                MoveDirs.Down => MoveDirs.Up,
                MoveDirs.Left => MoveDirs.Right,
                MoveDirs.Right => MoveDirs.Left,
                _ => throw new Exception($"Can't reverse this direction: {dir}")
            };
        }
    }
}
