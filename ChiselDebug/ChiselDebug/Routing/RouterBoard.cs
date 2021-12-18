using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChiselDebug.Routing
{
    internal class SubBoard
    {
        internal Point BoardPos;
        private readonly MoveDirs[] CellAllowedDirs;
        private readonly ScorePath[] CellScoreAndPath;
        private readonly MoveDirs[] CheckpointAllowedDirs;
        private readonly ScorePath[] CheckpointScoreAndPath;
        internal bool HasCheckpoint => CheckpointAllowedDirs != null;

        internal SubBoard(Point boardPos, bool needsCheckpoint)
        {
            BoardPos = boardPos;
            CellAllowedDirs = GC.AllocateArray<MoveDirs>(RouterBoard.SubBoardSize, true);
            CellScoreAndPath = GC.AllocateArray<ScorePath>(RouterBoard.SubBoardSize, true);

            if (needsCheckpoint)
            {
                CheckpointAllowedDirs = new MoveDirs[RouterBoard.SubBoardSize];
                CheckpointScoreAndPath = new ScorePath[RouterBoard.SubBoardSize];
            }


            ClearBoard();
        }

        internal ref MoveDirs GetCellMoves(int index)
        {
            return ref CellAllowedDirs[index];
        }

        internal ref ScorePath GetCellScorePath(int index)
        {
            return ref CellScoreAndPath[index];
        }

        internal CellData GetCellData(int index)
        {
            return new CellData(ref CellAllowedDirs[index], ref CellScoreAndPath[index]);
        }

        internal void CreateCheckpoint()
        {
            Array.Copy(CellAllowedDirs, CheckpointAllowedDirs, CellAllowedDirs.Length);
            Array.Copy(CellScoreAndPath, CheckpointScoreAndPath, CellScoreAndPath.Length);
        }

        internal void ReloadCheckpoint()
        {
            Array.Copy(CheckpointAllowedDirs, CellAllowedDirs, CellAllowedDirs.Length);
            Array.Copy(CheckpointScoreAndPath, CellScoreAndPath, CellScoreAndPath.Length);
        }

        internal void ClearBoard()
        {
            Array.Fill(CellAllowedDirs, MoveDirs.All);
            Array.Fill(CellScoreAndPath, ScorePath.NotReachedYet());
        }
    }

    public readonly unsafe ref struct Ref<T> where T : unmanaged
    {
        private readonly T* ValuePointer;
        public ref T Value => ref Unsafe.AsRef<T>(ValuePointer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Ref(ref T value)
        {
            ValuePointer = (T*)Unsafe.AsPointer(ref value);
        }
    }

    internal readonly ref struct CellData
    {
        public readonly Ref<MoveDirs> CellMoves;
        public readonly Ref<ScorePath> CellScoreAndPath;

        internal CellData(ref MoveDirs cellMoves, ref ScorePath scopePath)
        {
            CellMoves = new Ref<MoveDirs>(ref cellMoves);
            CellScoreAndPath = new Ref<ScorePath>(ref scopePath);
        }
    }

    internal class RouterBoard
    {
        public readonly int CellsWide;
        public readonly int CellsHigh;
        private readonly int SubBoardsWide;
        private readonly int SubBoardsHigh;
        private readonly SubBoard[] SubBoards;
        internal const int SubBoardSideLength = 8;
        internal const int SubBoardSize = SubBoardSideLength * SubBoardSideLength;
        private readonly Queue<SubBoard> UnusedBoards = new Queue<SubBoard>();
        private SubBoard PrevBoard = null;

        private const int CellSize = 10;
        public const int MinSeparation = 4;
        private const int RectPadding = 2;

        public RouterBoard(Point neededBoardSize)
        {
            CellsWide = CeilDiv(neededBoardSize.X, CellSize) + 1;
            CellsHigh = CeilDiv(neededBoardSize.Y, CellSize) + 1;
            SubBoardsWide = CeilDiv(CellsWide, SubBoardSideLength);
            SubBoardsHigh = CeilDiv(CellsHigh, SubBoardSideLength);
            SubBoards = new SubBoard[SubBoardsWide * SubBoardsHigh];
        }

        private static int CeilDiv(int x, int y)
        {
            return (x + (y - 1)) / y;
        }

        internal int CellIndex(Point pos)
        {
            return CellIndex(pos.X, pos.Y);
        }

        internal int CellIndex(int x, int y)
        {
            return CellIndex(x, y, CellsWide);
        }

        private static int CellIndex(int x, int y, int width)
        {
            return y * width + x;
        }

        public Point CellFromIndex(int index)
        {
            return CellFromIndex(index, CellsWide);
        }

        private static Point CellFromIndex(int index, int div)
        {
            int x;
            int y = Math.DivRem(index, div, out x);
            return new Point(x, y);
        }

        internal ref MoveDirs GetCellMoves(int cellIndex)
        {
            return ref GetCellMoves(CellFromIndex(cellIndex));
        }

        internal ref MoveDirs GetCellMoves(int x, int y)
        {
            return ref GetCellMoves(new Point(x, y));
        }

        internal ref MoveDirs GetCellMoves(Point pos)
        {
            return ref GetCellMoves(pos, false);
        }

        private ref MoveDirs GetCellMoves(int x, int y, bool needsCheckpoint)
        {
            return ref GetCellMoves(new Point(x, y), needsCheckpoint);
        }

        private ref MoveDirs GetCellMoves(Point pos, bool needsCheckpoint)
        {
            int boardPosX = (int)((uint)pos.X / SubBoardSideLength);
            int boardPosY = (int)((uint)pos.Y / SubBoardSideLength);
            int subBoardPosX = (int)((uint)pos.X % SubBoardSideLength);
            int subBoardPosY = (int)((uint)pos.Y % SubBoardSideLength);
            int subCellIndex = CellIndex(subBoardPosX, subBoardPosY, SubBoardSideLength);
            if (PrevBoard != null && PrevBoard.BoardPos == new Point(boardPosX, boardPosY))
            {
                return ref PrevBoard.GetCellMoves(subCellIndex);
            }

            int subBoardIndex = CellIndex(boardPosX, boardPosY, SubBoardsWide);
            ref SubBoard subBoard = ref SubBoards[subBoardIndex];
            if (subBoard == null)
            {
                Point boardPos = new Point(boardPosX, boardPosY);
                if (!needsCheckpoint && UnusedBoards.Count > 0)
                {
                    subBoard = UnusedBoards.Dequeue();
                    subBoard.BoardPos = boardPos;
                }
                else
                {
                    subBoard = new SubBoard(boardPos, needsCheckpoint);
                }
            }

            PrevBoard = subBoard;
            return ref subBoard.GetCellMoves(subCellIndex);
        }

        internal ref ScorePath GetCellScorePath(int cellIndex)
        {
            return ref GetCellScorePath(CellFromIndex(cellIndex));
        }

        internal ref ScorePath GetCellScorePath(Point pos)
        {
            int boardPosX = (int)((uint)pos.X / SubBoardSideLength);
            int boardPosY = (int)((uint)pos.Y / SubBoardSideLength);
            int subBoardPosX = (int)((uint)pos.X % SubBoardSideLength);
            int subBoardPosY = (int)((uint)pos.Y % SubBoardSideLength);
            int subCellIndex = CellIndex(subBoardPosX, subBoardPosY, SubBoardSideLength);
            if (PrevBoard != null && PrevBoard.BoardPos == new Point(boardPosX, boardPosY))
            {
                return ref PrevBoard.GetCellScorePath(subCellIndex);
            }

            int subBoardIndex = CellIndex(boardPosX, boardPosY, SubBoardsWide);
            ref SubBoard subBoard = ref SubBoards[subBoardIndex];
            if (subBoard == null)
            {
                Point boardPos = new Point(boardPosX, boardPosY);
                if (UnusedBoards.Count > 0)
                {
                    subBoard = UnusedBoards.Dequeue();
                    subBoard.BoardPos = boardPos;
                }
                else
                {
                    subBoard = new SubBoard(boardPos, false);
                }
            }

            PrevBoard = subBoard;
            return ref subBoard.GetCellScorePath(subCellIndex);
        }

        internal CellData GetCellData(Point pos)
        {
            return GetCellData(pos, false);
        }

        private CellData GetCellData(Point pos, bool needsCheckpoint)
        {
            int boardPosX = (int)((uint)pos.X / SubBoardSideLength);
            int boardPosY = (int)((uint)pos.Y / SubBoardSideLength);
            int subBoardPosX = (int)((uint)pos.X % SubBoardSideLength);
            int subBoardPosY = (int)((uint)pos.Y % SubBoardSideLength);
            int subCellIndex = CellIndex(subBoardPosX, subBoardPosY, SubBoardSideLength);
            if (PrevBoard != null && PrevBoard.BoardPos == new Point(boardPosX, boardPosY))
            {
                return PrevBoard.GetCellData(subCellIndex);
            }

            int subBoardIndex = CellIndex(boardPosX, boardPosY, SubBoardsWide);
            ref SubBoard subBoard = ref SubBoards[subBoardIndex];
            if (subBoard == null)
            {
                Point boardPos = new Point(boardPosX, boardPosY);
                if (!needsCheckpoint && UnusedBoards.Count > 0)
                {
                    subBoard = UnusedBoards.Dequeue();
                    subBoard.BoardPos = boardPos;
                }
                else
                {
                    subBoard = new SubBoard(boardPos, needsCheckpoint);
                }
            }

            PrevBoard = subBoard;
            return PrevBoard.GetCellData(subCellIndex);
        }

        internal Point GetRelativeBoardPos(Point pos)
        {
            return new Point(pos.X / CellSize, pos.Y / CellSize);
        }

        internal void PrepareBoard(List<Rectangle> usedSpace)
        {
            //Moves that go outside the board are not allowed
            for (int x = 0; x < CellsWide; x++)
            {
                GetCellMoves(x, 0, true) &= MoveDirs.None;
                GetCellMoves(x, CellsHigh - 1, true) &= MoveDirs.None;
            }
            for (int y = 0; y < CellsHigh; y++)
            {
                GetCellMoves(0, y, true) &= MoveDirs.None;
                GetCellMoves(CellsWide - 1, y, true) &= MoveDirs.None;
            }

            //Moves that go into used space are not allowed.
            //Moves inside used space are not removed because
            //they shouldn't be reachable.
            for (int i = 0; i < usedSpace.Count; i++)
            {
                Rectangle spaceRel = GetRelativeBoard(usedSpace[i]);
                for (int x = spaceRel.LeftX + 1; x < spaceRel.RightX; x++)
                {
                    GetCellMoves(x, spaceRel.TopY, true) &= MoveDirs.ExceptDown;
                    GetCellMoves(x, spaceRel.BottomY, true) &= MoveDirs.ExceptUp;
                }
                for (int y = spaceRel.TopY + 1; y < spaceRel.BottomY; y++)
                {
                    GetCellMoves(spaceRel.LeftX, y, true) &= MoveDirs.ExceptRight;
                    GetCellMoves(spaceRel.RightX, y, true) &= MoveDirs.ExceptLeft;
                }
            }
        }

        internal Rectangle GetRelativeBoard(Rectangle rect)
        {
            Point spaceTopLeft = rect.Pos;
            return new Rectangle(
                new Point(
                    (spaceTopLeft.X / CellSize) - RectPadding,
                    (spaceTopLeft.Y / CellSize) - RectPadding),
                new Point(
                    CeilDiv(rect.Size.X, CellSize) + RectPadding * 2,
                    CeilDiv(rect.Size.Y, CellSize) + RectPadding * 2));
        }

        internal void CreateCheckpoint()
        {
            for (int i = 0; i < SubBoards.Length; i++)
            {
                SubBoards[i]?.CreateCheckpoint();
            }
        }

        internal void ReloadCheckpoint()
        {
            PrevBoard = null;
            for (int i = 0; i < SubBoards.Length; i++)
            {
                if (SubBoards[i] != null && !SubBoards[i].HasCheckpoint)
                {
                    SubBoards[i].ClearBoard();
                    UnusedBoards.Enqueue(SubBoards[i]);
                    SubBoards[i] = null;
                    continue;
                }
                SubBoards[i]?.ReloadCheckpoint();
            }
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

            GetCellMoves(cellPosX, cellPosY) = allowedMoved;
        }

        internal void SetCellAllowedMoves(Point pos, MoveDirs moves)
        {
            GetCellMoves(pos) = moves;
        }

        internal void AddCellAllowedMoves(Point pos, MoveDirs moves)
        {
            AddCellAllowedMoves(CellIndex(pos), moves);
        }

        internal void AddCellAllowedMoves(int posIndex, MoveDirs moves)
        {
            GetCellMoves(posIndex) |= moves;
        }

        internal void SetAllIncommingMoves(Point pos)
        {
            int cellPosX = pos.X;
            int cellPosY = pos.Y;

            if (cellPosX - 1 >= 0)
            {
                GetCellMoves(cellPosX - 1, cellPosY) |= MoveDirs.Right;
            }
            if (cellPosX + 1 < CellsWide)
            {
                GetCellMoves(cellPosX + 1, cellPosY) |= MoveDirs.Left;
            }
            if (cellPosY - 1 >= 0)
            {
                GetCellMoves(cellPosX, cellPosY - 1) |= MoveDirs.Down;
            }
            if (cellPosY + 1 < CellsHigh)
            {
                GetCellMoves(cellPosX, cellPosY + 1) |= MoveDirs.Up;
            }
        }

        internal void RemoveAllIncommingMoves(Point pos)
        {
            int cellPosX = pos.X;
            int cellPosY = pos.Y;

            if (cellPosX - 1 >= 0)
            {
                GetCellMoves(cellPosX - 1, cellPosY) &= MoveDirs.ExceptRight;
            }
            if (cellPosX + 1 < CellsWide)
            {
                GetCellMoves(cellPosX + 1, cellPosY) &= MoveDirs.ExceptLeft;
            }
            if (cellPosY - 1 >= 0)
            {
                GetCellMoves(cellPosX, cellPosY - 1) &= MoveDirs.ExceptDown;
            }
            if (cellPosY + 1 < CellsHigh)
            {
                GetCellMoves(cellPosX, cellPosY + 1) &= MoveDirs.ExceptUp;
            }
        }

        internal WirePath GetPath(Point start, Point end, IOInfo startIO, IOInfo endIO, bool pathToWire)
        {
            List<Point> pathAsTurns = new List<Point>();
            List<int> allBoardPoses = new List<int>();
            List<int> boardPosTurns = new List<int>();

            MoveDirs prevDir = MoveDirs.None;
            Point boardPos = end;
            Point actualPos = end * CellSize;
            while (boardPos != start)
            {
                allBoardPoses.Add(CellIndex(boardPos));

                ScorePath path = GetCellScorePath(boardPos);
                if (path.DirFrom != prevDir)
                {
                    pathAsTurns.Add(actualPos);
                    if (prevDir != MoveDirs.None)
                    {
                        boardPosTurns.Add(CellIndex(boardPos));
                    }
                }
                prevDir = path.DirFrom;
                boardPos = path.DirFrom.MovePoint(boardPos);
                actualPos = actualPos + path.DirFrom.MovePoint(Point.Zero) * CellSize;
            }

            allBoardPoses.Add(CellIndex(start));
            pathAsTurns.Add(actualPos);
            pathAsTurns.Reverse();
            boardPosTurns.Reverse();

            return new WirePath(startIO, endIO, pathAsTurns, allBoardPoses, boardPosTurns, pathToWire);
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

        internal string BoardAllowedMovesToString(Point start, Point end)
        {   
            //Right Left Down Right
            string[] mas = new string[((int)MoveDirs.All) + 1];
            mas[0b0000] = " ";
            mas[0b0001] = "╹";
            mas[0b0010] = "╻";
            mas[0b0011] = "┃";
            mas[0b0100] = "╸";
            mas[0b0101] = "┛";
            mas[0b0110] = "┓";
            mas[0b0111] = "┫";
            mas[0b1000] = "╺";
            mas[0b1001] = "┗";
            mas[0b1010] = "┏";
            mas[0b1011] = "┣";
            mas[0b1100] = "━";
            mas[0b1101] = "┻";
            mas[0b1110] = "┳";
            mas[0b1111] = "╋";

            StringBuilder sBuilder = new StringBuilder();
            for (int y = 0; y < CellsHigh; y++)
            {
                for (int x = 0; x < CellsWide; x++)
                {
                    Point pos = new Point(x, y);
                    if (pos == start)
                    {
                        sBuilder.Append('S');
                    }
                    else if (pos == end)
                    {
                        sBuilder.Append('E');
                    }
                    else
                    {
                        MoveDirs allowedDirs = GetCellMoves(pos);
                        sBuilder.Append(mas[(int)(allowedDirs & MoveDirs.All)]);
                    }
                }
                sBuilder.AppendLine();
            }

            return sBuilder.ToString();
        }
    }
}
