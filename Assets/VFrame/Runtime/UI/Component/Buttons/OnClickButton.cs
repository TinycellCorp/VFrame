using UnityEngine;
using UnityEngine.UI;

namespace VFrame.UI.Component.Buttons
{
    [RequireComponent(typeof(Button))]
    public abstract class OnClickButton : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            GetComponent<Button>().onClick.RemoveAllListeners();
        }

        protected abstract void OnClick();
    }
}