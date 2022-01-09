using System;
using UnityEngine;

namespace VFrame.UI.Blackboard.Reference
{
    public class ReferenceHandlerBase<TReference, T> : MonoBehaviour where TReference : ReferenceBase<T>
    {
        private TReference _reference;
        private Action<T> _onChangedEvent;
        private bool _isAddedEvent = false;

        public void Register(TReference reference, Action<T> onChangedEvent)
        {
            if (_reference != null)
            {
                ClearEvent();
                _reference.Dispose();
                _reference = null;
            }

            if (reference == null || onChangedEvent == null) throw new ArgumentNullException();

            _reference = reference;
            _reference.RefreshHandler();

            _onChangedEvent = onChangedEvent;
            RegisterEvent();
        }

        private void OnEnable()
        {
            RegisterEvent();
        }

        private void OnDisable()
        {
            ClearEvent();
        }

        private void OnDestroy()
        {
            ClearEvent();
            _reference?.Dispose();
            _reference = null;
        }

        private void RegisterEvent()
        {
            if (!_isAddedEvent && _reference != null && _onChangedEvent != null)
            {
                _reference.ValueChanged += _onChangedEvent;
                _isAddedEvent = true;
            }
        }

        private void ClearEvent()
        {
            if (_reference != null && _onChangedEvent != null)
            {
                _reference.ValueChanged -= _onChangedEvent;
                _isAddedEvent = false;
            }
        }
    }
}