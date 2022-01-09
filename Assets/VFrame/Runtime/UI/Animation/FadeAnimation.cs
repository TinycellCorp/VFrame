using Cysharp.Threading.Tasks;
using VFrame.UI.Extension;
using VFrame.UI.View;

namespace VFrame.UI.Animation
{
    public class FadeAnimation<TView> : IAnimation<TView> where TView : IView
    {
        public async UniTask In(IView view)
        {
            await view.DOFade(1, 0.3f);
        }

        public async UniTask Out(IView view)
        {
            await view.DOFade(0, 0.3f);
        }
    }
}