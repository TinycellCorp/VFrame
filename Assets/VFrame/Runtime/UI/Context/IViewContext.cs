using Cysharp.Threading.Tasks;
using VFrame.UI.Command;
using VFrame.UI.View;

namespace VFrame.UI.Context
{
    public interface IViewContext
    {
        IView Peek();
        void Push(IView view);
        IView Pop();
        bool Any();
        bool SafetyAny();
        bool Contains(IView view);

        void PutManipulator(IView view, IManipulator manipulator);
        bool TryPopManipulator(IView view, out IManipulator manipulator);

        void ImmediatePop();
        
    }
}