using System.Collections.Generic;
using System.Linq;
using VFrame.UI;
using VContainer;
using VContainer.Unity;

namespace VFrame.Scene
{
    public static class SceneScope
    {
        private static readonly Dictionary<string, Queue<IReserveRegistrable>> Installers =
            new Dictionary<string, Queue<IReserveRegistrable>>();


        public static void Reserve(IReserveRegistrable registrable)
        {
            if (!Installers.TryGetValue(registrable.SceneName, out var queue))
            {
                queue = new Queue<IReserveRegistrable>();
                Installers.Add(registrable.SceneName, queue);
            }

            queue.Enqueue(registrable);
        }

        public static void Configure(LifetimeScope scope, IContainerBuilder builder)
        {
            var sceneName = scope.gameObject.scene.name;
            if (Installers.TryGetValue(sceneName, out var queue))
            {
                while (queue.Any())
                {
                    queue.Dequeue().Configure(builder);
                }

                Installers.Remove(sceneName);
            }
        }

        public interface IReserveRegistrable
        {
            string SceneName { get; }
            void Configure(IContainerBuilder builder);
        }
    }


    public class SceneScope<TEntry> : LifetimeScope
        where TEntry : SceneEntry
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            SceneScope.Configure(this, builder);
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
            SceneScope.Configure(this, builder);
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