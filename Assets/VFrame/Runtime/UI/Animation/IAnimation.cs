using Cysharp.Threading.Tasks;
using VFrame.UI.View;

namespace VFrame.UI.Animation
{
    public interface IAnimation
    {
        UniTask In(IView view);
        UniTask Out(IView view);
    }
    
    public interface IAnimation<in TView> : IAnimation where TView : IView
    {
    }

}