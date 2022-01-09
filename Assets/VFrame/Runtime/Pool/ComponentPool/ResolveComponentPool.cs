using System;
using VFrame.Pool.ComponentPool.UniRx.Toolkit;
using UnityEngine;
using VContainer;
using VFrame.Extension;

namespace VFrame.Pool.ComponentPool
{
    public class ResolveComponentPool<T> : ObjectPool<T>, IComponentPool<T> where T : Component
    {
        private readonly IObjectResolver _resolver;
        private readonly Transform _parent;

        public ResolveComponentPool(IObjectResolver resolver, Transform parent)
        {
            _resolver = resolver;
            _parent = parent;
        }

        protected override T CreateInstance()
        {
            return _resolver.Resolve<T>();
        }

        protected override void OnBeforeRent(T instance)
        {
            base.OnBeforeRent(instance);
            this.ChangeParent(instance, _parent);
        }

        protected override void OnBeforeReturn(T instance)
        {
            base.OnBeforeReturn(instance);
            this.ChangeParent(instance, _parent);
        }
    }

    public class ResolveComponentPool<TParam, T> : ObjectPool<T>, IComponentPool<TParam, T>
        where T : Component, IRentable<TParam>
    {
        private readonly IObjectResolver _resolver;
        private readonly T _prefab;
        private readonly Transform _parent;
        private IDisposable _beforeHandler;

        public ResolveComponentPool(IObjectResolver resolver, T prefab, Transform parent)
        {
            _resolver = resolver;
            _prefab = prefab;
            _parent = parent;
        }

        protected override T CreateInstance()
        {
            _beforeHandler = _prefab.InstantiateBefore(out T component, _parent);
            _resolver.Inject(component);
            return component;
        }

        protected override void OnBeforeRent(T instance)
        {
            this.ChangeParent(instance, _parent);
        }

        protected override void OnBeforeReturn(T instance)
        {
            base.OnBeforeReturn(instance);
            this.ChangeParent(instance, _parent);
        }

        public T Rent(TParam p1)
        {
            var instance = Rent();
            instance.Rented(p1);
            if (_beforeHandler == null)
            {
                instance.gameObject.SetActive(true);
                return instance;
            }

            _beforeHandler.Dispose();
            _beforeHandler = null;
            return instance;
        }
    }

   
}