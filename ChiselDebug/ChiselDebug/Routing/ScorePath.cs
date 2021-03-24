namespace ChiselDebug.Routing
{
    internal readonly struct ScorePath
    {
        public readonly ushort TraveledDist;
        public readonly ushort TurnsTaken;
        public readonly MoveDirs DirFrom;

        public ScorePath(int travled, int turns, MoveDirs fromDir)
        {
            this.TraveledDist = (ushort)travled;
            this.TurnsTaken = (ushort)turns;
            this.DirFrom = fromDir;
        }

        public ScorePath Move(MoveDirs revDir, bool onEnemyWire, bool onWireCorner, bool isTurningOnEnemyWire)
        {
            //If the direction we are moving in is not the
            //opposite of the direction we came from,
            //then we must be turning unless we started
            //the path from this point. If we didn't come
            //from any direction then we must've started
            //the path from here.
            bool isTurning = DirFrom != revDir && DirFrom != MoveDirs.None;
            int turns = TurnsTaken;
            turns += isTurning ? 1 : 0;
            turns += onEnemyWire ? 1 : 0;
            turns += onWireCorner ? 500 : 0;
            turns += isTurningOnEnemyWire ? 50 : 0;

            return new ScorePath(TraveledDist + 1, turns, revDir);
        }

        public bool IsBetterScoreThan(ScorePath score)
        {
            return GetTotalScore() < score.GetTotalScore();
            //if (TraveledDist < score.TraveledDist)
            //{
            //    return true;
            //}

            //return TraveledDist == score.TraveledDist &&
            //       TurnsTaken < score.TurnsTaken;
        }

        public int GetTotalScore()
        {
            return TraveledDist + TurnsTaken;
        }

        public static ScorePath NotReachedYet()
        {
            return new ScorePath(ushort.MaxValue, ushort.MaxValue, MoveDirs.None);
        }
    }
}
