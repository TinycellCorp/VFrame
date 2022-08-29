using Cysharp.Threading.Tasks;
using VFrame.UI.Context;

namespace VFrame.UI.Command.Route
{
    public class PopRouteGroupCommand : ICommand
    {
        private readonly bool _isSafety;

        public PopRouteGroupCommand(bool isSafety = true)
        {
            _isSafety = isSafety;
        }

        public async UniTask Execute(ISystemContext context)
        {
            if (_isSafety)
            {
                if (!context.View.SafetyAny()) return;
            }
            else
            {
                if (!context.View.Any()) return;
            }

            var filters = context.ResolveRouteFilters();
            var peek = context.View.Peek();

            foreach (var filter in filters)
            {
                var isSuccess = await filter.Pop(context, peek);
                if (isSuccess) return;
            }

            await new PopCommand(_isSafety).Execute(context);
            // await context.Command.Pop();
        }
    }
}