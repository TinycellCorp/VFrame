using System;
using VFrame.Pool.ComponentPool.UniRx.Toolkit;
using UnityEngine;

namespace VFrame.Pool.ComponentPool
{
    public class FactoryComponentPool<T> : ObjectPool<T>, IComponentPool<T> where T : Component
    {
        private readonly Func<T> _factory;

        public FactoryComponentPool(Func<T> factory)
        {
            _factory = factory;
        }

        protected override T CreateInstance()
        {
            return _factory.Invoke();
        }
    }
}