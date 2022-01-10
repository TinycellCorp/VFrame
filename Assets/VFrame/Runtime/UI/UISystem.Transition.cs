using Cysharp.Threading.Tasks;
using VFrame.UI.Command;
using VFrame.UI.Context;
using VFrame.UI.Transition;
using UnityEngine.SceneManagement;

namespace VFrame.UI
{
    public partial class UISystem
    {
        public static void Transition<T>(T transition)
            where T : class, ITransition
        {
            if (IsBlocking) return;
            EnqueueCommand(new InTransitionCommand(transition));
        }


        private class InTransitionCommand : ICommand, ITransitionJob
        {
            private readonly ITransition _transition;
            public InTransitionCommand(ITransition transition) => _transition = transition;

            private ISystemContext _context;

            public async UniTask Execute(ISystemContext context)
            {
                _context = context;
                await _transition.In(context, this);
            }

            UniTask ITransitionJob.Execute()
            {
                while (_context.View.Any())
                {
                    _context.View.ImmediatePop();
                }

                return UniTask.CompletedTask;
            }
        }
    }
}