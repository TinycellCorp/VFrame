using System;
using System.Collections.Generic;
using System.Linq;
using VFrame.UI.Command;
using VFrame.UI.Context;
using VFrame.UI.Extension;
using VFrame.UI.View;

namespace VFrame.UI
{
    public partial class UISystem : IViewContext
    {
        private readonly Stack<IView> _views = new Stack<IView>();
        private readonly Dictionary<IView, IManipulator> _manipulators = new Dictionary<IView, IManipulator>();
        public IViewContext View => this;

        void IViewContext.Push(IView view)
        {
            if (_views.Contains(view))
            {
                throw new Exception("Push Contains View");
            }

            if (_views.Any())
            {
                _views.Peek().IsInteractable = false;
            }

            _views.Push(view);
        }

        IView IViewContext.Pop()
        {
            var view = _views.Pop();
            view.IsInteractable = false;

            if (_views.Any())
                _views.Peek().IsInteractable = true;

            return view;
        }


        IView IViewContext.Peek()
        {
            if (_views.Any())
            {
                return _views.Peek();
            }

            return null;
        }


        bool IViewContext.Any()
        {
            return _views.Any();
        }

        bool IViewContext.SafetyAny()
        {
            return _views.Count > 1;
        }

        bool IViewContext.Contains(IView view)
        {
            return _views.Contains(view);
        }

        void IViewContext.ClearStack()
        {
            while (View.SafetyAny())
            {
                View.ImmediatePop();
            }
        }

        void IViewContext.PutManipulator(IView view, IManipulator manipulator)
        {
            _manipulators[view] = manipulator;
        }

        bool IViewContext.TryPopManipulator(IView view, out IManipulator manipulator)
        {
            var has = _manipulators.TryGetValue(view, out manipulator);
            _manipulators.Remove(view);
            return has;
        }

        void IViewContext.ImmediatePop()
        {
            if ((this as IViewContext).SafetyAny())
            {
                var view = _views.Pop();
                view.Hide();
                if (_groups.TryGetValue(view, out var @group))
                {
                    group.OnImmediatePop(view);
                }
            }
        }
    }
}