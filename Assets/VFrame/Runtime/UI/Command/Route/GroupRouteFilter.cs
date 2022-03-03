using Cysharp.Threading.Tasks;
using VFrame.UI.Context;
using VFrame.UI.View;

namespace VFrame.UI.Command.Route
{
    public class GroupRouteFilter : IRouteFilter
    {
        public async UniTask<bool> Push(ISystemContext context, IView view)
        {
            if (!context.TryResolveGroup(view, out var group)) return false;

            //todo: main stream is await or sub stream is non await
            await @group.Push(context, view);
            return true;
        }

        public async UniTask<bool> Pop(ISystemContext context, IView view)
        {
            if (!context.TryResolveGroup(view, out var group)) return false;

            await @group.Pop(context);
            return true;
        }
    }
}