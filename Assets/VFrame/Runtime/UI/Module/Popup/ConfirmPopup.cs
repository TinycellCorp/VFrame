using Cysharp.Threading.Tasks;
using VFrame.UI.Command;
using VFrame.UI.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VFrame.UI.Module.Popup
{
    public abstract class ConfirmPopup<TView> : PopupView<TView> where TView : IView
    {
        [SerializeField] protected TextMeshProUGUI contentText;

        [SerializeField] protected Button confirmButton;
        [SerializeField] protected TextMeshProUGUI confirmText;

        public override void OnEnter()
        {
            if (confirmButton.onClick.GetPersistentEventCount() == 0)
            {
                confirmButton.onClick.AddListener(() => { UISystem.Back(); });
            }
        }

        public override void OnExit()
        {
            confirmButton.onClick.RemoveAllListeners();
        }

        public abstract class ConfirmManipulator : IManipulator
        {
            private readonly string _content;
            private readonly string _confirm;

            protected ConfirmManipulator(string content, string confirm)
                => (_content, _confirm) = (content, confirm);

            public async UniTask Ready(IView view)
            {
                if (view is ConfirmPopup<TView> target)
                {
                    view.IsActive = false;
                    //Content
                    target.contentText.text = _content;

                    //Confirm
                    target.confirmButton.onClick.AddListener(Confirm);
                    target.confirmText.text = _confirm;

                    await PostReady(target);

                    view.IsActive = true;
                }
            }

            protected abstract UniTask PostReady(ConfirmPopup<TView> view);
            protected abstract void Confirm();
        }

    }
}