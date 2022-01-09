using Cysharp.Threading.Tasks;
using VFrame.UI.Context;
using VFrame.UI.Group;
using VFrame.UI.View;

namespace VFrame.UI.Module.Window
{
    public class WindowGroup : IGroup
    {
        public async UniTask Push(ISystemContext context, IView view)
        {
            if (context.View.Contains(view)) return;

            if (!context.Group.Include<WindowGroup>(context.View.Peek()))
            {
                await context.Command.Push(view);
            }
            else
            {
                await context.Command.Replace(view);
            }
        }

        public async UniTask Pop(ISystemContext context)
        {
            await context.Command.Pop();
        }

        public void OnImmediatePop(IView view)
        {
        }
    }
}