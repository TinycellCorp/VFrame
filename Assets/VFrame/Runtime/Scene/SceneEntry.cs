using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using VFrame.Pool.ComponentPool;
using VFrame.Pool.Standalone;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;

namespace VFrame.Scene
{
    public abstract class SceneEntry : IInitializable, IAsyncStartable, IDisposable
    {
        private readonly IObjectResolver _resolver;

        private readonly IPreloader _preloader;

        protected AssetCache Assets { get; private set; }

        public SceneEntry(IObjectResolver resolver)
        {
            _resolver = resolver;

            _preloader = _resolver.Resolve<IPreloader>();
            Assets = _resolver.Resolve<AssetCache>();
        }

        public abstract void Initialize();

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            var prefabKeys = _resolver.Resolve<IReadOnlyList<IPreloadPrefabReservable>>();
            foreach (var reserved in prefabKeys)
            {
                var pool = _resolver.Resolve(reserved.PoolType);
                if (pool is ILazyComponentPoolInitializable initializable)
                {
                    var prefab = await Addressables.LoadAssetAsync<GameObject>(reserved.BundleKey).Task.AsUniTask();
                    Assets.RegisterPrefab(reserved.BundleKey, prefab);
                    initializable.Initialize(prefab);
                }
            }

            var prefabVariantKeys = _resolver.Resolve<IReadOnlyList<IPreloadPrefabVariantReservable>>();
            foreach (var reserved in prefabVariantKeys)
            {
                var pool = _resolver.Resolve(reserved.PoolType);
                if (pool is ILazyVariantComponentPoolInitializable initializable)
                {
                    foreach (var key in reserved.BundleKeys)
                    {
                        var prefab = await Addressables.LoadAssetAsync<GameObject>(key).Task.AsUniTask();
                        Assets.RegisterPrefab(key, prefab);
                    }

                    initializable.Initialize(builder =>
                    {
                        foreach (var key in reserved.BundleKeys)
                        {
                            builder.Add(key, Assets.GetPrefab(key));
                        }
                    });
                }
            }

            await _preloader.PreloadAsync(Assets);
            await PostStartAsync(cancellation);
        }

        protected virtual UniTask PostStartAsync(CancellationToken cancellation)
        {
            return UniTask.CompletedTask;
        }

        
        public CancellationToken GetCancellationTokenOnDestroy()
        {
            var scope = _resolver.Resolve<LifetimeScope>();
            return scope.GetCancellationTokenOnDestroy();
        }

        public void Dispose()
        {
            Assets.Dispose();
            OnDisposed();
        }

        protected abstract void OnDisposed();

        protected AddressablePool<T> CreateAddressablePool<T>(string key) where T : Component
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            return new AddressablePool<T>(_resolver, key);
        }
    }
}