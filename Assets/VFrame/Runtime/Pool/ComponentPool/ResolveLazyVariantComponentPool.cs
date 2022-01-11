using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace VFrame.Pool.ComponentPool
{
   
    public class ResolveLazyVariantComponentPool<T> :
        ResolveLazyVariantComponentPoolBase<ResolveLazyComponentPool<T>, T>,
        ILazyVariantComponentPool<T>
        where T : Component
    {
        public ResolveLazyVariantComponentPool(IObjectResolver resolver, Transform parent) : base(resolver, parent)
        {
        }

        protected override ResolveLazyComponentPool<T> CreatePool(IObjectResolver resolver, Transform parent)
        {
            return new ResolveLazyComponentPool<T>(resolver, parent);
        }

    }

    public class ResolveLazyVariantComponentPool<TParam, T> :
        ResolveLazyVariantComponentPoolBase<ResolveLazyComponentPool<TParam, T>, T>,
        ILazyVariantComponentPool<TParam, T>
        where T : Component, IRentable<TParam>
    {
        public ResolveLazyVariantComponentPool(IObjectResolver resolver, Transform parent) : base(resolver, parent)
        {
        }

        protected override ResolveLazyComponentPool<TParam, T> CreatePool(IObjectResolver resolver, Transform parent)
        {
            return new ResolveLazyComponentPool<TParam, T>(resolver, parent);
        }

        public T Rent(string key, TParam param)
        {
            var pool = GetPool(key);
            return pool.Rent(param);
        }
    }
}