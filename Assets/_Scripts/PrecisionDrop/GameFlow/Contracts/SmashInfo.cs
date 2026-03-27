using CocaCopa.Primitives;

namespace PrecisionDrop.GameFlow.Contracts {
    public readonly struct SmashInfo {
        public readonly SmashType SmashType;
        public readonly C_Vector3 CollisionPoint;

        public SmashInfo(SmashType smashType, C_Vector3 collisionPoint) {
            SmashType = smashType;
            CollisionPoint = collisionPoint;
        }
    }
}