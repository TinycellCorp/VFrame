using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VFrame.Audio
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