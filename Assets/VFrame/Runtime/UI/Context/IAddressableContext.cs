using Cysharp.Threading.Tasks;
using VFrame.UI.View;

namespace VFrame.UI.Context
{
    public interface IAddressableContext
    {
        UniTask<ComponentView> LoadView(string key);
    }
}