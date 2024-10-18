using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using VFrame.Extension;
using VFrame.UI.Command;
using VFrame.UI.Command.Addressable;
using VFrame.UI.Context;
using VFrame.UI.View;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace VFrame.UI
{
    public partial class UISystem : IAddressableContext
    {
        private readonly Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();
        private readonly Dictionary<string, ComponentView> _loadedViews = new Dictionary<string, ComponentView>();
        private readonly HashSet<string> _loadingKeys = new HashSet<string>();

        public IAddressableContext Addressable => this;

        public static void To(string key)
        {
            _sharedInstance.ToAddressableView(key);
        }

        public static void To(string key, IManipulator manipulator)
        {
            _sharedInstance.ToAddressableView(key, manipulator);
        }

        private void ThrowContainsLoadingView(in string key)
        {
            if (_loadingKeys.Contains(key))
            {
                throw new Exception("contains loading view");
            }
        }

        private void ToAddressableView(string key, IManipulator manipulator = null)
        {
            if (IsBlocking) return;

            if (_loadingKeys.Contains(key))
            {
                // throw new Exception("contains loading view");
                return;
            }

            if (_loadedViews.TryGetValue(key, out var view))
            {
                if (view == null)
                {
                    _loadedViews.Remove(key);
                }
                else
                {
                    if (manipulator == null) To(view);
                    else To(view, manipulator);
                    return;
                }
            }

            _loadingKeys.Add(key);
            if (manipulator == null) EnqueueCommand(new LoadViewCommand(key));
            else EnqueueCommand(new LoadViewWithManipulatorCommand(key, manipulator));
        }

        async UniTask<ComponentView> IAddressableContext.LoadView(string key)
        {
            if (!_prefabs.ContainsKey(key))
            {
                var prefab = await Addressables.LoadAssetAsync<GameObject>(key).Task.AsUniTask();
                _prefabs.Add(key, prefab);
            }

            if (!_loadedViews.ContainsKey(key))
            {
                var prefab = _prefabs[key];

                ComponentView view;
                using (EnableBlockingRegisterView())
                using (prefab.InstantiateBefore(out view))
                {
                    _container.Inject(view);
                    _loadedViews.Add(key, view);
                }

                view.Rect.localScale = Vector3.one;
                view.IsActive = false;
                _loadingKeys.Remove(key);
                
                var scene = SceneManager.GetActiveScene();
                if (SceneViews.TryGetValue(scene, out var views))
                {
                    views.Enqueue(view);
                }
                else
                {
                    views = new Queue<ComponentView>();
                    views.Enqueue(view);
                    SceneViews.Add(scene, views);
                }
            }

            return _loadedViews[key];
        }
    }
}