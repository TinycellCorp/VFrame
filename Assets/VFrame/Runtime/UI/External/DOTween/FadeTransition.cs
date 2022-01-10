using Cysharp.Threading.Tasks;
using VFrame.UI.Animation;
using VFrame.UI.Context;
using VFrame.UI.Extension;
using VFrame.UI.Transition;

namespace VFrame.UI.External
{
    public abstract class FadeTransition : ITransition
    {
        public async UniTask In(ISystemContext context, ITransitionJob job)
        {
            var animator = context.ResolveAnimator<FadeView>();
            await animator.In();

            await job.Execute();
            await In(context);

            await animator.Out();
        }

        public async UniTask Out(ISystemContext context, ITransitionJob job)
        {
            var animator = context.ResolveAnimator<FadeView>();
            await animator.In();

            await job.Execute();
            await Out(context);

            await animator.Out();
        }

        protected abstract UniTask In(ISystemContext context);
        protected abstract UniTask Out(ISystemContext context);
    }
}