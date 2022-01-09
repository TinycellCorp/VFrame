using Cysharp.Threading.Tasks;
using VFrame.UI.Extension;
using TMPro;
using UnityEngine;

namespace VFrame.UI.Module.Toast
{
    public abstract class FromSecondToastView : ToastView<FromSecondToastView>
    {
        [SerializeField] private TextMeshProUGUI content;

        public override UniTask Ready()
        {
            content.text = string.Empty;
            return UniTask.CompletedTask;
        }

        public class Manipulator : ManipulatorBase
        {
            private readonly string _content;

            public Manipulator(string content, float seconds = 1f) : base(seconds)
            {
                _content = content;
            }

            protected override UniTask PostReady(ToastView<FromSecondToastView> view)
            {
                view.As().content.text = _content;
                return UniTask.CompletedTask;
            }
        }
    }
}