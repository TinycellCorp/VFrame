using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VFrame.UI.View
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