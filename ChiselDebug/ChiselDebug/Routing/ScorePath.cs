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
            int onEnemyWireInt = BoolToInt(onEnemyWire);
            int moveToEnemyWireInt = BoolToInt(moveToEnemyWire);
            int moveToFriendWireInt = BoolToInt(!moveToFriendWire);
            int moveToWireCornerInt = BoolToInt(moveToWireCorner);
            int isTurningInt = BoolToInt(isTurning);

            int turns = TravelDist;
            turns += isTurningInt * 5;
            turns += moveToEnemyWireInt * 5;
            turns += (moveToEnemyWireInt & moveToWireCornerInt) * 500;
            turns += (isTurningInt & onEnemyWireInt) * 50;
            turns += moveToFriendWireInt;

            return new ScorePath(turns, revDir);
        }

        private unsafe static int BoolToInt(bool value)
        {
            return *((byte*)&value);
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
