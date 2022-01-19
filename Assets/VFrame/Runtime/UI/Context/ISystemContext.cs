using System;
using System.Collections.Generic;
using VContainer.Unity;
using VFrame.UI.Animation;
using VFrame.UI.Command.Route;
using VFrame.UI.Group;
using VFrame.UI.Transition;
using VFrame.UI.View;

namespace VFrame.UI.Context
{
    public interface ISystemContext
    {
        T Resolve<T>();
        T ResolveView<T>() where T : class, IView;
        IAnimation ResolveAnimation<T>() where T : class, IView;
        IAnimation ResolveAnimation(IView view);


        bool HasTransition(IView view);
        ITransition ResolveTransition<T>() where T : class, IView;
        ITransition ResolveTransition(IView view);
        IGroup ResolveGroup<TGroup>() where TGroup : class, IGroup;
        bool TryResolveGroup(IView view, out IGroup group);
        IReadOnlyList<IRouteFilter> ResolveRouteFilters();

        ViewAnimator ResolveAnimator<TView>() where TView : class, IView;
        ViewAnimator ResolveAnimator(IView view);

        IViewContext View { get; }
        IGroupContext Group { get; }
        ICommandContext Command { get; }
        IAddressableContext Addressable { get; }

    }
}