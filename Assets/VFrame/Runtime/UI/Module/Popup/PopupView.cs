using VFrame.UI.Animation;
using VFrame.UI.Group;
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
            //UISystem.RegisterPopup<TView, IPopupGroup, IAnimation<IPopupView>>(this);
            //Layer 스펙 점검 필요. 그 전 까지 비활성화 (AddressableView 부모 못 찾는 상태) 
            UISystem.Register<TView, PopupLayer, IPopupGroup, IAnimation<IPopupView>>(this);
            Init();
        }
    }
}

namespace VFrame.UI
{
    public partial class UISystem
    {
        public static void RegisterPopup<TView, TGroup, TAnimation>(ComponentView<TView> view)
            where TView : IView
            where TGroup : class, IGroup
            where TAnimation : class, IAnimation
        {
            RegisterView(view);
            ExecuteCacheCommand(new MergeCommand(
                new CacheAnimationCommand<TView, TAnimation>(view),
                new CacheTransitionCommand<TView>(view),
                new CacheGroupCommand<TView, TGroup>(view)
            ));
        }
    }
}