using System;
using Audio;
using Cysharp.Threading.Tasks;
using VFrame.Scene;

namespace VFrame.Extension
{
    public static class SceneEntryExtensions
    {
        public static async UniTask UseAudioClip(this SceneEntry scene, string key)
        {
            await AudioSystem.Use(key);
            new AudioClipDisposableHandler(key).AddTo(scene.GetCancellationTokenOnDestroy());
        }

        private class AudioClipDisposableHandler : IDisposable
        {
            private readonly string _key;

            public AudioClipDisposableHandler(string key)
            {
                _key = key;
            }

            public void Dispose()
            {
                AudioSystem.DisposeAudioClip(_key);
            }
        }
    }
}