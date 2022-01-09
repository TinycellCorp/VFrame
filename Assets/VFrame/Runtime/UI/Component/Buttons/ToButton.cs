using VFrame.UI.View;
using UnityEngine;
using UnityEngine.UI;

namespace VFrame.UI.Component.Buttons
{
    [AddComponentMenu("Tiny/UI/Button/UISystem.To(View)")]
    [RequireComponent(typeof(Button))]
    public class ToButton : OnClickButton
    {
        [SerializeField] private ComponentView target;
        public ComponentView Target => target;

        protected override void OnClick()
        {
            if (ReferenceEquals(target, null)) return;

            UISystem.To(target);
        }
    }
}