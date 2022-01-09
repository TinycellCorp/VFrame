using System.Collections.Generic;
using VFrame.UI.Context;
using VFrame.UI.View;
using VContainer;

namespace VFrame.UI
{
    public partial class UISystem : IGroupContext
    {
        public IGroupContext Group => this;

        bool IGroupContext.Include<TGroup>(IView view)
        {
            var target = _container.Resolve<TGroup>();
            if (_groups.TryGetValue(view, out var cached))
            {
                return ReferenceEquals(target, cached);
            }

            return false;
        }
    }
}