using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace VFrame.Extension
{
    public static class AddressableExtensions
    {
        public static AddressableReference<TObject> ToReference<TObject>(this AsyncOperationHandle<TObject> handle)
        {
            return new AddressableReference<TObject>(handle);
        }

        public static AudioClipReference ToReference(this AsyncOperationHandle<AudioClip> handle)
        {
            return new AudioClipReference(handle);
        }
    }

    public class AddressableReference<TObject> : IDisposable
    {
        private readonly AsyncOperationHandle<TObject> _handle;

        private bool _isDisposed = false;

        public AddressableReference(AsyncOperationHandle<TObject> handle)
        {
            _handle = handle;
        }

        public bool TryGetAsset(out TObject asset)
        {
            ThrowDisposed();

            if (_handle.IsDone)
            {
                asset = _handle.Result;
                return true;
            }

            asset = default;
            return false;
        }

        private void ThrowDisposed()
        {
            if (_isDisposed) throw new ObjectDisposedException(nameof(AddressableReference<TObject>));
        }

        public void Dispose()
        {
            if(_isDisposed) return;
            
            if (TryGetAsset(out var asset))
            {
                OnPreRelease(asset);
            }

            _isDisposed = true;
            Addressables.Release(_handle);
        }

        protected virtual void OnPreRelease(TObject asset)
        {
        }

        public UniTask<TObject> AsUniTask()
        {
            return _handle.Task.AsUniTask();
        }
    }

    public class AudioClipReference : AddressableReference<AudioClip>
    {
        public AudioClipReference(AsyncOperationHandle<AudioClip> handle) : base(handle)
        {
        }

        protected override void OnPreRelease(AudioClip clip)
        {
            if (clip.loadState == AudioDataLoadState.Loaded)
            {
                clip.UnloadAudioData();
            }
        }
    }
}