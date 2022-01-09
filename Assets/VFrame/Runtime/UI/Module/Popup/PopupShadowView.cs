using Cysharp.Threading.Tasks;
using VFrame.UI.View;

namespace VFrame.UI.Module.Popup
{
    public class PopupShadowView : ComponentView<PopupShadowView>
    {
        protected override void Awake()
        {
            base.Awake();
        }

        public override UniTask Ready()
        {
            var task = base.Ready();
            Alpha = 0;
            return task;
        }

        public override void OnEnter()
        {
            
        }

        public override void OnExit()
        {
        }
    }
}
