using VContainer;
using VContainer.Unity;

namespace VFrame.UI
{
    public class UISystemInitializer : IInitializable
    {
        private readonly IObjectResolver _resolver;
        public UISystemInitializer(IObjectResolver resolver) => _resolver = resolver;

        public void Initialize()
        {
            UISystem.Initialize(_resolver);
        }
    }

    public class UISystemRootInitializer : IInitializable
    {
        private readonly IObjectResolver _resolver;
        public UISystemRootInitializer(IObjectResolver resolver) => _resolver = resolver;

        public void Initialize()
        {
            UISystem.Initialize(_resolver, true);
        }
    }
}