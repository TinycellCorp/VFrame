using System;
using Cysharp.Threading.Tasks;
using VFrame.UI.Context;
using VFrame.UI.Transition;
using VFrame.UI.View;

namespace VFrame.UI.Command
{
    public class TransitionPopCommand : ICommand
    {
        private readonly OutTransitionJob _job = new OutTransitionJob();

        public async UniTask Execute(ISystemContext context)
        {
            if (!context.View.SafetyAny()) return;

            var outView = context.View.Pop();
            var outTransition = context.ResolveTransition(outView);

            using (_job.Init(context, outView))
            {
                await outTransition.Out(context, _job);
                if (!_job.IsExecuted)
                {
                    throw new Exception("require transition execute!");
                }

                await _job.RestoreTask;
            }
        }

        private class OutTransitionJob : ITransitionJob, IDisposable
        {
            public bool IsExecuted { get; private set; }
            public UniTask RestoreTask { get; private set; }

            private ISystemContext _context;
            private IView _outView;

            public IDisposable Init(ISystemContext context, IView outView)
            {
                IsExecuted = false;
                RestoreTask = UniTask.CompletedTask;
                _context = context;
                _outView = outView;
                return this;
            }

            public async UniTask Execute()
            {
                _outView.IsActive = false;
                _outView.OnExit();

                if (_context.View.Any())
                {
                    var restoreView = _context.View.Peek();
                    if (restoreView.IsActive == false)
                    {
                        var restoreAnimation = _context.ResolveAnimation(restoreView);
                        await restoreView.Ready();

                        restoreView.IsActive = true;

                        async UniTask Restore()
                        {
                            await restoreAnimation.In(restoreView);
                            restoreView.OnEnter();
                        }

                        RestoreTask = Restore();
                    }
                }

                IsExecuted = true;
            }

            public void Dispose()
            {
                _context = null;
                _outView = null;
            }
        }
    }
}