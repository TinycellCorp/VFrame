using VFrame.UI.Animation;
using VFrame.UI.View;

namespace VFrame.UI.Module.Window
{
    public interface IWindowView : IView
    {
    }

    public abstract class WindowView<T> : ComponentView<T>, IWindowView where T : ComponentView
    {
        protected override void Awake()
        {
            UISystem.RegisterGroupWithAnimation<T, WindowGroup, IAnimation<IWindowView>>(this);
            Init();
        }
    }
}