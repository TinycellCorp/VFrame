using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;

namespace Audio
{
    [CreateAssetMenu(fileName = "New Audio Group", menuName = "VFrame/Audio Group")]
    public class AudioGroup : ScriptableObject
    {
        [SerializeField] private AudioMixerGroup group;
        [SerializeField] private AssetReferenceT<AudioClip>[] clips = Array.Empty<AssetReferenceT<AudioClip>>();

        public AssetReferenceT<AudioClip>[] Clips => clips;
    }
}