using Cysharp.Threading.Tasks;
using VFrame.UI.Context;
using VFrame.UI.View;

namespace VFrame.UI.Group
{
    public interface IGroup
    {
        //TODO: naming (to,back), ()...
        UniTask Push(ISystemContext context, IView view);
        UniTask Pop(ISystemContext context);
        void OnImmediatePop(IView view);
    }
    
    public interface IGroup<T> : IGroup where T : IView
    {
    }

}