using Cysharp.Threading.Tasks;
using UnityEngine;
using VFrame.UI.View;

namespace VFrame.UI.External
{
    public class FadeView : ComponentView<FadeView>
    {
        public override UniTask Ready()
        {
            Rect.anchoredPosition = Vector2.zero;
            return UniTask.CompletedTask;
        }

        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
        }
    }
}