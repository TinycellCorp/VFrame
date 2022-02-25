using System;
using Cysharp.Threading.Tasks;
using VFrame.UI.Context;
using VFrame.UI.Extension;
using VFrame.UI.View;

namespace VFrame.UI.Animation
{
    public readonly struct ViewAnimator
    {
        private readonly IView _view;
        private readonly IAnimation _animation;

        public ViewAnimator(IView view, IAnimation animation)
        {
            _view = view;
            _animation = animation;
        }

        public UniTask In() => _view.In(_animation);
        public UniTask Out() => _view.Out(_animation);
        
        public async UniTask<ScopeAnimator> Scope()
        {
            await In();
            return new ScopeAnimator(this);
        }

        public readonly struct ScopeAnimator : IUniTaskAsyncDisposable
        {
            private readonly ViewAnimator _animator;
            public ScopeAnimator(ViewAnimator animator) => _animator = animator;

            public UniTask DisposeAsync()
            {
                return _animator.Out();
            }
        }

    }
}