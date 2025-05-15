using VContainer;
using VContainer.Unity;
using VFrame.External.LitMotion;
using VFrame.UI.Extension;
using VFrame.UI.View;

namespace GettingStarted
{
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.UseUISystem();
            
            builder.RegisterViewDefaultAnimation<FadeAnimation<IView>>();
        }
    }
}