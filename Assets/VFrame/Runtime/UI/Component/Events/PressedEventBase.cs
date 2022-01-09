using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace VFrame.UI.Component.Events
{
    public abstract class PressedEventBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        protected abstract float FromSeconds { get; }
        protected abstract float IntervalSeconds { get; }

        [SerializeField] private UnityEvent onPressed = new UnityEvent();

        private float _currentTime = 0;
        private float _waitTime = 0;
        private bool _isPressed = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPressed = true;
            _currentTime = 0;
            _waitTime = FromSeconds;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isPressed = false;
        }

        private void Update()
        {
            if (!_isPressed) return;

            _currentTime += Time.deltaTime;
            if (_currentTime > _waitTime)
            {
                _currentTime = 0;
                _waitTime = IntervalSeconds;
                onPressed.Invoke();
            }
        }
    }
}