#if VFRAME_DOTWEEN
using Cysharp.Threading.Tasks;
using VFrame.UI.Animation;
using VFrame.UI.View;

namespace VFrame.UI.External
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
#endif