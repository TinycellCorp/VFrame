using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace VFrame.Pool.ComponentPool
{
    public interface ILazyVariantComponentPoolInitializable
    {
        void Initialize(Action<Dictionary<string, GameObject>> configuration);
    }

    public abstract class ResolveLazyVariantComponentPoolBase<TPool, T> : ILazyVariantComponentPoolInitializable
        where TPool : ResolveLazyComponentPoolBase<T>
        where T : Component
    {
        private readonly Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();
        private readonly Dictionary<string, TPool> _pools = new Dictionary<string, TPool>();

        private readonly IObjectResolver _resolver;
        private readonly Transform _parent;

        public bool IsInitialize { get; private set; } = false;


        protected ResolveLazyVariantComponentPoolBase(IObjectResolver resolver, Transform parent)
        {
            _resolver = resolver;
            _parent = parent;
        }

        public void Initialize(Action<Dictionary<string, GameObject>> configuration)
        {
            if (IsInitialize) throw new Exception($"initialized lazy variant pool: {typeof(T).Name}");

            configuration(_prefabs);
            foreach (var pair in _prefabs)
            {
                var pool = CreatePool(_resolver, _parent);
                pool.Initialize(pair.Value);
                _pools.Add(pair.Key, pool);
            }

            IsInitialize = true;
        }

        public T Rent() => throw new Exception("require using key");

        public void Return(T instance) => throw new Exception("require using key");

        public T Rent(string key)
        {
            var pool = GetPool(key);
            return pool.Rent();
        }

        public void Return(string key, T component)
        {
            var pool = GetPool(key);
            pool.Return(component);
        }

        protected abstract TPool CreatePool(IObjectResolver resolver, Transform parent);

        protected TPool GetPool(string key)
        {
            if (_pools.TryGetValue(key, out var pool))
            {
                return pool;
            }

            throw new KeyNotFoundException($"{typeof(T).Name} {key}");
        }
    }

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