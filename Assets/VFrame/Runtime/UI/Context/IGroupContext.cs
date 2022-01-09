using VFrame.UI.Group;
using VFrame.UI.View;


namespace VFrame.UI.Context
{
    public interface IGroupContext
    {
        bool Include<TGroup>(IView view) where TGroup : class, IGroup;
    }
}