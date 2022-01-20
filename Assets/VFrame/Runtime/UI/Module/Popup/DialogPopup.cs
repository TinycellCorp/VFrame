using Cysharp.Threading.Tasks;
using VFrame.UI.Command;
using VFrame.UI.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VFrame.UI.Module.Popup
{
    public interface IDialogManipulator : IManipulator
    {
        void Positive();
        void Negative();
    }


    public abstract class DialogPopup<TView> : PopupView<TView> where TView : IView
    {
        [SerializeField] protected TextMeshProUGUI contentText;
        [SerializeField] protected Button positiveButton;
        [SerializeField] protected TextMeshProUGUI positiveText;
        [SerializeField] protected Button negativeButton;
        [SerializeField] protected TextMeshProUGUI negativeText;

        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
            positiveButton.onClick.RemoveAllListeners();
            negativeButton.onClick.RemoveAllListeners();
        }


        public abstract class DialogManipulator : IDialogManipulator
        {
            private readonly string _content;
            private readonly string _positive;
            private readonly string _negative;

            protected DialogManipulator(string content, string positive, string negative)
                => (_content, _positive, _negative) = (content, positive, negative);

            public UniTask Ready(IView view)
            {
                if (view is DialogPopup<TView> target)
                {
                    target.contentText.text = _content;

                    target.positiveButton.onClick.AddListener(Positive);
                    target.positiveText.text = _positive;

                    target.negativeButton.onClick.AddListener(Negative);
                    target.negativeText.text = _negative;

                    return PostReady(target);
                }

                return UniTask.CompletedTask;
            }

            protected abstract UniTask PostReady(DialogPopup<TView> view);
            public abstract void Positive();
            public abstract void Negative();
        }
    }
}