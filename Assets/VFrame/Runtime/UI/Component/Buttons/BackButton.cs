using UnityEngine;
using UnityEngine.UI;

namespace VFrame.UI.Component.Buttons
{
    [AddComponentMenu("Tiny/UI/Button/UISystem.Back")]
    [RequireComponent(typeof(Button))]
    public class BackButton : OnClickButton
    {
        protected override void OnClick()
        {
            UISystem.Back();
        }
    }
}