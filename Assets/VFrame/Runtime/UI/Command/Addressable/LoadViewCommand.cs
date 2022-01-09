using Cysharp.Threading.Tasks;
using VFrame.UI.Command.Route;
using VFrame.UI.Context;

namespace VFrame.UI.Command.Addressable
{
    public class LoadViewCommand : ICommand
    {
        private readonly string _key;

        public LoadViewCommand(string key)
        {
            _key = key;
        }

        public async UniTask Execute(ISystemContext context)
        {
            var view = await context.Addressable.LoadView(_key);
            await context.Command.To(view);
        }
    }

    public class LoadViewWithManipulatorCommand : ICommand
    {
        private readonly string _key;
        private readonly IManipulator _manipulator;

        public LoadViewWithManipulatorCommand(string key, IManipulator manipulator)
        {
            _key = key;
            _manipulator = manipulator;
        }

        public async UniTask Execute(ISystemContext context)
        {
            var view = await context.Addressable.LoadView(_key);
            await context.Command.To(view, _manipulator);
        }
    }
}