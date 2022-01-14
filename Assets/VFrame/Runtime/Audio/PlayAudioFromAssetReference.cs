using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace VFrame.Audio
{
    [AddComponentMenu("Tiny/UI/Button/PlayAudio.AssetReference")]
    [RequireComponent(typeof(Button))]
    public class PlayAudioFromAssetReference : PlayAudioFromButton
    {
        [SerializeField] private AssetReferenceT<AudioClip> asset;

        private string _key;

        protected override string Key
        {
            get
            {
                if (string.IsNullOrEmpty(_key))
                {
                    _key = AudioSystem.AssetReferenceToKey(asset);
                }

                return _key;
            }
        }
    }
}