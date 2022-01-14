using UnityEngine;
using UnityEngine.UI;

namespace Audio
{
    [AddComponentMenu("Tiny/UI/Button/PlayAudio.Key")]
    [RequireComponent(typeof(Button))]
    public class PlayAudioFromKey : PlayAudioFromButton
    {
        [SerializeField] private string key;
        protected override string Key => key;
    }
}