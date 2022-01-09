using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VFrame.Extension
{
    public static class PrefabExtensions
    {
        public static IDisposable InstantiateBefore<T>(this T prefab, out T component, Transform parent = null)
            where T : Component
        {
            return new BeforeHandlerByComponent<T>(prefab, out component, parent);
        }

        private class BeforeHandlerByComponent<T> : IDisposable where T : Component
        {
            private readonly bool _wasActive = false;
            private readonly T _prefab;
            private readonly T _instance;

            public BeforeHandlerByComponent(T prefab, out T component, Transform parent)
            {
                _prefab = prefab;
                _wasActive = _prefab.gameObject.activeSelf;
                if (_wasActive)
                {
                    _prefab.gameObject.SetActive(false);
                }

                _instance = Object.Instantiate(_prefab, parent);
                component = _instance;
            }

            public void Dispose()
            {
                if (_wasActive)
                {
                    _prefab.gameObject.SetActive(true);
                    _instance.gameObject.SetActive(true);
                }
            }
        }

        public static IDisposable InstantiateBefore<T>(this GameObject prefab, out T component, Transform parent = null)
            where T : Component
        {
            return new BeforeHandlerByGameObject<T>(prefab, out component, parent);
        }

        private class BeforeHandlerByGameObject<T> : IDisposable where T : Component
        {
            private readonly bool _wasActive = false;
            private readonly GameObject _prefab;
            private readonly GameObject _instance;

            public BeforeHandlerByGameObject(GameObject prefab, out T component, Transform parent)
            {
                _prefab = prefab;
                _wasActive = _prefab.activeSelf;
                if (_wasActive)
                {
                    _prefab.SetActive(false);
                }

                _instance = Object.Instantiate(_prefab, parent);
                component = _instance.GetComponent<T>();
            }

            public void Dispose()
            {
                if (_wasActive)
                {
                    _prefab.SetActive(true);
                    _instance.SetActive(true);
                }
            }
        }
    }
}