using System;
using Cysharp.Threading.Tasks;
using VFrame.UI.Animation;
using VFrame.UI.Context;
using VFrame.UI.Transition;
using VFrame.UI.View;

namespace VFrame.UI.Command
{
    public abstract class TransitionCommandBase : ICommand
    {
        protected abstract IView GetNextView(ISystemContext context);
        protected abstract IAnimation GetNextAnimation(ISystemContext context);
        protected abstract ITransition GetTransition(ISystemContext context);

        private readonly InTransitionJob _job = new InTransitionJob();

        public async UniTask Execute(ISystemContext context)
        {
            using (UISystem.EnableCommandBlocking())
            {
                var nextView = GetNextView(context);
                if (context.View.Contains(nextView)) return;

                var transition = GetTransition(context);

                using (_job.Init(context, nextView))
                {
                    await transition.In(context, _job);
                    if (!_job.IsExecuted)
                    {
                        throw new Exception("require transition execute!");
                    }
                }

                var animation = GetNextAnimation(context);
                await animation.In(nextView);
                nextView.OnEnter();
            }
        }

        private class InTransitionJob : ITransitionJob, IDisposable
        {
            public bool IsExecuted { get; private set; }
            private ISystemContext _context;
            private IView _nextView;

            public IDisposable Init(ISystemContext context, IView nextView)
            {
                IsExecuted = false;
                _context = context;
                _nextView = nextView;
                return this;
            }

            public async UniTask Execute()
            {
                await _nextView.Ready();
                if (_context.View.TryPopManipulator(_nextView, out var manipulator))
                {
                    await manipulator.Ready(_nextView);
                }

                _context.View.ClearSafe();

                if (_context.View.Any())
                {
                    var prev = _context.View.Peek();
                    prev.IsActive = false;
                    prev.OnExit();
                }

                _context.View.Push(_nextView);

                _nextView.IsActive = true;
                IsExecuted = true;
            }

            public void Dispose()
            {
                _context = null;
                _nextView = null;
            }
        }
    }

    public class TransitionCommand<TView> : TransitionCommandBase where TView : class, IView
    {
        protected override IView GetNextView(ISystemContext context) => context.ResolveView<IView>();

        protected override IAnimation GetNextAnimation(ISystemContext context)
        {
            return context.ResolveAnimation<TView>();
        }

        protected override ITransition GetTransition(ISystemContext context)
        {
            return context.ResolveTransition<TView>();
        }
    }

    public class TransitionCommand : TransitionCommandBase
    {
        private readonly IView _view;

        public TransitionCommand(IView view)
        {
            _view = view;
        }

        protected override IView GetNextView(ISystemContext context)
        {
            return _view;
        }

        protected override IAnimation GetNextAnimation(ISystemContext context)
        {
            return context.ResolveAnimation(_view);
        }

        protected override ITransition GetTransition(ISystemContext context)
        {
            return context.ResolveTransition(_view);
        }
    }
}