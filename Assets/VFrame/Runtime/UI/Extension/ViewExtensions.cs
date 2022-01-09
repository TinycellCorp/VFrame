using Cysharp.Threading.Tasks;
using VFrame.UI.Animation;
using VFrame.UI.View;

namespace VFrame.UI.Extension
{
    public static class ViewExtensions
    {
        public static async UniTask In(this IView view, IAnimation animation)
        {
            view.IsActive = true;
            await view.Ready();
            await animation.In(view);
        }

        public static async UniTask Out(this IView view, IAnimation animation)
        {
            await animation.Out(view);
            view.IsActive = false;
        }

        public static async UniTask Show(this IView view)
        {
            await view.Ready();
            view.Alpha = 1;
            view.IsActive = true;
            view.OnEnter();
        }

        public static void Hide(this IView view)
        {
            view.IsActive = false;
            view.OnExit();
        }
    }
}