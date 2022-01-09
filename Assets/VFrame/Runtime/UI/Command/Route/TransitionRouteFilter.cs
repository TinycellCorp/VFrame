using Cysharp.Threading.Tasks;
using VFrame.UI.Context;
using VFrame.UI.Transition;
using VFrame.UI.View;

namespace VFrame.UI.Command.Route
{
    public class TransitionRouteFilter : IRouteFilter
    {
        public async UniTask<bool> Push(ISystemContext context, IView view)
        {
            if (!context.HasTransition(view)) return false;

            var group = context.ResolveGroup<TransitionGroup>();
            await @group.Push(context, view);
            return true;
        }

        public async UniTask<bool> Pop(ISystemContext context, IView view)
        {
            if (!context.HasTransition(view)) return false;

            var group = context.ResolveGroup<TransitionGroup>();
            await @group.Pop(context);
            return true;
        }
    }
}