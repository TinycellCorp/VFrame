using VFrame.UI;
using VFrame.UI.Group;
using VContainer;
using VContainer.Unity;

namespace VFrame.Scene
{
    public class SceneScope<TEntry> : LifetimeScope
        where TEntry : SceneEntry
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            UISystem.Configure(this, builder);
            builder.RegisterEntryPoint<TEntry>().AsSelf();
            builder.Register<AssetCache>(Lifetime.Singleton);
            builder.Register<IPreloader, NothingPreload>(Lifetime.Singleton);
            builder.RegisterEntryPoint<UISystemInitializer>();
        }

        protected override void OnDestroy()
        {
            UISystem.Clear(this);
            base.OnDestroy();
        }
    }

    public class SceneScope<TEntry, TPreloader> : LifetimeScope
        where TEntry : SceneEntry
        where TPreloader : IPreloader
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            UISystem.Configure(this, builder);
            builder.RegisterEntryPoint<TEntry>().AsSelf();
            builder.Register<AssetCache>(Lifetime.Singleton);
            builder.Register<IPreloader, TPreloader>(Lifetime.Singleton);
            builder.RegisterEntryPoint<UISystemInitializer>();
        }

        protected override void OnDestroy()
        {
            UISystem.Clear(this);
            base.OnDestroy();
        }
    }
}