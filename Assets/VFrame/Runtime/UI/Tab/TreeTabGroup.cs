using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using VFrame.UI.Context;
using VFrame.UI.Extension;
using VFrame.UI.Group;
using VFrame.UI.View;

namespace VFrame.UI.Tab
{
    public class TreeTabNode
    {
        public readonly Type ViewType;
        public readonly List<TreeTabNode> Children;

        public TreeTabNode(Type viewType)
        {
            ViewType = viewType;
            Children = new List<TreeTabNode>();
        }
    }

    public class TreeTabRoot<TRoot>
    {
        private readonly TreeTabNode _rootNode;

        public TreeTabRoot(TreeTabNode rootNode)
        {
            _rootNode = rootNode;
        }

        public void Find(ISystemContext context, IView targetView, List<IView> path)
        {
            if (!Recursive(_rootNode))
            {
                throw new Exception($"not found child: parent = {typeof(TRoot).Name}");
            }

            bool Recursive(TreeTabNode node)
            {
                var view = context.Resolve<IView>(node.ViewType);
                if (view == targetView)
                {
                    if (node.Children.Any())
                    {
                        throw new Exception($"{node.ViewType.Name} is not leaf");
                    }

                    path.Add(view);
                    return true;
                }

                if (node.Children.Count > 0)
                {
                    path.Add(view);
                    foreach (var child in node.Children)
                    {
                        if (Recursive(child)) return true;
                    }

                    path.RemoveAt(path.Count - 1);
                }

                return false;
            }
        }
    }

    public class TreeTabGroup<TRoot> : IGroup where TRoot : class, IView
    {
        private List<IView> _searchedPath = new();
        private List<IView> _path = new();

        private readonly List<UniTask> _awaitTasks = new();

        public async UniTask Push(ISystemContext context, IView view)
        {
            if (_searchedPath.Any() && _searchedPath.Last() == view)
            {
                return;
            }

            var root = context.Resolve<TreeTabRoot<TRoot>>();
            root.Find(context, view, _path);

            var rootView = _path.First();

            var startIndex = 1;
            if (!context.View.Contains(rootView))
            {
                startIndex = 0;
                context.View.Push(rootView);
            }
            
            
            var hideIndex = startIndex;
            for (int i = hideIndex; i < _searchedPath.Count; i++)
            {
                if (i < _path.Count && _searchedPath[i] == _path[i])
                {
                    hideIndex++;
                    continue;
                }

                break;
            }

            for (int i = _searchedPath.Count - 1; i >= hideIndex; i--)
            {
                _searchedPath[i].Hide();
            }

            _searchedPath.Clear();

            #region Path Ready

            for (int i = hideIndex; i < _path.Count; i++)
            {
                _path[i].IsActive = true;
                await _path[i].Ready();
            }

            #endregion

            #region animation in

            for (int i = hideIndex; i < _path.Count; i++)
            {
                var ani = context.ResolveAnimator(_path[i]);
                _awaitTasks.Add(ani.In());
            }

            await _awaitTasks;
            _awaitTasks.Clear();

            #endregion

            #region On Enter

            for (int i = hideIndex; i < _path.Count; i++)
            {
                _path[i].OnEnter();
            }

            #endregion

            (_searchedPath, _path) = (_path, _searchedPath);
        }

        public async UniTask Pop(ISystemContext context)
        {
            for (int i = _searchedPath.Count - 1; i > 0; i--)
            {
                _searchedPath[i].Hide();
            }

            _searchedPath.Clear();
            await context.Command.Pop();
        }

        public void OnImmediatePop(IView view)
        {
            for (int i = _searchedPath.Count - 1; i >= 0; i--)
            {
                _searchedPath[i].Hide();
            }

            _searchedPath.Clear();
        }
    }
}