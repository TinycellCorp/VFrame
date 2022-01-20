using VFrame.UI.Animation;
using VFrame.UI.View;

namespace VFrame.UI.Module.Popup
{
    public interface IPopupView : IView
    {
    }

    public abstract class PopupView<TView> : ComponentView<TView>, IPopupView where TView : IView
    {
        protected override void Awake()
        {
            UISystem.Register<TView, PopupLayer, IPopupGroup, IAnimation<IPopupView>>(this);
            Init();
        }

    }
}