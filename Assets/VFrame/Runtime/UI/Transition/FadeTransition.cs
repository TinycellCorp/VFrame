using Cysharp.Threading.Tasks;
using VFrame.UI.Animation;
using VFrame.UI.Context;
using VFrame.UI.Extension;
using VFrame.UI.View;

namespace VFrame.UI.Transition
{
    public abstract class FadeTransition : ITransition
    {
        public readonly struct Handler
        {
            private readonly FadeView _fade;
            private readonly IAnimation _animation;

            public Handler(ISystemContext context)
            {
                _fade = context.ResolveView<FadeView>();
                _animation = context.ResolveAnimation<FadeView>();
            }

            public UniTask In() => _fade.In(_animation);
            public UniTask Out() => _fade.Out(_animation);
        }

        public async UniTask In(ISystemContext context, ITransitionExecutor executor)
        {
            var handler = new Handler(context);

            await handler.In();

            await executor.Execute();
            await In(context);

            await handler.Out();
        }

        public async UniTask Out(ISystemContext context, ITransitionExecutor executor)
        {
            var handler = new Handler(context);

            await handler.In();

            await executor.Execute();
            await Out(context);

            await handler.Out();
        }

        protected abstract UniTask In(ISystemContext context);
        protected abstract UniTask Out(ISystemContext context);
    }
}