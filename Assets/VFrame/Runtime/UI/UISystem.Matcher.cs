using System;
using System.Collections.Generic;
using VFrame.UI.Animation;
using VFrame.UI.Group;
using VFrame.UI.Matcher;
using VFrame.UI.View;
using VContainer;

namespace VFrame.UI
{
    public partial class UISystem
    {
        private readonly Dictionary<Type, IViewMatcher<IGroup>> _groupMatchers =
            new Dictionary<Type, IViewMatcher<IGroup>>();

        private readonly Dictionary<Type, IViewMatcher<IAnimation>> _animationMatchers =
            new Dictionary<Type, IViewMatcher<IAnimation>>();

        private static void InitializeMatcher(IObjectResolver resolver)
        {
            InitializeGroupMatcher(resolver);
            // InitializeAnimationMatcher(resolver);
        }

        #region Group Matcher

        private static void InitializeGroupMatcher(IObjectResolver resolver)
        {
            try
            {
                var matchers = resolver.Resolve<IReadOnlyList<IViewMatcher<IGroup>>>();
                foreach (var matcher in matchers)
                {
                    _sharedInstance._groupMatchers.Add(matcher.ViewType, matcher);
                }
            }
            catch
            {
                // ignored
            }
        }

        private void ConsumeMatcher(IView view, in IDictionary<IView, IGroup> cache, out IGroup @group)
        {
            group = null;
            var viewType = view.GetType();
            if (_groupMatchers.TryGetValue(viewType, out var resolver))
            {
                group = resolver.Resolve(this);
                cache.Add(view, group);
                _groupMatchers.Remove(viewType);
            }
        }

        #endregion

        #region Animation Matcher

        private static void InitializeAnimationMatcher(IObjectResolver resolver)
        {
            try
            {
                var matchers = resolver.Resolve<IReadOnlyList<IViewMatcher<IAnimation>>>();
                foreach (var matcher in matchers)
                {
                    _sharedInstance._animationMatchers.Add(matcher.ViewType, matcher);
                }
            }
            catch
            {
                // ignored
            }
        }

        private void ConsumeMatcher(IView view, in IDictionary<IView, IAnimation> cache, out IAnimation animation)
        {
            animation = null;
            var viewType = view.GetType();
            if (_animationMatchers.TryGetValue(viewType, out var resolver))
            {
                animation = resolver.Resolve(this);
                cache.Add(view, animation);
                _groupMatchers.Remove(viewType);
            }

        }

        #endregion
    }
}