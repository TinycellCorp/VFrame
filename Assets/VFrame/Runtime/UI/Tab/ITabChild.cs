using VFrame.UI.Context;
using VFrame.UI.View;

namespace VFrame.UI.Tab
{
    public interface ITabChild<TParent> where TParent : class, IView
    {
        IView ResolveView(ISystemContext context);
    }
    
    public class TabChildInstance<TParent, TChild> : ITabChild<TParent>
        where TParent : class, IView
        where TChild : class, IView
    {
        public IView ResolveView(ISystemContext context)
        {
            return context.ResolveView<TChild>();
        }
    }

}