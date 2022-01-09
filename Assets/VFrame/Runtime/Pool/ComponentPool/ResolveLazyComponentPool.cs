using System;
using VFrame.Extension;
using VFrame.Pool.ComponentPool.UniRx.Toolkit;
using UnityEngine;
using VContainer;

namespace VFrame.Pool.ComponentPool
{

    public interface ILazyComponentPoolInitializable
    {
        void Initialize(GameObject prefab);
    }

    public abstract class ResolveLazyComponentPoolBase<T> : ObjectPool<T>, ILazyComponentPoolInitializable
        where T : Component
    {
        private readonly IObjectResolver _resolver;
        private readonly Transform _parent;
        public bool IsInitialize { get; private set; } = false;
        private GameObject _prefab;
        private IDisposable _beforeHandler;

        private GameObject Prefab
        {
            get
            {
                if (!IsInitialize) throw new Exception($"not initialized lazy pool: [{typeof(T).Name}]");

                return _prefab;
            }
        }

        protected ResolveLazyComponentPoolBase(IObjectResolver resolver, Transform parent)
        {
            _resolver = resolver;
            _parent = parent;
        }

        public void Initialize(GameObject prefab)
        {
            if (IsInitialize) throw new Exception($"initialized lazy pool: {typeof(T).Name}");
            IsInitialize = true;
            _prefab = prefab;
        }

        protected sealed override T CreateInstance()
        {
            _beforeHandler = Prefab.InstantiateBefore(out T component, _parent);
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


        protected void EndBuild(T instance)
        {
            if (_beforeHandler == null)
            {
                instance.gameObject.SetActive(true);
            }
            else
            {
                _beforeHandler.Dispose();
                _beforeHandler = null;
            }
        }


        protected ParameterBuilder Build(out T component)
        {
            component = Rent();
            return new ParameterBuilder(this, component);
        }

        protected readonly struct ParameterBuilder : IDisposable
        {
            private readonly ResolveLazyComponentPoolBase<T> _pool;
            private readonly T _instance;

            public ParameterBuilder(ResolveLazyComponentPoolBase<T> pool, T instance)
            {
                _pool = pool;
                _instance = instance;
            }

            public void Dispose()
            {
                _pool.EndBuild(_instance);
            }
        }
    }

    public class ResolveLazyComponentPool<T> : ResolveLazyComponentPoolBase<T>, ILazyComponentPool<T>
        where T : Component
    {
        public ResolveLazyComponentPool(IObjectResolver resolver, Transform parent) : base(resolver, parent)
        {
        }

        protected override void OnBeforeRent(T instance)
        {
            base.OnBeforeRent(instance);
            EndBuild(instance);
        }
    }

    public class ResolveLazyComponentPool<TParam, T> : ResolveLazyComponentPoolBase<T>, ILazyComponentPool<TParam, T>
        where T : Component, IRentable<TParam>
    {
        public ResolveLazyComponentPool(IObjectResolver resolver, Transform parent) : base(resolver, parent)
        {
        }

        public T Rent(TParam p1)
        {
            // ver.1
            // var instance = Rent();
            // instance.Rented(p1);
            // EndBuild(instance);
            // return instance;

            // ver.2
            using (Build(out var component))
            {
                component.Rented(p1);
                return component;
            }
        }
    }

    public class ResolveLazyComponentPool<TParam1, TParam2, T> : ResolveLazyComponentPoolBase<T>,
        ILazyComponentPool<TParam1, TParam2, T>
        where T : Component, IRentable<TParam1, TParam2>
    {
        public ResolveLazyComponentPool(IObjectResolver resolver, Transform parent) : base(resolver, parent)
        {
        }

        public T Rent(TParam1 p1, TParam2 p2)
        {
            using (Build(out var component))
            {
                component.Rented(p1, p2);
                return component;
            }
        }
    }
}