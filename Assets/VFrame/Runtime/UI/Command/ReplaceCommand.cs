using Cysharp.Threading.Tasks;
using VFrame.UI.Animation;
using VFrame.UI.Context;
using VFrame.UI.View;

namespace VFrame.UI.Command
{
    public abstract class ReplaceCommandBase : ICommand
    {
        protected abstract IView GetNextView(ISystemContext context);
        protected abstract IAnimation GetNextAnimation(ISystemContext context);

        protected abstract ICommand CreatePushCommand();

        public async UniTask Execute(ISystemContext context)
        {
            if (!context.View.Any())
            {
                await context.Command.Execute(CreatePushCommand());
                return;
            }

            var prev = context.View.Pop();
            var view = GetNextView(context);

            if (ReferenceEquals(prev, view))
            {
                view.IsActive = false;
                await context.Command.Execute(CreatePushCommand());
                return;
            }

            await view.Ready(); //TODO: throw
            context.View.Push(view);

            var inAnimation = GetNextAnimation(context);
            var outAnimation = context.ResolveAnimation(prev);
            view.IsActive = true;
            await UniTask.WhenAll(
                outAnimation.Out(prev),
                inAnimation.In(view)
            );
            prev.IsActive = false;
            prev.OnExit();
            view.OnEnter();
        }
    }

    public class ReplaceCommand<TView> : ReplaceCommandBase where TView : class, IView
    {
        protected override IView GetNextView(ISystemContext context) => context.ResolveView<TView>();
        protected override IAnimation GetNextAnimation(ISystemContext context) => context.ResolveAnimation<TView>();
        protected override ICommand CreatePushCommand() => new PushCommand<TView>();
    }

    public class ReplaceCommand : ReplaceCommandBase
    {
        private readonly IView _nextView;
        public ReplaceCommand(IView nextView) => _nextView = nextView;
        protected override IView GetNextView(ISystemContext context) => _nextView;
        protected override IAnimation GetNextAnimation(ISystemContext context) => context.ResolveAnimation(_nextView);
        protected override ICommand CreatePushCommand() => new PushCommand(_nextView);
    }
}