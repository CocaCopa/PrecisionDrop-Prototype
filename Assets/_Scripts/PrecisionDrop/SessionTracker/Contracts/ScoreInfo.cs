namespace PrecisionDrop.SessionTracker.Contracts {
    public struct ScoreInfo {
        public readonly ScoreType ScoreType;
        public readonly int AddedAmount;
        public readonly int Total;

        public ScoreInfo(ScoreType scoreType, int addedAmount, int total) {
            ScoreType = scoreType;
            AddedAmount = addedAmount;
            Total = total;
        }
    }
}