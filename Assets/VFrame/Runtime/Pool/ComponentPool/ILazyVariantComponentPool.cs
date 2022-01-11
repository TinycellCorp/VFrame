using UnityEngine;

namespace VFrame.Pool.ComponentPool
{
    public interface ILazyVariantComponentPool<T> : ILazyComponentPool<T> where T : Component
    {
        T Rent(string key);
        void Return(string key, T component);
        void Clear(string key);
    }

    public interface ILazyVariantComponentPool<in TParam, T> : ILazyVariantComponentPool<T>
        where T : Component, IRentable<TParam>
    {
        T Rent(string key, TParam param);
    }
}