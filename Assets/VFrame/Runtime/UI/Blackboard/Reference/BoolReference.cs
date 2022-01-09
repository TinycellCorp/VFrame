using System;
using UnityEngine;

namespace VFrame.UI.Blackboard.Reference
{
    [Serializable]
    public class BoolReference : ReferenceBase<bool>
    {
        public BoolReference(BlackboardAsset asset, string key) : base(asset, key)
        {
        }
    }
}