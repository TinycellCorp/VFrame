using VFrame.UI.Blackboard;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace VFrame.UI
{
    [RequireComponent(typeof(Canvas))]
    public class RootCanvas : MonoBehaviour
    {
        [SerializeField] private BlackboardAsset blackboardAsset;
 
        public void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<RootBlackboard>().AsSelf();
            builder.RegisterInstance(blackboardAsset);

            UISystem.RootConfigure(gameObject.scene, builder);

            // Animation - View
            // builder.RegisterInstance(fadeView);
        }
    }
}