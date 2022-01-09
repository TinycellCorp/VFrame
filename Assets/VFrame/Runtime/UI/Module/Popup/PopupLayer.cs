using VFrame.UI.Layer;

namespace VFrame.UI.Module.Popup
{
    public class PopupLayer : CanvasLayer
    {
        protected override void Awake()
        {
            base.Awake();
            UISystem.Register(this);
        }

        private void OnDestroy()
        {
            UISystem.Unregister(this);
        }
    }
}