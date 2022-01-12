using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Audio
{
    [AddComponentMenu("Tiny/UI/Button/PlayAudio")]
    [RequireComponent(typeof(Button))]
    public class PlayAudioFromButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private AssetReferenceT<AudioClip> clip;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!clip.RuntimeKeyIsValid())
            {
#if UNITY_EDITOR
                UnityEditor.EditorGUIUtility.PingObject(gameObject);
#endif
                throw new ArgumentNullException(gameObject.name);
            }
        }
    }
}