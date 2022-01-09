using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using VFrame.UI.Animation;
using VFrame.UI.Command;
using VFrame.UI.View;

namespace VFrame.UI.Module.Toast
{
    public interface IToastView : IView
    {
    }

    public abstract class ToastAnimationBase : CancelableAnimation, IAnimation
    {
        protected sealed override async UniTask In(IView view, CancellationToken token)
        {
            await Play(view, token);
        }

        protected UniTask Delay(IView view, CancellationToken token)
        {
            var seconds = 0.6f;
            if (view is FromSecondToastView toastView && toastView.Seconds > 0) seconds = toastView.Seconds;
            return UniTask.Delay(TimeSpan.FromSeconds(seconds), cancellationToken: token);
        }

        protected sealed override UniTask Out(IView view, CancellationToken token) => UniTask.CompletedTask;

        protected abstract UniTask Play(IView view, CancellationToken token);
    }

    public abstract class ToastView<TView> : ComponentView<TView>, IToastView where TView : IView
    {
        public float Seconds { get; protected set; }

        // public abstract class AnimationBase : CancelableAnimation, IAnimation<TView>
        // {
        // }

        public abstract class ManipulatorBase : IManipulator
        {
            private readonly float _seconds;

            protected ManipulatorBase(float seconds = 1f)
            {
                _seconds = seconds;
            }

            public UniTask Ready(IView view)
            {
                if (view is ToastView<TView> toastView)
                {
                    toastView.Seconds = _seconds;
                    return PostReady(toastView);
                }

                return UniTask.CompletedTask;
            }

            protected abstract UniTask PostReady(ToastView<TView> view);
        }

        protected override void Awake()
        {
            UISystem.RegisterGroupWithAnimation<TView, ToastGroup, IAnimation<IToastView>>(this);
            Init();
        }
    }
}