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

        internal WirePath GetPath(Point start, Point end, Point actualStart, Point actualEnd, bool pathToWire)
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

            return new WirePath(pathAsTurns, pathToWire);
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
