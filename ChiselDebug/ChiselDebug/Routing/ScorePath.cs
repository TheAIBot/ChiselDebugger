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

        public ScorePath Move(MoveDirs revDir, bool onEnemyWire, bool moveToEnemyWire, bool moveToFriendWire, bool moveToWireCorner, bool isTurning)
        {
            int turns = TravelDist;
            turns += isTurning ? 5 : 0;
            turns += moveToEnemyWire ? 5 : 0;
            turns += moveToEnemyWire && moveToWireCorner ? 500 : 0;
            turns += isTurning && onEnemyWire ? 50 : 0;
            turns += moveToFriendWire ? 0 : 1;

            return new ScorePath(turns, revDir);
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
