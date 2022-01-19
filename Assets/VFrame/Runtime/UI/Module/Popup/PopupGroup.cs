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
        // private TShadow _shadow;
        private Transform _shadowParent;

        public async UniTask Push(ISystemContext context, IView view)
        {
            if (ReferenceEquals(context.View.Peek(), view))
            {
                context.View.ImmediatePop();
            }

            if (_views.Contains(view)) return;

            var shadow = context.ResolveView<TShadow>();
            if (_shadowParent == null)
            {
                _shadowParent = shadow.Rect.parent;
            }

            RepositionShadow(shadow, view);
            
            if (!shadow.IsActive && _views.Count == 0)
            {
                context.ResolveAnimator(shadow).In();
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

        private void RepositionShadow(TShadow shadow, IView view)
        {
            if (shadow.Rect.parent != view.Rect.parent)
            {
                shadow.Rect.SetParent(view.Rect.parent);
                shadow.Rect.anchoredPosition = Vector2.zero;
                shadow.Rect.localScale = Vector3.one;
            }
        }

        private void ReturnShadow(TShadow shadow)
        {
            if (shadow.Rect.parent != _shadowParent)
            {
                shadow.Rect.SetParent(_shadowParent);
                shadow.Rect.anchoredPosition = Vector2.zero;
                shadow.Rect.localScale = Vector3.one;
            }
        }

        public async UniTask Pop(ISystemContext context)
        {
            if (_views.Any()) _views.Pop();

            if (_views.Count == 0)
            {
                var shadow = context.Resolve<TShadow>();
                ReturnShadow(shadow);

                context.ResolveAnimator(shadow).Out();
                await context.Command.Pop();
            }
            else
            {
                var shadow = context.ResolveView<TShadow>();
                var nextView = _views.Peek();
                RepositionShadow(shadow, nextView);
                // shadow.Rect.SetSiblingIndex(nextView.Rect.GetSiblingIndex() - 1);
                shadow.Rect.SetAsLastSibling();
                nextView.Rect.SetAsLastSibling();

                
                await context.Command.Pop();
            }
        }

        public void OnImmediatePop(IView view)
        {
            _views.Pop();
            //TODO: ReturnShadow
        }
    }
}