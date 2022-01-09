using System;

namespace VFrame.UI.Blackboard.Reference
{
    [Serializable]
    public class FloatReference : ReferenceBase<float>
    {
        public FloatReference(BlackboardAsset asset, string key) : base(asset, key)
        {
        }
    }
}