using Cysharp.Threading.Tasks;
using VFrame.UI.Context;

namespace VFrame.UI.Command
{
    public interface ICommand
    {
        UniTask Execute(ISystemContext context);
    }
}