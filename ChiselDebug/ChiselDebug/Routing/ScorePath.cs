namespace ChiselDebug.Routing
{
    internal readonly struct ScorePath
    {
        private readonly uint Data;
        private int TravelDist => (int)Data & 0xffffff;
        public MoveDirs DirFrom => (MoveDirs)((Data >> 24) & 0xff);

        public ScorePath(int travled, MoveDirs fromDir)
        {
            this.Data = (uint)travled | ((uint)fromDir << 24);
        }

        public ScorePath Move(MoveDirs revDir, bool onEnemyWire, bool onFriendWire, bool onWireCorner, bool isTurningOnEnemyWire)
        {
            //If the direction we are moving in is not the
            //opposite of the direction we came from,
            //then we must be turning unless we started
            //the path from this point. If we didn't come
            //from any direction then we must've started
            //the path from here.
            bool isTurning = DirFrom != revDir && DirFrom != MoveDirs.None;
            int turns = TravelDist;
            turns += isTurning ? 1 : 0;
            turns += onEnemyWire ? 1 : 0;
            turns += onWireCorner ? 500 : 0;
            turns += isTurningOnEnemyWire ? 50 : 0;

            return new ScorePath(turns + (onFriendWire ? 0 : 1), revDir);
        }

        public bool IsBetterScoreThan(ScorePath score)
        {
            return GetTotalScore() < score.GetTotalScore();
        }

        public int GetTotalScore()
        {
            return TravelDist;
        }

        public static ScorePath NotReachedYet()
        {
            return new ScorePath(0xffffff, MoveDirs.None);
        }
    }
}
