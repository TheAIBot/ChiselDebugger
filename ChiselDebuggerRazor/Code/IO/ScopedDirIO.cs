using ChiselDebug.Routing;
using ChiselDebug.Utilities;

namespace ChiselDebuggerRazor.Code.IO
{
    public class ScopedDirIO
    {
        internal DirectedIO DirIO { get; private set; }
        internal readonly int ScopeXOffset;

        public ScopedDirIO(DirectedIO dirIO, int scopeXOffset)
        {
            DirIO = dirIO;
            ScopeXOffset = scopeXOffset;
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
    }
}
