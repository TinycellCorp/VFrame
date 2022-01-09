using System.Threading;
using Cysharp.Threading.Tasks;
using VFrame.UI.Animation;
using VFrame.UI.View;

namespace VFrame.UI.Animation
{
    public interface ICancelableAnimation : IAnimation
    {
        void Cancel();
    }

    public abstract class CancelableAnimation : ICancelableAnimation
    {
        private CancellationTokenSource _cancellation;
        private CancellationToken _token;

        async UniTask IAnimation.In(IView view)
        {
            _cancellation = new CancellationTokenSource();
            _token = _cancellation.Token;
            await In(view, _token);
        }

        async UniTask IAnimation.Out(IView view)
        {
            await Out(view, _token);
            _cancellation?.Dispose();
            _cancellation = null;
        }

        public void Cancel()
        {
            _cancellation?.Cancel();
            _cancellation = null;
        }

        protected abstract UniTask In(IView view, CancellationToken token);
        protected abstract UniTask Out(IView view, CancellationToken token);
    }
}