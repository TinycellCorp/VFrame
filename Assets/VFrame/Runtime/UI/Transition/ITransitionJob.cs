using Cysharp.Threading.Tasks;

namespace VFrame.UI.Transition
{
    public interface ITransitionJob
    {
        UniTask Execute();
    }
}