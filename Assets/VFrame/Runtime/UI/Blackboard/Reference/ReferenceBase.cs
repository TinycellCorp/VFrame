using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace VFrame.UI.Blackboard.Reference
{
    [Serializable]
    public abstract class ReferenceBase<T> : IDisposable
    {
        [SerializeField] private string key;

        public string Key
        {
            get => key;
            set
            {
                if (value != key)
                {
                    ClearHandler();
                    _usedValue = null;
                }

                key = value;
            }
        }

        private IVariableValueChanged _usedValue;
        private Action<T> _changedHandler;

        public event Action<T> ValueChanged
        {
            add
            {
                if (value == null)
                    throw new ArgumentNullException();

                bool wasEmpty = _changedHandler == null;
                _changedHandler += value;

                if (wasEmpty)
                {
                    OnValueChanged(_usedValue);
                }
            }
            remove
            {
                if (value == null) throw new ArgumentNullException();

                _changedHandler -= value;

                if (_changedHandler == null)
                {
                    // empty
                }
            }
        }

        private readonly BlackboardAsset _asset;

        public ReferenceBase()
        {
            _asset = null;
            key = null;
        }

        public ReferenceBase(BlackboardAsset asset, string key)
        {
            _asset = asset;
            this.key = key;
        }


        public void RefreshHandler()
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException();
            }

            ClearHandler();

            if (_asset != null && _asset.TryGetValueChanged(key, out _usedValue))
            {
                RegisterHandler();
            }
            else if (RootBlackboard.TryGetValueChanged(key, out _usedValue))
            {
                RegisterHandler();
            }
            else
            {
                _usedValue = null;
                throw new KeyNotFoundException(key);
            }
        }

        private void OnValueChanged(IVariable variable)
        {
            if (variable == null) return;

            var value = variable.GetSourceValue(null);
            if (value is T arg)
            {
                _changedHandler?.Invoke(arg);
            }
            // switch (value)
            // {
            //     case bool boolValue:
            //         _changedHandler.Invoke(boolValue);
            //         break;
            // }
        }

        private void RegisterHandler()
        {
            if (_usedValue != null) _usedValue.ValueChanged += OnValueChanged;
        }

        private void ClearHandler()
        {
            if (_usedValue != null) _usedValue.ValueChanged -= OnValueChanged;
        }

        public void Dispose()
        {
            ClearHandler();
            _usedValue = null;
        }
    }
}