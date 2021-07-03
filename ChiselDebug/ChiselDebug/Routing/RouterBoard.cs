using System;
using System.Collections.Generic;
using System.Text;

namespace ChiselDebug.Routing
{
    internal class RouterBoard
    {
        public readonly int CellsWide;
        public readonly int CellsHigh;
        private readonly MoveDirs[] CellAllowedDirs;
        private readonly ScorePath[] CellScoreAndPath;
        private readonly MoveDirs[] CheckpointAllowedDirs;
        private readonly ScorePath[] CheckpointScoreAndPath;

        public const int CellSize = 10;
        public const int MinSeparation = 4;
        private const int RectPadding = 2;

        public RouterBoard(Point neededBoardSize)
        {
            this.CellsWide = CeilDiv(neededBoardSize.X, CellSize) + 1;
            this.CellsHigh = CeilDiv(neededBoardSize.Y, CellSize) + 1;

            this.CellAllowedDirs = new MoveDirs[CellsWide * CellsHigh];
            this.CellScoreAndPath = new ScorePath[CellAllowedDirs.Length];

            this.CheckpointAllowedDirs = new MoveDirs[CellAllowedDirs.Length];
            this.CheckpointScoreAndPath = new ScorePath[CellAllowedDirs.Length];
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

        public Point CellFromIndex(int index)
        {
            int x;
            int y = Math.DivRem(index, CellsWide, out x);
            return new Point(x, y);
        }

        internal MoveDirs GetCellMoves(Point pos)
        {
            return GetCellMoves(CellIndex(pos));
        }

        internal MoveDirs GetCellMoves(int cellIndex)
        {
            return CellAllowedDirs[cellIndex];
        }

        internal ref ScorePath GetCellScorePath(Point pos)
        {
            return ref GetCellScorePath(CellIndex(pos));
        }

        internal ref ScorePath GetCellScorePath(int cellIndex)
        {
            return ref CellScoreAndPath[cellIndex];
        }

        internal Point GetRelativeBoardPos(Point pos)
        {
            return new Point(pos.X / CellSize, pos.Y / CellSize);
        }

        internal void PrepareBoard(List<Rectangle> usedSpace)
        {
            //Start with all moves are legal
            MoveDirs allDirs = MoveDirs.All;
            Array.Fill(CellAllowedDirs, allDirs);
            Array.Fill(CellScoreAndPath, ScorePath.NotReachedYet());

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

            //Moves that go into used space are not allowed.
            //Moves inside used space are not removed because
            //they shouldn't be reachable.
            for (int i = 0; i < usedSpace.Count; i++)
            {
                Rectangle spaceRel = GetRelativeBoard(usedSpace[i]);
                for (int x = spaceRel.LeftX + 1; x < spaceRel.RightX; x++)
                {
                    CellAllowedDirs[CellIndex(x, spaceRel.TopY)] &= MoveDirs.ExceptDown;
                    CellAllowedDirs[CellIndex(x, spaceRel.BottomY)] &= MoveDirs.ExceptUp;
                }
                for (int y = spaceRel.TopY + 1; y < spaceRel.BottomY; y++)
                {
                    CellAllowedDirs[CellIndex(spaceRel.LeftX, y)] &= MoveDirs.ExceptRight;
                    CellAllowedDirs[CellIndex(spaceRel.RightX, y)] &= MoveDirs.ExceptLeft;
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
            Array.Copy(CellAllowedDirs, CheckpointAllowedDirs, CellAllowedDirs.Length);
            Array.Copy(CellScoreAndPath, CheckpointScoreAndPath, CellScoreAndPath.Length);
        }

        internal void ReloadCheckpoint()
        {
            Array.Copy(CheckpointAllowedDirs, CellAllowedDirs, CellAllowedDirs.Length);
            Array.Copy(CheckpointScoreAndPath, CellScoreAndPath, CellScoreAndPath.Length);
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
            AddCellAllowedMoves(CellIndex(pos), moves);
        }

        internal void AddCellAllowedMoves(int posIndex, MoveDirs moves)
        {
            CellAllowedDirs[posIndex] |= moves;
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
