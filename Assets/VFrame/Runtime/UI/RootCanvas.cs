using System;
using VFrame.UI.Blackboard;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VFrame.Core;

namespace VFrame.UI
{
    [RequireComponent(typeof(Canvas))]
    public class RootCanvas : MonoBehaviour
    {
        // [SerializeField] private BlackboardAsset blackboardAsset;

        public void Configure(IContainerBuilder builder)
        {
            if (VFrameSettings.Instance.BlackboardAsset != null)
            {
                builder.RegisterInstance(VFrameSettings.Instance.BlackboardAsset);
                builder.RegisterEntryPoint<RootBlackboard>().AsSelf();
            }
            else
            {
                //todo: register safety
                throw new ArgumentNullException($"{nameof(BlackboardAsset)}");
            }

            UISystem.RootConfigure(gameObject.scene, builder);

            // Animation - View
            // builder.RegisterInstance(fadeView);
        }
    }
}