using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;
using VContainer;
using Object = UnityEngine.Object;

namespace VFrame.Scene
{
    public partial class AssetCache : IDisposable
    {
        public static readonly AssetCache Empty = new AssetCache(null);

        private readonly Dictionary<string, GameObject> _loadedPrefab = new Dictionary<string, GameObject>();
        private readonly Dictionary<string, Sprite> _loadedSprite = new Dictionary<string, Sprite>();

        private readonly IObjectResolver _resolver;

        public AssetCache(IObjectResolver resolver) =>
            (_resolver) =
            (resolver);

        public void Dispose()
        {
            ClearPrefab();
            ClearSprites();

            ClearAtlas();
            Resources.UnloadUnusedAssets();
        }
    }

    /// <summary>
    /// Prefab Section
    /// </summary>
    public partial class AssetCache
    {
        public string[] GetLoadedPrefabKeys()
        {
            return _loadedPrefab.Keys.ToArray();
        }

        public void RegisterPrefab(string key, GameObject prefab)
        {
            _loadedPrefab.Add(key, prefab);
        }

        public GameObject Instantiate(string key)
        {
            if (_loadedPrefab.TryGetValue(key, out var prefab))
            {
                return Object.Instantiate(prefab);
            }
            else
            {
                throw new KeyNotFoundException($"not found key: {key}");
            }
        }

        public GameObject GetPrefab(string key)
        {
            if (_loadedPrefab.TryGetValue(key, out var prefab))
            {
                return prefab;
            }
            else
            {
                throw new KeyNotFoundException($"not found key: {key}");

            }
        }

        private void ClearPrefab()
        {
            foreach (var prefab in _loadedPrefab.Values)
                Addressables.Release(prefab);
            _loadedPrefab.Clear();
        }
    }

    /// <summary>
    /// Sprite Section
    /// </summary>
    public partial class AssetCache
    {
        public void RegisterSprite(string key, Sprite sprite)
        {
            _loadedSprite.Add(key, sprite);
        }

        public void ApplySprite(string key, SpriteRenderer renderer)
        {
            if (_loadedSprite.TryGetValue(key, out var sprite))
            {
                renderer.sprite = sprite;
            }
            else
            {
                throw new KeyNotFoundException($"not found key: {key}");

            }
        }

        private void ClearSprites()
        {
            foreach (var sprite in _loadedSprite.Values)
                Addressables.Release(sprite);

            _loadedSprite.Clear();
        }
    }

    /// <summary>
    /// Atlas Section
    /// </summary>
    public partial class AssetCache
    {
        private readonly Dictionary<string, SpriteAtlas> _loadedAtlas = new Dictionary<string, SpriteAtlas>();

        private readonly Dictionary<(string atlas, string sprite), Sprite> _fromAtlasSprites =
            new Dictionary<(string atlas, string sprite), Sprite>();

        public void RegisterAtlas(string key, SpriteAtlas atlas)
        {
            _loadedAtlas.Add(key, atlas);
        }

        public void ApplySprite(string atlasKey, string spriteName, SpriteRenderer renderer)
        {
            var key = (atlasKey, spriteName);
            if (_fromAtlasSprites.TryGetValue(key, out var loaded))
            {
                renderer.sprite = loaded;
            }
            else if (_loadedAtlas.TryGetValue(atlasKey, out var atlas))
            {
                var sprite = atlas.GetSprite(spriteName);
                if (sprite == null) throw new KeyNotFoundException($"not found sprite: {atlasKey}[{spriteName}]");

                _fromAtlasSprites.Add(key, sprite);
                renderer.sprite = sprite;
            }
            else
            {
                throw new KeyNotFoundException($"not found atlas: {atlasKey}");
            }
        }

        private void ClearAtlas()
        {
            foreach (var sprite in _fromAtlasSprites.Values)
            {
                Object.Destroy(sprite);
            }

            _fromAtlasSprites.Clear();

            foreach (var atlas in _loadedAtlas.Values)
            {
                Addressables.Release(atlas);
            }

            _loadedAtlas.Clear();
        }
    }
}