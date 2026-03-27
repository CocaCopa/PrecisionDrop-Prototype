using CocaCopa.Primitives;

namespace PrecisionDrop.SessionTracker.Contracts {
    public readonly struct ScorePopupInfo {
        public readonly ScoreInfo ScoreInfo;
        public readonly C_Vector3 ContactPoint;

        public ScorePopupInfo(ScoreInfo scoreInfo, C_Vector3 contactPoint) {
            ScoreInfo = scoreInfo;
            ContactPoint = contactPoint;
        }
    }
}