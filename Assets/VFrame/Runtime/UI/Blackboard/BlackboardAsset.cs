using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace VFrame.UI.Blackboard
{
    [CreateAssetMenu(menuName = "VFrame/Blackboard Asset")]
    public class BlackboardAsset : VariablesGroupAsset
    {
        public bool TryGetValueChanged(string key, out IVariableValueChanged valueChanged)
        {
            valueChanged = null;

            if (TryGetValue(key, out var value) && value is IVariableValueChanged changed)
            {
                valueChanged = changed;
            }

            return valueChanged != null;
        }
    }
}