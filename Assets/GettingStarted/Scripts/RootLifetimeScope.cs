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

            builder.Register<Company>(Lifetime.Singleton).AsSelf();
            builder.Register<Person>(Lifetime.Singleton).AsSelf();
        }
    }


    public class Company
    {
        [Inject] private Person _ceo;
        
        public string CeoName => $"CEO: {_ceo.Name}";
    }

    public class Person
    {
        public string Name => nameof(Person);
    }
}