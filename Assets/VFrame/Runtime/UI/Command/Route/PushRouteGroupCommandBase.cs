using Cysharp.Threading.Tasks;
using VFrame.UI.Context;
using VFrame.UI.View;

namespace VFrame.UI.Command.Route
{
    public abstract class PushRouteGroupCommandBase : ICommand
    {
        protected abstract IView GetNextView(ISystemContext context);

        public async UniTask Execute(ISystemContext context)
        {
            var view = GetNextView(context);

            var filters = context.ResolveRouteFilters();
            foreach (var filter in filters)
            {
                var isSuccess = await filter.Push(context, view);
                if (isSuccess) return;
            }

            await context.Command.Push(view);
        }
    }

    public class PushRouteGroupCommand : PushRouteGroupCommandBase
    {
        private readonly IView _nextView;

        public PushRouteGroupCommand(IView view)
        {
            _nextView = view;
        }

        protected override IView GetNextView(ISystemContext context)
        {
            return _nextView;
        }
    }

    public class PushRouteGroupCommand<TView> : PushRouteGroupCommandBase
        where TView : class, IView
    {
        protected override IView GetNextView(ISystemContext context)
        {
            return context.ResolveView<TView>();
        }
    }
 
    public class PushRouteGroupCommandWithManipulator : PushRouteGroupCommand
    {
        private readonly IManipulator _manipulator;

        public PushRouteGroupCommandWithManipulator(IView view, IManipulator manipulator) : base(view)
        {
            _manipulator = manipulator;
        }

        protected override IView GetNextView(ISystemContext context)
        {
            var view = base.GetNextView(context);
            context.View.PutManipulator(view, _manipulator);
            return view;
        }
    }

    public class PushRouteGroupCommandWithManipulator<TView> : PushRouteGroupCommand<TView>
        where TView : class, IView
    {
        private readonly IManipulator _manipulator;

        public PushRouteGroupCommandWithManipulator(IManipulator manipulator)
        {
            _manipulator = manipulator;
        }

        protected override IView GetNextView(ISystemContext context)
        {
            var view = base.GetNextView(context);
            context.View.PutManipulator(view, _manipulator);
            return view;
        }
    }
}