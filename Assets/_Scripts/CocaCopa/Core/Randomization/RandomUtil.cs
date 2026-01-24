using System;

namespace CocaCopa.Core.Randomization {
    public static class RandomUtil {
        private static readonly Random sharedRng = new Random();

        #region Float
        public static float Float(float min, float max) {
            return Float(min, max, sharedRng);
        }

        public static float Float(float min, float max, Random rng) {
            if (min > max) { throw new ArgumentException("min cannot be greater than max"); }
            return (float)(rng.NextDouble() * (max - min) + min);
        }
        #endregion

        #region Int
        public static int Int(int min, int max) {
            return Int(min, max, sharedRng);
        }

        public static int Int(int min, int max, Random rng) {
            if (min > max) { throw new ArgumentException("min cannot be greater than max"); }
            return rng.Next(min, max + 1);
        }
        #endregion
    }
}
