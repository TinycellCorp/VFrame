namespace VFrame.Pool.ObjectPool
{
    public interface IObjectPool<T> : IPool<T> where T : IPoolable
    {
    }

    public interface IObjectPool<in TParam, T> : IPool<T> where T : IPoolable<TParam>
    {
        T Rent(TParam param);
    }
}