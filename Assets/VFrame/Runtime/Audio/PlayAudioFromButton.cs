using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Audio
{
    public abstract class PlayAudioFromButton : MonoBehaviour, IPointerDownHandler
    {
        protected abstract string Key { get; }

        public void OnPointerDown(PointerEventData eventData)
        {
            AudioSystem.Play(Key);
        }
    }
}