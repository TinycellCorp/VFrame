using System;
using VContainer;
using VFrame.UI.Group;
using VFrame.UI.Matcher;
using VFrame.UI.Tab;
using VFrame.UI.View;

namespace VFrame.UI.Extension.VContainer
{
    public static class TreeTabExtensions
    {
        public static void AddTreeTab<TRoot>(this IContainerBuilder builder,
            Action<TreeTabBuilder<TRoot>> configuration)
            where TRoot : class, IView
        {
            var rootNode = new TreeTabNode(typeof(TRoot));
            configuration(new TreeTabBuilder<TRoot>(builder, rootNode));
            builder.RegisterInstance(new TreeTabRoot<TRoot>(rootNode));
            builder.RegisterGroup<TreeTabGroup<TRoot>>();
            
            builder.Register<IViewMatcher<IGroup>, GroupMatcher<TRoot, TreeTabGroup<TRoot>>>(Lifetime.Scoped);
        }

        public readonly struct TreeTabBuilder<TRoot> where TRoot : class, IView
        {
            private readonly IContainerBuilder _builder;
            private readonly TreeTabNode _parentNode;

            public TreeTabBuilder(IContainerBuilder builder, TreeTabNode parentNode)
            {
                _builder = builder;
                _parentNode = parentNode;
            }

            public TreeTabBuilder<TRoot> Append<TView>() where TView : class, IView
            {
                var node = new TreeTabNode(typeof(TView));
                _parentNode.Children.Add(node);
                return new TreeTabBuilder<TRoot>(_builder, node);
            }

            public void Leaf<TView>() where TView : class, IView
            {
                var node = new TreeTabNode(typeof(TView));
                _parentNode.Children.Add(node);
                _builder.Register<IViewMatcher<IGroup>, GroupMatcher<TView, TreeTabGroup<TRoot>>>(Lifetime.Scoped);
            }
        }
    }
}