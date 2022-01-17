using Cysharp.Threading.Tasks;
using VFrame.UI.Context;
using VFrame.UI.View;

namespace VFrame.UI.Command.Route
{
    public abstract class PushRouteGroupCommandBase : ICommand
    {
        protected abstract IView GetNextView(ISystemContext context);

        public async UniTask Execute(ISystemContext context)
        {
            var view = GetNextView(context);

            var filters = context.ResolveRouteFilters();
            foreach (var filter in filters)
            {
                var isSuccess = await filter.Push(context, view);
                if (isSuccess) return;
            }

            if (context.View.TryPopManipulator(view, out var manipulator))
            {
                await context.Command.Push(view, manipulator);
            }
            else
            {
                await context.Command.Push(view);
            }
        }
    }
}