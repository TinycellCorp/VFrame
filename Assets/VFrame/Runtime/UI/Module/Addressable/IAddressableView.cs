using VFrame.UI.View;

namespace VFrame.UI.Module.Addressable
{
    public interface IAddressableView<TView> where TView : IView
    {
        public string Key { get; }
    }
}