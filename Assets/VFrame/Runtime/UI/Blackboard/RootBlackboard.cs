using System;
using System.Collections.Generic;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using VContainer.Unity;

namespace VFrame.UI.Blackboard
{
    public class RootBlackboard : IInitializable
    {
        private static RootBlackboard _sharedInstance;
        private readonly BlackboardAsset _blackboard;
        private readonly Dictionary<string, IVariable> _memorizedValue = new Dictionary<string, IVariable>();
        private readonly string[] _singleItem = new string[1];

        private object this[string key]
        {
            set
            {
                if (_sharedInstance.TryGetValue(key, out var variable))
                {
                    switch (variable)
                    {
                        case BoolVariable boolean:
                            if (value is bool arg) boolean.Value = arg;
                            break;
                    }
                }
            }
        }

        public RootBlackboard(BlackboardAsset asset)
        {
            _sharedInstance = this;
            _blackboard = asset;
        }

        public void Initialize()
        {
        }

        public static void Overwrite(Dictionary<string, object> collection)
        {
            foreach (var pair in collection)
            {
                _sharedInstance[pair.Key] = pair.Value;
            }
        }

        public static void Overwrite<T>(string key, T value)
        {
            if (_sharedInstance.TryGetValue(key, out var variable))
            {
                if (variable is Variable<T> accessor)
                {
                    accessor.Value = value;
                }
                else
                {
                    throw new InvalidCastException($"{typeof(T).Name} to {variable.GetType().Name} not supported");
                }
            }
            else
            {
                throw new KeyNotFoundException(key);
            }
        }

        private bool TryGetValue(string key, out IVariable value)
        {
            if (_memorizedValue.TryGetValue(key, out value))
            {
                return true;
            }

            string[] items = null;
            if (!key.Contains("."))
            {
                _singleItem[0] = key;
                items = _singleItem;
            }
            else
            {
                items = key.Split('.');
            }

            value = GetValue(_blackboard, items);
            if (value != null)
            {
                _memorizedValue[key] = value;
            }

            return value != null;
        }

        private IVariable GetValue(IVariableGroup group, string[] items, int index = 0)
        {
            if (index >= items.Length)
            {
                throw new Exception("not found value");
            }

            if (group.TryGetValue(items[index], out var value))
            {
                if (value is IVariableGroup nesting)
                {
                    return GetValue(nesting, items, index + 1);
                }
                else
                {
                    return value;
                }
            }

            return null;
        }

        public static bool TryGetValueChanged(string key, out IVariableValueChanged valueChanged)
        {
            valueChanged = null;

            if (_sharedInstance.TryGetValue(key, out var value) && value is IVariableValueChanged changed)
            {
                valueChanged = changed;
                return true;
            }

            return false;
        }
    }
}