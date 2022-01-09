using Cysharp.Threading.Tasks;
using VFrame.UI.Context;
using VFrame.UI.Group;
using VFrame.UI.View;

namespace VFrame.UI.Transition
{
    public class TransitionGroup : IGroup
    {
        public async UniTask Push(ISystemContext context, IView view)
        {
            if (context.View.Contains(view)) return;
            await context.Command.Transition(view);
        }

        public async UniTask Pop(ISystemContext context)
        {
            await context.Command.TransitionPop();
        }

        public void OnImmediatePop(IView view)
        {
        }
    }
}