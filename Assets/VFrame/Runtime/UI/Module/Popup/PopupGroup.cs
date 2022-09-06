using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using VFrame.UI.Context;
using VFrame.UI.View;
using UnityEngine;
using VFrame.UI.Extension;

namespace VFrame.UI.Module.Popup
{
    public class PopupGroup<TShadow> : IPopupGroup where TShadow : class, IView
    {
        private readonly Stack<IView> _views = new Stack<IView>();
        private TShadow _shadow;
        private Transform _shadowParent;

        public async UniTask Push(ISystemContext context, IView view)
        {
            if (ReferenceEquals(context.View.Peek(), view))
            {
                context.View.ImmediatePop();
            }

            if (_views.Contains(view)) return;

            _shadow ??= context.ResolveView<TShadow>();
            _shadowParent ??= _shadow.Rect.parent;

            RepositionShadow(_shadow, view);

            var tasks = (UniTask.CompletedTask, UniTask.CompletedTask);
            if (!_shadow.IsActive && _views.Count == 0)
            {
                tasks.Item1 = context.ResolveAnimator(_shadow).In();
            }

            if (context.View.TryPopManipulator(view, out var manipulator))
            {
                tasks.Item2 = context.Command.Push(view, manipulator);
            }
            else
            {
                tasks.Item2 = context.Command.Push(view);
            }

            view.IsInteractable = false;
            await tasks;
            view.IsInteractable = true;

            _views.Push(view);
        }

        public async UniTask Pop(ISystemContext context)
        {
            if (_views.Any()) _views.Pop();

            if (_views.Count == 0)
            {
                ReturnShadow(_shadow);

                await (
                    context.ResolveAnimator(_shadow).Out(),
                    context.Command.Pop()
                );
            }
            else
            {
                var nextView = _views.Peek();
                RepositionShadow(_shadow, nextView);
                await context.Command.Pop();
            }
        }

        public void OnImmediatePop(IView view)
        {
            _views.Pop();
            if (_views.Any())
            {
                var nextView = _views.Peek();
                RepositionShadow(_shadow, nextView);
            }
            else
            {
                ReturnShadow(_shadow);
                _shadow.Hide();
            }
        }

        private void RepositionShadow(TShadow shadow, IView view)
        {
            if (shadow.Rect.parent != view.Rect.parent)
            {
                shadow.Rect.SetParent(view.Rect.parent);
                shadow.Rect.anchoredPosition = Vector2.zero;
                shadow.Rect.localScale = Vector3.one;
            }

            shadow.Rect.SetAsLastSibling();
            view.Rect.SetAsLastSibling();
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
    }
}