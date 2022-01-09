using Cysharp.Threading.Tasks;
using VFrame.UI.Context;
using VFrame.UI.View;

namespace VFrame.UI.Command.Route
{
    public interface IRouteFilter
    {
        UniTask<bool> Push(ISystemContext context, IView view);
        UniTask<bool> Pop(ISystemContext context, IView view);
    }
}