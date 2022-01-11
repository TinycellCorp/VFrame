using UnityEngine;
using VContainer;

namespace VFrame.Pool.ComponentPool
{

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