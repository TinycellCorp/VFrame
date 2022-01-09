namespace VFrame.Pool.ComponentPool
{
    public interface IComponentPool<T> : IPool<T> where T : UnityEngine.Component
    {
    }

    public interface IComponentPool<in TParam, T> : IComponentPool<T> where T : UnityEngine.Component
    {
        T Rent(TParam p1);
    }
}