namespace CocaCopa.ObjectPooling {
    public interface IPoolable {
        void ResetForReuse();
        void PrepareForRelease();
    }
}