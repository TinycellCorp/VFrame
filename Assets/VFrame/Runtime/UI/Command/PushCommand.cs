using System;
using Cysharp.Threading.Tasks;
using VFrame.UI.Animation;
using VFrame.UI.Context;
using VFrame.UI.View;

namespace VFrame.UI.Command
{
    public abstract class PushCommandBase : ICommand
    {
        protected abstract IView GetNextView(ISystemContext context);
        protected abstract IAnimation GetNextAnimation(ISystemContext context);

        public async UniTask Execute(ISystemContext context)
        {
            var view = GetNextView(context);

            if (context.View.Contains(view))
            {
                //TODO: Insert Back->EnqueueCommand(this)
                return;
            }

            await view.Ready(); //TODO: throw
            await PostReadyAsync(view);
            
            context.View.Push(view);

            view.IsActive = true;
            var animation = GetNextAnimation(context);
            await animation.In(view);
            view.OnEnter();
        }

        protected virtual UniTask PostReadyAsync(IView view)
        {
            return UniTask.CompletedTask;
        }
    }


    public class PushCommand<TView> : PushCommandBase where TView : class, IView
    {
        protected override IView GetNextView(ISystemContext context) => context.ResolveView<TView>();
        protected override IAnimation GetNextAnimation(ISystemContext context) => context.ResolveAnimation<TView>();
    }

    public class PushCommand : PushCommandBase
    {
        private readonly IView _nextView;
        public PushCommand(IView nextView) => _nextView = nextView;
        protected override IView GetNextView(ISystemContext context) => _nextView;
        protected override IAnimation GetNextAnimation(ISystemContext context) => context.ResolveAnimation(_nextView);
    }


    public interface IManipulator
    {
        UniTask Ready(IView view);
    }

    public class PushWithManipulatorCommand : PushCommand
    {
        private readonly IManipulator _manipulator;

        public PushWithManipulatorCommand(IView nextView, IManipulator manipulator) : base(nextView)
        {
            _manipulator = manipulator;
        }

        protected override UniTask PostReadyAsync(IView view)
        {
            return _manipulator.Ready(view);
        }
    }

    public class PushParallelCommand : ICommand
    {
        private readonly IView _nextView;

        public PushParallelCommand(IView nextView)
        {
            _nextView = nextView;
        }

        public async UniTask Execute(ISystemContext context)
        {
            var view = _nextView;

            if (ReferenceEquals(view, context.View.Peek()))
            {
                return;
            }

            await view.Ready(); //TODO: throw

            context.View.Push(view);
            view.IsActive = true;
            var animation = context.ResolveAnimation(view);
            animation.In(view).ContinueWith(() => { view.OnEnter(); });
        }
    }
}