using UnityEngine;

namespace VFrame.Pool.ComponentPool
{
    public interface ILazyComponentPool<T> : IComponentPool<T> where T : Component
    {
        bool IsInitialize { get; }
    }

    public interface ILazyComponentPool<in TParam, T> : ILazyComponentPool<T> where T : Component
    {
        T Rent(TParam p1);
    }


    public interface ILazyComponentPool<in TParam1, in TParam2, T> : ILazyComponentPool<T> where T : Component
    {
        T Rent(TParam1 p1, TParam2 p2);
    }
}