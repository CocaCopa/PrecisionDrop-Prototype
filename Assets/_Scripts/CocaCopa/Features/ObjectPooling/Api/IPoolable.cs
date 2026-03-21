namespace CocaCopa.ObjectPooling {
    public interface IPoolable {
        /// <summary>
        /// Invoked once the object is rented from the pool.
        /// </summary>
        void ResetForReuse();

        /// <summary>
        /// Invoked when the object is released back to its pool
        /// </summary>
        void PrepareForRelease();
    }
}