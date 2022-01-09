using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

namespace VFrame.Scene
{
    public static class AssetCacheExtension
    {
        public static async UniTask LoadPrefabAsync(this AssetCache cache, string cacheKey, string key)
        {
            var prefab = await Addressables.LoadAssetAsync<GameObject>(key).Task.AsUniTask();
            cache.RegisterPrefab(cacheKey, prefab);
        }

        public static async UniTask LoadSpriteAsync(this AssetCache cache, string cacheKey, string key)
        {
            var sprite = await Addressables.LoadAssetAsync<Sprite>(key).Task.AsUniTask();
            cache.RegisterSprite(cacheKey, sprite);
        }

        public static async UniTask LoadSpriteAsync(this AssetCache cache, string cacheKey, string atlasKey,
            string spriteName)
        {
            var sprite = await Addressables.LoadAssetAsync<Sprite>($"{atlasKey}[{spriteName}]").Task.AsUniTask();
            cache.RegisterSprite(cacheKey, sprite);
        }


        public static async UniTask LoadAtlasAsync(this AssetCache cache, string cacheKey, string key)
        {
            var atlas = await Addressables.LoadAssetAsync<SpriteAtlas>(key);
            cache.RegisterAtlas(cacheKey, atlas);
        }


        private static UniTask<TObject>.Awaiter GetAwaiter<TObject>(this AsyncOperationHandle<TObject> handle)
        {
            
            return handle.Task.AsUniTask().GetAwaiter();
        }
    }
}