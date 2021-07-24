using ChiselDebug;

namespace ChiselDebuggerRazor.Code
{
    public class ScopedDirIO
    {
        internal DirectedIO DirIO { get; private set; }
        internal readonly int ScopeXOffset;

        public ScopedDirIO(DirectedIO dirIO, int scopeXOffset)
        {
            this.DirIO = dirIO;
            this.ScopeXOffset = scopeXOffset;
        }

        public void SetX(int newX)
        {
            Point currPos = DirIO.Position;
            Point newPos = new Point(newX, DirIO.Position.Y);
            Point offset = newPos - currPos;

            DirIO = DirIO.WithOffsetPosition(offset);
        }

        public void SetY(int newY)
        {
            Point currPos = DirIO.Position;
            Point newPos = new Point(currPos.X, newY);
            Point offset = newPos - currPos;

            DirIO = DirIO.WithOffsetPosition(offset);
        }

        public ScopedDirIO Copy()
        {
            return new ScopedDirIO(DirIO.WithOffsetPosition(Point.Zero), ScopeXOffset);
        }
    }
}
