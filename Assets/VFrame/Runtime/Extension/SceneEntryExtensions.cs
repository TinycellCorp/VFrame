using System;
using Cysharp.Threading.Tasks;
using VFrame.Audio;
using VFrame.Scene;

namespace VFrame.Extension
{
    public static class SceneEntryExtensions
    {
        public static async UniTask ReadyAudioClip(this SceneEntry scene, string key)
        {
            await AudioSystem.Ready(key);
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
                AudioSystem.Release(_key);
            }
        }
    }
}