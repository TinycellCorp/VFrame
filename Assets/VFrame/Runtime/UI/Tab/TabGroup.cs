using System;
using Cysharp.Threading.Tasks;
using VFrame.UI.Context;
using VFrame.UI.Extension;
using VFrame.UI.Group;
using VFrame.UI.View;

namespace VFrame.UI.Tab
{
    public class TabGroup<TParent> : IGroup where TParent : class, IView
    {
        private IView _focusView;
        private readonly UniTask[] _buffer = new UniTask[2];

        public IView FocusView => _focusView;

        public IView InView { get; private set; }
        public IView OutView { get; private set; }

        public async UniTask Push(ISystemContext context, IView view)
        {
            if (_focusView == view) return;

            var parent = context.ResolveView<TParent>();
            if (!context.View.Contains(parent))
            {
                _focusView = view;
                await _focusView.Show();
                await context.Command.Push(parent);
            }
            else
            {
                OutView = _focusView;
                InView = view;

                await InView.Ready();
                InView.IsActive = true;

                var outAnimator = context.ResolveAnimator(OutView);
                var inAnimator = context.ResolveAnimator(InView);

                _buffer[0] = outAnimator.Out();
                _buffer[1] = inAnimator.In();
                await UniTask.WhenAll(_buffer);

                OutView.Hide();
                InView.OnEnter();

                _focusView = InView;
            }

            InView = null;
            OutView = null;
        }

        public async UniTask Pop(ISystemContext context)
        {
            _focusView?.Hide();
            _focusView = null;
            await context.Command.Pop();
        }

        public void OnImmediatePop(IView view)
        {
            if (_focusView != null)
            {
                _focusView.IsActive = false;
                _focusView = null;
            }
        }
    }
}