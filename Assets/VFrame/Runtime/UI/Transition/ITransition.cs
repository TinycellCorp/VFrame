using Cysharp.Threading.Tasks;
using VFrame.UI.Command;
using VFrame.UI.Context;
using VFrame.UI.View;

namespace VFrame.UI.Transition
{
    public interface ITransition
    {
        UniTask In(ISystemContext context, ITransitionExecutor executor);
        UniTask Out(ISystemContext context, ITransitionExecutor executor);
    }

    public interface ITransitionExecutor
    {
        UniTask Execute();
    }

    public interface ITransition<TTo> : ITransition where TTo : IView
    {
    }
}