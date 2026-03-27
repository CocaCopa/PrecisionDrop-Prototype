using CocaCopa.Primitives;

namespace PrecisionDrop.Platforms.Contracts {
    public readonly struct CollisionData {
        public readonly PieceVariant PieceVariant;
        public readonly C_Vector3 ContactPoint;

        public CollisionData(PieceVariant pieceVariant, C_Vector3 contactPoint) {
            PieceVariant = pieceVariant;
            ContactPoint = contactPoint;
        }
    }
}