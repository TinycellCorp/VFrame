using Cysharp.Threading.Tasks;
using VFrame.UI.Animation;
using VFrame.UI.View;

namespace VFrame.UI.Module.Message
{
    public interface IMessageAnimation : IAnimation<IMessageView>
    {
        UniTask Shift(IView view, int index);
        UniTask Hide(IView view);
    }
}