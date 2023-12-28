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


        public ScorePath Move(MoveDirs revDir, int onEnemyWire, int moveToEnemyWire, int moveToFriendWire, int moveToWireCorner, bool isTurning)
        {
            int isTurningInt = BoolToInt(isTurning);

            int turns = TravelDist + 1; // +1 so not moving to friendly wire results in +1 - 0 and moving to friendly wire results in +1 - 1
            turns += isTurningInt * 5;
            turns += (isTurningInt & onEnemyWire) * 50;
            turns += moveToEnemyWire * 5;
            turns += (moveToEnemyWire & moveToWireCorner) * 500;
            turns -= moveToFriendWire;

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
