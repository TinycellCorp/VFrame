using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VFrame.UI.Component.Events
{
    [AddComponentMenu("Tiny/UI/Button/Pressed Event")]
    [RequireComponent(typeof(Button))]
    public class PressedEvent : PressedEventBase
    {
        [SerializeField] protected float fromSeconds = 0.2f;
        [SerializeField] protected float intervalSeconds = 0.05f;

        protected override float FromSeconds => fromSeconds;
        protected override float IntervalSeconds => intervalSeconds;

    }
}