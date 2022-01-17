using Cysharp.Threading.Tasks;
using VFrame.UI.Animation;
using VFrame.UI.Context;
using VFrame.UI.Group;
using VFrame.UI.View;

namespace VFrame.UI.Module.Toast
{
    public class ToastGroup : IGroup
    {
        public async UniTask Push(ISystemContext context, IView view)
        {
            if (!(context.ResolveAnimation(view) is CancelableAnimation animation)) return;
            if (!context.View.TryPopManipulator(view, out var manipulator)) return;

            var animator = new CancelableViewAnimator(view, animation, manipulator);
            animator.Cancel();
            animator.Play().Forget();
        }

        public UniTask Pop(ISystemContext context) => UniTask.CompletedTask;

        public void OnImmediatePop(IView view)
        {
        }
    }
}