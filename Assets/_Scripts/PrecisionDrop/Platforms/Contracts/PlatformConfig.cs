using CocaCopa.Primitives;

namespace PrecisionDrop.Platforms.Contracts {
    public readonly struct PlatformConfig {
        public readonly float RotationY;
        public readonly RangeInt[] GapPositions;
        public readonly RangeInt DangerPositions;

        public PlatformConfig(float rotationY, RangeInt[] gapPositions, RangeInt dangerPositions) {
            RotationY = rotationY;
            GapPositions = gapPositions;
            DangerPositions = dangerPositions;
        }
    }
}
