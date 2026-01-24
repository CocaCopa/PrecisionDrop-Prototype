using System;

namespace CocaCopa.Primitives {
    [Serializable]
    public struct RangeInt {
        public int min;
        public int max;
        public readonly int Length => max - min;

        public RangeInt(int min, int max) {
            this.min = min;
            this.max = max;
        }
        public static implicit operator RangeInt((int min, int max) t) => new RangeInt(t.min, t.max);
    }

    [Serializable]
    public struct RangeFloat {
        public float min;
        public float max;
        public readonly float Length => max - min;

        public RangeFloat(float min, float max) {
            this.min = min;
            this.max = max;
        }
        public static implicit operator RangeFloat((float min, float max) t) => new RangeFloat(t.min, t.max);
    }
}
