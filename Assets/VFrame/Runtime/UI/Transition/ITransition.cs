using Cysharp.Threading.Tasks;
using VFrame.UI.Context;
using VFrame.UI.View;

namespace VFrame.UI.Transition
{
    public interface ITransition
    {
        UniTask In(ISystemContext context, ITransitionJob job);
        UniTask Out(ISystemContext context, ITransitionJob job);
    }

    public interface ITransition<TTo> : ITransition where TTo : IView
    {
    }
}