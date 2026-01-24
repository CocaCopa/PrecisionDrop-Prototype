using CocaCopa.Primitives;

namespace PrecisionDrop.Platforms.Contracts {
    public struct PlatformConfig {
        public float rotationY;
        public RangeInt gapPositions;
        public RangeInt dangerPositions;
        public bool hasPowerUp;

        public PlatformConfig(float rotationY, RangeInt gapPositions, RangeInt dangerPositions, bool hasPowerUp) {
            this.rotationY = rotationY;
            this.gapPositions = gapPositions;
            this.dangerPositions = dangerPositions;
            this.hasPowerUp = hasPowerUp;
        }
    }
}
