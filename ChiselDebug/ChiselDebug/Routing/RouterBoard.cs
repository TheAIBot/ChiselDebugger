using System;
using System.Collections.Generic;
using System.Text;

namespace ChiselDebug.Routing
{
    internal class RouterBoard
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
            this.TopLeft = Point.Zero;
            this.BottomRight = neededBoardSize;

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

            //Moves that go into used space are not allowed.
            //Moves inside used space are not removed because
            //they shouldn't be reachable.
            for (int i = 0; i < usedSpace.Count; i++)
            {
                Rectangle spaceRel = GetRelativeBoard(usedSpace[i]);

                //Need to remove moves not at the edge of the used space
                //but at the outer neighbors to the uses space. That's
                //why the rectangle is made 1 larger on all sides.
                Rectangle spaceCellEdge = spaceRel.ResizeCentered(1);
                for (int x = spaceCellEdge.LeftX + 1; x < spaceCellEdge.RightX; x++)
                {
                    CellAllowedDirs[CellIndex(x, spaceCellEdge.TopY)] &= MoveDirs.ExceptDown;
                    CellAllowedDirs[CellIndex(x, spaceCellEdge.BottomY)] &= MoveDirs.ExceptUp;
                }
                for (int y = spaceCellEdge.TopY + 1; y < spaceCellEdge.BottomY; y++)
                {
                    CellAllowedDirs[CellIndex(spaceCellEdge.LeftX, y)] &= MoveDirs.ExceptRight;
                    CellAllowedDirs[CellIndex(spaceCellEdge.RightX, y)] &= MoveDirs.ExceptLeft;
                }
            }
        }

        internal Rectangle GetRelativeBoard(Rectangle rect)
        {
            Point spaceTopLeft = rect.Pos - TopLeft;
            return new Rectangle(
                new Point(
                    spaceTopLeft.X / CellSize,
                    spaceTopLeft.Y / CellSize),
                new Point(
                    CeilDiv(rect.Size.X, CellSize),
                    CeilDiv(rect.Size.Y, CellSize)));
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

        internal WirePath GetPath(Point start, Point end, IOInfo startIO, IOInfo endIO, bool pathToWire)
        {
            List<Point> pathAsTurns = new List<Point>();
            List<Point> allBoardPoses = new List<Point>();
            List<Point> boardPosTurns = new List<Point>();

            MoveDirs prevDir = MoveDirs.None;
            Point boardPos = end;
            Point actualPos = end * CellSize + TopLeft;
            while (boardPos != start)
            {
                allBoardPoses.Add(boardPos);

                ScorePath path = GetCellScorePath(boardPos);
                if (path.DirFrom != prevDir)
                {
                    pathAsTurns.Add(actualPos);
                    boardPosTurns.Add(boardPos);
                }
                prevDir = path.DirFrom;
                boardPos = path.DirFrom.MovePoint(boardPos);
                actualPos = actualPos + path.DirFrom.MovePoint(Point.Zero) * CellSize;
            }

            allBoardPoses.Add(start);
            boardPosTurns.Add(start);
            pathAsTurns.Add(actualPos);
            pathAsTurns.Reverse();

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
