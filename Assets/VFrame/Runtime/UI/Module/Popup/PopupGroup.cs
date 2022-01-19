using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using VFrame.UI.Context;
using VFrame.UI.View;
using UnityEngine;

namespace VFrame.UI.Module.Popup
{
    public class PopupGroup<TShadow> : IPopupGroup where TShadow : class, IView
    {
        private readonly Stack<IView> _views = new Stack<IView>();
        private TShadow _shadow;
        public async UniTask Push(ISystemContext context, IView view)
        {
            if (ReferenceEquals(context.View.Peek(), view))
            {
                context.View.ImmediatePop();
            }

            if (_views.Contains(view)) return;

            var shadow = context.ResolveView<TShadow>();
            if (!shadow.IsActive && _views.Count == 0)
            {
                if (shadow.Rect.parent != view.Rect.parent)
                {
                    shadow.Rect.SetParent(view.Rect.parent);
                }

                shadow.Rect.anchoredPosition = Vector2.zero;
                context.ResolveAnimator(shadow).In();
                // context.Command.Push(shadow);
            }

            shadow.Rect.SetAsLastSibling();
            view.Rect.SetAsLastSibling();

            if (context.View.TryPopManipulator(view, out var manipulator))
            {
                await context.Command.Push(view, manipulator);
            }
            else
            {
                await context.Command.Push(view);
            }

            _views.Push(view);
        }

        public async UniTask Pop(ISystemContext context)
        {
            if (_views.Any()) _views.Pop();

            if (_views.Count == 0)
            {
                // context.Command.Pop();
                context.ResolveAnimator<TShadow>().Out();
                await context.Command.Pop();
            }
            else
            {
                var shadow = context.ResolveView<TShadow>();
                shadow.Rect.SetSiblingIndex(shadow.Rect.GetSiblingIndex() - 1);
                await context.Command.Pop();
            }
        }

        public void OnImmediatePop(IView view)
        {
            _views.Pop();
        }
    }
}