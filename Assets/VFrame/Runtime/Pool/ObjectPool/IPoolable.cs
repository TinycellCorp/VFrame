using System;

namespace VFrame.Pool.ObjectPool
{
    public interface IPooled : IDisposable
    {
        void Return();
    }

    public interface IRentable
    {
        void Rented();
    }

    public interface IPoolable : IPooled, IRentable
    {
    }

    public interface IPoolable<in TParam> : IPooled, IRentable<TParam>
    {
    }
}