using ChiselDebug.Utilities;

namespace ChiselDebug.Routing
{
    public readonly record struct Line(Point Start, Point End)
    {
        public int GetManhattanDistance()
        {
            return Point.ManhattanDistance(Start, End);
        }
    }
}
