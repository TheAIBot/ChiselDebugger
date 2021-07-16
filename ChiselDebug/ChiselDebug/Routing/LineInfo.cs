namespace ChiselDebug.Routing
{
    internal record LineInfo(IOInfo Start, IOInfo End)
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
}
