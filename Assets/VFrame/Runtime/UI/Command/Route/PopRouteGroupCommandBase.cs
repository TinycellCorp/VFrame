using Cysharp.Threading.Tasks;
using VFrame.UI.Context;
using VFrame.UI.Group;

namespace VFrame.UI.Command.Route
{
    public class PopRouteGroupCommand : ICommand
    {
        public async UniTask Execute(ISystemContext context)
        {
            if (!context.View.SafetyAny()) return;
            
            var filters = context.ResolveRouteFilters();
            var peek = context.View.Peek();

            foreach (var filter in filters)
            {
                var isSuccess = await filter.Pop(context, peek);
                if (isSuccess) return;
            }

            await context.Command.Pop();
        }
    }
}