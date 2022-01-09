using Cysharp.Threading.Tasks;
using VFrame.UI.Command;
using VFrame.UI.View;

namespace VFrame.UI.Context
{
    public interface ICommandContext
    {
        
        //TODO: ExecuteCommand<TCommand>();
        UniTask Execute(ICommand command);
        UniTask To(IView view);
        UniTask To(IView view, IManipulator manipulator);
        
        UniTask Push(IView view);
        UniTask Push(IView view, IManipulator manipulator);
        UniTask Replace(IView view);
        UniTask Pop();

        UniTask Transition(IView view);
        UniTask TransitionPop();
    }
}