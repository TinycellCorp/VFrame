using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VFrame.Pool;
using VFrame.Pool.ComponentPool;
using VFrame.Pool.ObjectPool;
using VFrame.Scene;

namespace VFrame.Extension
{
    public static class ContainerBuilderExtensions
    {
        #region Use Factory

        public static void RegisterComponentPool<T>(this IContainerBuilder builder, Func<T> factory)
            where T : Component
        {
            builder.RegisterFactory(factory); // Lifetime.Singleton
            builder.Register<IComponentPool<T>, FactoryComponentPool<T>>(Lifetime.Singleton);
        }

        public static void RegisterComponentPool<T>(this IContainerBuilder builder,
            Func<IObjectResolver, Func<T>> factory, Lifetime lifetime) where T : Component
        {
            builder.RegisterFactory(factory, lifetime);
            builder.Register<IComponentPool<T>, FactoryComponentPool<T>>(lifetime);
        }

        public static void RegisterComponentPool<TFactory, TComponent>(this IContainerBuilder builder,
            TComponent prefab,
            Func<IObjectResolver, Func<TComponent>> factory, Lifetime lifetime) where TComponent : Component
        {
            builder.RegisterInstance(prefab);
            builder.Register<TFactory>(lifetime);
            builder.RegisterFactory(factory, lifetime);
            builder.Register<IComponentPool<TComponent>, FactoryComponentPool<TComponent>>(lifetime);
        }

        #endregion

        #region ObjectPool

        public static void RegisterObjectPool<T>(this IContainerBuilder builder, Lifetime lifetime = Lifetime.Singleton)
            where T : IPoolable
        {
            builder.Register<T>(Lifetime.Transient);
            builder.RegisterFactory<T>(_ => _.Resolve<T>, lifetime);
            builder.Register<IObjectPool<T>, ObjectPool<T>>(lifetime);
        }

        public static void RegisterObjectPool<TParam, T>(this IContainerBuilder builder,
            Lifetime lifetime = Lifetime.Singleton)
            where T : IPoolable<TParam>
        {
            builder.Register<T>(Lifetime.Transient);
            builder.RegisterFactory<T>(_ => _.Resolve<T>, lifetime);
            builder.Register<IObjectPool<TParam, T>, ObjectPool<TParam, T>>(lifetime);
        }

        #endregion

        #region IComponentPool

        public static void RegisterComponentPool<T>(this IContainerBuilder builder, T prefab,
            Transform parent = null,
            Lifetime lifetime = Lifetime.Singleton)
            where T : Component
        {
            builder.RegisterComponentInNewPrefab(prefab, Lifetime.Transient).UnderTransform(parent);
            builder.Register<IComponentPool<T>, ResolveComponentPool<T>>(lifetime).WithParameter(parent);
        }

        public static void RegisterComponentPool<TParam, T>(this IContainerBuilder builder, T prefab,
            Transform parent = null, Lifetime lifetime = Lifetime.Singleton)
            where T : Component, IRentable<TParam>
        {
            builder.Register<IComponentPool<TParam, T>, ResolveComponentPool<TParam, T>>(lifetime)
                .WithParameter(prefab)
                .WithParameter(parent);
        }

        #endregion

        #region ILazyComponentPool

        public static void RegisterLazyComponentPool<T>(this IContainerBuilder builder, string key,
            Transform parent = null,
            Lifetime lifetime = Lifetime.Singleton)
            where T : Component
        {
            builder.Register<ILazyComponentPool<T>, ResolveLazyComponentPool<T>>(lifetime).AsSelf()
                .WithParameter(parent);
            builder.Register<IPreloadPrefabReservable, AddressablePrefabReservable>(Lifetime.Scoped)
                .WithParameter(key)
                .WithParameter(typeof(ResolveLazyComponentPool<T>));
        }

        public static void RegisterLazyComponentPool<TParam, T>(this IContainerBuilder builder,
            string key, Transform parent, Lifetime lifetime = Lifetime.Singleton)
            where T : Component, IRentable<TParam>
        {
            builder.Register<ILazyComponentPool<TParam, T>, ResolveLazyComponentPool<TParam, T>>(lifetime).AsSelf()
                .WithParameter(parent);
            builder.Register<IPreloadPrefabReservable, AddressablePrefabReservable>(Lifetime.Scoped)
                .WithParameter(key)
                .WithParameter(typeof(ResolveLazyComponentPool<TParam, T>));
        }

        public static void RegisterLazyComponentPool<TParam1, TParam2, T>(this IContainerBuilder builder,
            string key, Transform parent = null, Lifetime lifetime = Lifetime.Singleton)
            where T : Component, IRentable<TParam1, TParam2>
        {
            builder
                .Register<ILazyComponentPool<TParam1, TParam2, T>,
                    ResolveLazyComponentPool<TParam1, TParam2, T>>(lifetime).AsSelf()
                .WithParameter(parent);
            builder.Register<IPreloadPrefabReservable, AddressablePrefabReservable>(Lifetime.Scoped)
                .WithParameter(key)
                .WithParameter(typeof(ResolveLazyComponentPool<TParam1, TParam2, T>));
        }

        #endregion

        #region ILazyVariantComponentPool

        public static void RegisterLazyVariantComponentPool<T>(this IContainerBuilder builder,
            IReadOnlyList<string> keys, Transform parent = null, Lifetime lifetime = Lifetime.Singleton)
            where T : Component
        {
            builder.Register<ILazyVariantComponentPool<T>, ResolveLazyVariantComponentPool<T>>(lifetime).AsSelf()
                .WithParameter(parent);
            builder.Register<IPreloadPrefabVariantReservable, AddressablePrefabVariantReservable>(Lifetime.Scoped)
                .WithParameter(keys)
                .WithParameter(typeof(ResolveLazyVariantComponentPool<T>));
        }

        public static void RegisterLazyVariantComponentPool<TParam, T>(this IContainerBuilder builder,
            IReadOnlyList<string> keys, Transform parent = null, Lifetime lifetime = Lifetime.Singleton)
            where T : Component, IRentable<TParam>
        {
            builder.Register<ILazyVariantComponentPool<TParam, T>, ResolveLazyVariantComponentPool<TParam, T>>(lifetime)
                .AsSelf()
                .WithParameter(parent);
            builder.Register<IPreloadPrefabVariantReservable, AddressablePrefabVariantReservable>(Lifetime.Scoped)
                .WithParameter(keys)
                .WithParameter(typeof(ResolveLazyVariantComponentPool<TParam, T>));
        }

        #endregion

    }
}