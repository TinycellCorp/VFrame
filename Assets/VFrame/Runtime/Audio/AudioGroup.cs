using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace Audio
{
    [CreateAssetMenu(fileName = "New Audio Group", menuName = "VFrame/Audio Group")]
    public class AudioGroup : ScriptableObject
    {
        [SerializeField] private AudioMixerGroup mixerGroup;
        [SerializeField] private AudioClipData[] clips = Array.Empty<AudioClipData>();

        public AudioMixerGroup MixerGroup => mixerGroup;
        public AudioClipData[] Clips => clips;
    }

    [Serializable]
    public class AudioClipData
    {
        [SerializeField] private string key;
        [SerializeField] private AssetReferenceT<AudioClip> asset;

        public string Key => key;
        public AssetReferenceT<AudioClip> Asset => asset;
    }
}