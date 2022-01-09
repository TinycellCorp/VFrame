using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

namespace VFrame.Pool.Standalone
{
    public class AddressablePool<T> : IDisposable where T : Component
    {
        private readonly IObjectResolver _resolver;
        private readonly string _key;

        private readonly Queue<T> _components = new Queue<T>();
        public AddressablePool(IObjectResolver resolver, string key) => (_resolver, _key) = (resolver, key);

        public async UniTask<T> Rent()
        {
            if (_components.Any())
            {
                var component = _components.Dequeue();
                component.gameObject.SetActive(true);
                return component;
            }
            else
            {
                var instance = await Addressables.InstantiateAsync(_key).Task.AsUniTask();
                var component = instance.GetComponent<T>();
                _resolver.Inject(component);
                return component;
            }
        }

        public void Return(T component)
        {
            component.gameObject.SetActive(false);
            _components.Enqueue(component);
        }

     
        public void Dispose()
        {
            while (_components.Any())
            {
                Addressables.ReleaseInstance(_components.Dequeue().gameObject);
            }
        }

        public void Dispose(IReadOnlyCollection<T> collection)
        {
            foreach (var item in collection)
            {
                Return(item);
            }

            Dispose();
        }
    }
}