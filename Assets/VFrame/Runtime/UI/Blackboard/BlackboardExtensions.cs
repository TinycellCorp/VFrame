using System;
using VFrame.UI.Blackboard.Reference;
using UnityEngine;

namespace VFrame.UI.Blackboard
{
    public static class BlackboardExtensions
    {
        public static void AddTo(this FloatReference reference,
            GameObject gameObject, Action<float> onChangedEvent)
        {
            var handler = gameObject.AddComponent<FloatReferenceHandler>();
            handler.Register(reference, onChangedEvent);
        }
    }
}