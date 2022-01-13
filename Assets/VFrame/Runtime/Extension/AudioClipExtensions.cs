using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VFrame.Extension
{
    public static class AudioClipExtensions
    {
        public static async UniTask LoadAudioDataAsync(this AudioClip clip)
        {
            if (clip.loadState == AudioDataLoadState.Loaded) return;
            if (clip.loadState != AudioDataLoadState.Loading)
            {
                clip.LoadAudioData();
            }
            await UniTask.WaitUntil(() => clip.loadState != AudioDataLoadState.Loading);
        }
    }
}