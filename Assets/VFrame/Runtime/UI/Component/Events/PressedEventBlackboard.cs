using VFrame.UI.Blackboard;
using VFrame.UI.Blackboard.Reference;
using UnityEngine;

namespace VFrame.UI.Component.Events
{
    [AddComponentMenu("Tiny/UI/Button/Pressed Event - Blackboard")]
    public class PressedEventBlackboard : PressedEventBase
    {
        [SerializeField] private BlackboardAsset asset;
        [SerializeField] private string fromSecondsKey;
        [SerializeField] private string intervalSecondsKey;

        public string FromSecondsKey
        {
            set => fromSecondsKey = value;
        }

        public string IntervalSecondsKey
        {
            set => intervalSecondsKey = value;
        }

        private float _fromSeconds;
        private float _intervalSeconds;
        protected override float FromSeconds => _fromSeconds;
        protected override float IntervalSeconds => _intervalSeconds;

        private void Awake()
        {
            if (asset != null)
            {
                new FloatReference(asset, fromSecondsKey).AddTo(gameObject, OnChangedFromSeconds);
                new FloatReference(asset, intervalSecondsKey).AddTo(gameObject, OnChangedIntervalSeconds);
            }
        }

        private void OnChangedFromSeconds(float seconds) => _fromSeconds = seconds;
        private void OnChangedIntervalSeconds(float seconds) => _intervalSeconds = seconds;
    }
}