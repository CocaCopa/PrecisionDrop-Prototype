using System;

namespace CocaCopa.Core.Randomization {
    public static class RandomUtil {
        private static readonly Random SharedRng = new();

        /// <summary>
        /// Returns a random boolean value with a 50% probability for <c>true</c> and <c>false</c>.
        /// </summary>
        public static bool Bool() {
            return SharedRng.Next(2) == 0;
        }

        #region Float
        /// <summary>
        /// Returns a random floating-point value between <paramref name="minInclusive"/> and <paramref name="maxExclusive"/>.
        /// </summary>
        public static float Float(float minInclusive, float maxExclusive) {
            return Float(minInclusive, maxExclusive, SharedRng);
        }

        /// <summary>
        /// Returns a random floating-point value between <paramref name="minInclusive"/> and <paramref name="maxExclusive"/>
        /// using the provided <see cref="Random"/> instance.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="minInclusive"/> is greater than <paramref name="maxExclusive"/>.
        /// </exception>
        public static float Float(float minInclusive, float maxExclusive, Random rng) {
            if (minInclusive > maxExclusive) { throw new ArgumentException("min cannot be greater than max"); }
            return (float)(rng.NextDouble() * (maxExclusive - minInclusive) + minInclusive);
        }
        #endregion

        #region Int
        /// <summary>
        /// Returns a random integer between <paramref name="minInclusive"/> and <paramref name="maxInclusive"/>.
        /// </summary>
        public static int Int(int minInclusive, int maxInclusive) {
            return Int(minInclusive, maxInclusive, SharedRng);
        }

        /// <summary>
        /// Returns a random integer between <paramref name="minInclusive"/> and <paramref name="maxInclusive"/>
        /// using the provided <see cref="Random"/> instance.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="minInclusive"/> is greater than <paramref name="maxInclusive"/>.
        /// </exception>
        public static int Int(int minInclusive, int maxInclusive, Random rng) {
            if (minInclusive > maxInclusive) { throw new ArgumentException("min cannot be greater than max"); }
            return rng.Next(minInclusive, maxInclusive + 1);
        }
        #endregion
    }
}