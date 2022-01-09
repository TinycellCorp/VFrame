using Cysharp.Threading.Tasks;
using VFrame.UI.View;

namespace VFrame.UI.Animation
{
    public class ToggleAnimation : IAnimation<IView>
    {
        public UniTask In(IView view)
        {
            view.Alpha = 1;
            return UniTask.CompletedTask;
        }

        public UniTask Out(IView view)
        {
            view.Alpha = 0;
            return UniTask.CompletedTask;
        }
    }
}