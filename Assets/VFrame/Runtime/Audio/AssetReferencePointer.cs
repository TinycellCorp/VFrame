using UnityEngine;
using UnityEngine.AddressableAssets;

namespace VFrame.Audio
{
    public class AssetReferencePointer
    {
        public AudioGroup Group { get; }

        public AssetReferenceT<AudioClip> AssetReference => Group.Clips[_index].Asset;
        private readonly int _index;

        public AssetReferencePointer(AudioGroup audioGroup, int index)
        {
            Group = audioGroup;
            _index = index;
        }
    }
}