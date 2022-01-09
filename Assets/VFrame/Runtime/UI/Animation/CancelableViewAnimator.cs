using Cysharp.Threading.Tasks;
using VFrame.UI.Command;
using VFrame.UI.View;

namespace VFrame.UI.Animation
{
    public struct CancelableViewAnimator
    {
        private readonly IView _view;
        private readonly ICancelableAnimation _animation;
        private readonly IManipulator _manipulator;
        private bool _isReady;
        private bool _isPlaying;

        public CancelableViewAnimator(IView view, ICancelableAnimation animation, IManipulator manipulator = null)
        {
            _view = view;
            _animation = animation;
            _manipulator = manipulator;
            _isReady = false;
            _isPlaying = false;
        }

        public async UniTask In()
        {
            if (_isPlaying) return;
            _isPlaying = true;

            _view.IsActive = true;
            if (!_isReady)
            {
                await _view.Ready();
                _manipulator?.Ready(_view);
                _isReady = true;
            }

            await _animation.In(_view);

            _isPlaying = false;
        }

        public async UniTask Out()
        {
            if (_isPlaying) return;
            _isPlaying = true;

            await _animation.Out(_view);
            _view.IsActive = false;
            _isPlaying = false;
            _isReady = false;
        }

        public async UniTask Play()
        {
            await In();
            await Out();
        }

        public void Cancel()
        {
            _animation.Cancel();
            _view.IsActive = false;
            _isPlaying = false;
            _isReady = false;
        }
    }
}