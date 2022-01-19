using System;
using System.Collections.Generic;
using VFrame.Extension;
using VFrame.UI.Animation;
using VFrame.UI.Command.Route;
using VFrame.UI.Group;
using VFrame.UI.Matcher;
using VFrame.UI.Module.Popup;
using VFrame.UI.Module.Toast;
using VFrame.UI.Module.Window;
using VFrame.UI.Tab;
using VFrame.UI.Transition;
using VFrame.UI.View;
using VContainer;
using VContainer.Unity;
using VFrame.Audio;
using VFrame.Core;
using VFrame.UI.Context;
using VFrame.UI.External;
using Object = UnityEngine.Object;

namespace VFrame.UI.Extension
{
    public static class VContainerBuilderExtensions
    {
        public static void RegisterScopedInstanceWithInterfaces(this IContainerBuilder builder, object instance)
        {
            EntryPointsBuilder.EnsureDispatcherRegistered(builder);
            builder.Register(new ScopedInstanceWithInterfacesBuilder(instance));
        }

        public static void RegisterScopedInstance<T>(this IContainerBuilder builder, T instance) where T : class
        {
            builder.Register(new ScopedInstanceBuilder(instance));
        }

        #region Animation

        public static void RegisterViewAnimation<TView, TAnimation>(this IContainerBuilder builder)
            where TView : IView
            where TAnimation : IAnimation<TView>
        {
            builder.Register<IAnimation<TView>, TAnimation>(Lifetime.Singleton);
        }

        public static void RegisterViewDefaultAnimation<TAnimation>(this IContainerBuilder builder)
            where TAnimation : IAnimation<IView>
        {
            builder.Register<IAnimation<IView>, TAnimation>(Lifetime.Singleton);
        }

        //TODO: IAnimation<IView> Ignore
        public static void RegisterViewsAnimation<TAnimation>(this IContainerBuilder builder)
            where TAnimation : IAnimation
        {
            builder.Register(new AnimationViewsBuilder(typeof(TAnimation), Lifetime.Singleton));
            // builder.Register<TAnimation>(Lifetime.Singleton).AsImplementedInterfaces();
        }

        #endregion

        public static void RegisterTransition<TView, TTransition>(this IContainerBuilder builder)
            where TView : IView
            where TTransition : ITransition<TView>
        {
            builder.Register<ITransition<TView>, TTransition>(Lifetime.Singleton);
        }

        public static RegistrationBuilder RegisterGroup<TGroup>(this IContainerBuilder builder)
            where TGroup : IGroup
        {
            return builder.Register<TGroup>(Lifetime.Singleton).AsSelf();
        }

        public static RegistrationBuilder RegisterGroup<TInterface, TGroup>(this IContainerBuilder builder)
            where TInterface : IGroup
            where TGroup : TInterface
        {
            return builder.Register<TInterface, TGroup>(Lifetime.Singleton);
        }

        #region Module

        public static void RegisterPopupGroup<TShadowView>(this IContainerBuilder builder)
            where TShadowView : class, IView
        {
            builder.RegisterGroup<IPopupGroup, PopupGroup<TShadowView>>();
        }

        private static void ThrowVFrameSettingsIsNull()
        {
            if (VFrameSettings.Instance == null)
                throw new ArgumentNullException("require VFrameSettings asset to preload assets");
        }

        public static void UseUISystem(this IContainerBuilder builder)
        {
            ThrowVFrameSettingsIsNull();

            var rootCanvasPrefab = VFrameSettings.Instance.RootCanvas;

            RootCanvas instance;
            using (rootCanvasPrefab.InstantiateBefore(out instance))
            {
                Object.DontDestroyOnLoad(instance.gameObject);
            }

            instance.Configure(builder);
            builder.RegisterEntryPoint<UISystem>()
                .AsSelf()
                .As<ISystemContext>()
                .WithParameter(instance);
            builder.RegisterGroup<TransitionGroup>();
            builder.Register<IRouteFilter, TransitionRouteFilter>(Lifetime.Scoped);
            builder.Register<IRouteFilter, GroupRouteFilter>(Lifetime.Scoped);
            builder.RegisterEntryPoint<UISystemRootInitializer>();
            builder.RegisterViewAnimation<FadeView, FadeAnimation<FadeView>>();
        }

        public static void UseAudioSystem(this IContainerBuilder builder)
        {
            ThrowVFrameSettingsIsNull();
            var groups = VFrameSettings.Instance.AudioGroups;
            builder.RegisterEntryPoint<AudioSystem>().AsSelf()
                .WithParameter(groups)
                .WithParameter(new Dictionary<string, IAudioSourcePlayer>
                {
                    {"SFX", new PlayOneShotPlayer()},
                    {"BGM", new BGMPlayer()}
                });
        }

        public static void UseAudioSystem(this IContainerBuilder builder, Action<AudioSystemBuilder> configuration)
        {
            ThrowVFrameSettingsIsNull();
            var groups = VFrameSettings.Instance.AudioGroups;
            var registration = builder.RegisterEntryPoint<AudioSystem>().AsSelf().WithParameter(groups);

            var audioSystemBuilder = new AudioSystemBuilder(builder);
            configuration(audioSystemBuilder);
            registration.WithParameter(audioSystemBuilder.Players);
        }

        public readonly struct AudioSystemBuilder
        {
            private readonly IContainerBuilder _builder;

            public readonly Dictionary<string, IAudioSourcePlayer> Players;

            public AudioSystemBuilder(IContainerBuilder builder)
            {
                _builder = builder;
                Players = new Dictionary<string, IAudioSourcePlayer>();
            }

            public void AddPlayer(string mixerName, IAudioSourcePlayer player)
            {
                Players.Add(mixerName, player);
            }

            public void SavePlayerPrefs()
            {
                _builder.Register<AudioSourcePropertyToPlayerPrefs>(Lifetime.Singleton)
                    .As<IAudioSourcePropertyReader>()
                    .As<IAudioSourcePropertyWriter>();
            }
        }


        public static void UseUISystemModule(this IContainerBuilder builder,
            Action<UISystemModuleBuilder> configuration)
        {
            configuration(new UISystemModuleBuilder(builder));
        }

        public readonly struct UISystemModuleBuilder
        {
            private readonly IContainerBuilder _builder;
            public UISystemModuleBuilder(IContainerBuilder builder) => _builder = builder;

            public void UseWindow()
            {
                _builder.RegisterGroup<WindowGroup>();
            }

            public void UseWindow<TAnimation>() where TAnimation : IAnimation<IWindowView>
            {
                _builder.RegisterGroup<WindowGroup>();
                _builder.Register<IAnimation<IWindowView>, TAnimation>(Lifetime.Singleton);
            }

            public void UsePopup<TShadowView>() where TShadowView : class, IView
            {
                _builder.RegisterPopupGroup<TShadowView>();
            }

            public void UsePopup<TShadowView, TAnimation>()
                where TShadowView : class, IView
                where TAnimation : IAnimation<IPopupView>
            {
                _builder.RegisterPopupGroup<TShadowView>();
                _builder.Register<IAnimation<IPopupView>, TAnimation>(Lifetime.Singleton);
            }

            public void UseToast()
            {
                _builder.RegisterGroup<ToastGroup>();
            }

            public void UseToast<TAnimation>() where TAnimation : ToastAnimationBase, IAnimation<IToastView>
            {
                _builder.RegisterGroup<ToastGroup>();
                _builder.Register<IAnimation<IToastView>, TAnimation>(Lifetime.Singleton);
            }
        }

        public static void AddTab<TParent>(this IContainerBuilder builder,
            Action<UISystemTabBuilder<TParent>> configuration) where TParent : class, IView
        {
            builder.RegisterGroup<TabGroup<TParent>>();
            builder.Register<IViewMatcher<IGroup>, GroupMatcher<TParent, TabGroup<TParent>>>(Lifetime.Scoped);
            configuration(new UISystemTabBuilder<TParent>(builder));
        }

        public readonly struct UISystemTabBuilder<TParent> where TParent : class, IView
        {
            private readonly IContainerBuilder _builder;
            public UISystemTabBuilder(IContainerBuilder builder) => _builder = builder;

            public void AddChild<TChild>() where TChild : class, IView
            {
                _builder.Register<ITabChild<TParent>, TabChildInstance<TParent, TChild>>(Lifetime.Scoped);
                _builder.Register<IViewMatcher<IGroup>, GroupMatcher<TChild, TabGroup<TParent>>>(Lifetime.Scoped);
            }
        }

        #endregion


        public static void UseVirtualView(this IContainerBuilder builder, Action<VirtualViewBuilder> configuration)
        {
            configuration(new VirtualViewBuilder(builder));
        }

        public readonly struct VirtualViewBuilder
        {
            private readonly IContainerBuilder _builder;
            public VirtualViewBuilder(IContainerBuilder builder) => _builder = builder;

            public void Add<TView, TVirtualView>()
                where TView : class, IView
                where TVirtualView : VirtualView<TView>
            {
                _builder.Register<TVirtualView>(Lifetime.Scoped).AsSelf();
            }

            public void Add<TView, TVirtualView, TTransition>()
                where TView : class, IView
                where TVirtualView : VirtualView<TView>
                where TTransition : class, ITransition<TVirtualView>
            {
                _builder.Register<TVirtualView>(Lifetime.Scoped).AsSelf();
                _builder.RegisterTransition<TVirtualView, TTransition>();
                _builder.Register<
                    IViewMatcher<ITransition>,
                    TransitionMatcher<TVirtualView, ITransition<TVirtualView>>
                >(Lifetime.Scoped);
            }
        }
    }

    public class AnimationViewsBuilder : RegistrationBuilder
    {
        private static readonly Type AnimationType = typeof(IAnimation);
        private static readonly Type ViewAnimationType = typeof(IAnimation<IView>);

        public AnimationViewsBuilder(Type implementationType, Lifetime lifetime) : base(implementationType, lifetime)
        {
            var types = implementationType.GetInterfaces();
            foreach (var type in types)
            {
                if (type == AnimationType || type == ViewAnimationType) continue;
                As(type);
            }
        }
    }

    /// <summary>
    /// RegisterInstance(Lifetime.Scoped).AsImplementedInterfaces()
    /// </summary>
    public class ScopedInstanceWithInterfacesBuilder : RegistrationBuilder, IRegistration
    {
        readonly object implementationInstance;

        public ScopedInstanceWithInterfacesBuilder(object implementationInstance)
            : base(implementationInstance.GetType(), Lifetime.Scoped)
        {
            this.implementationInstance = implementationInstance;
            var interfaceTypes = new List<Type> {ImplementationType};
            interfaceTypes.AddRange(ImplementationType.GetInterfaces());
            InterfaceTypes = interfaceTypes;
        }

        public override IRegistration Build() => this;

        public object SpawnInstance(IObjectResolver resolver) => implementationInstance;

        public Type ImplementationType => implementationInstance.GetType();

        public IReadOnlyList<Type> InterfaceTypes { get; }

        public Lifetime Lifetime { get; }
    }

    /// <summary>
    /// RegisterInstance(Lifetime.Scoped)
    /// </summary>
    public class ScopedInstanceBuilder : RegistrationBuilder, IRegistration
    {
        readonly object implementationInstance;

        public ScopedInstanceBuilder(object implementationInstance)
            : base(implementationInstance.GetType(), Lifetime.Scoped)
        {
            this.implementationInstance = implementationInstance;
        }

        public override IRegistration Build() => this;

        public object SpawnInstance(IObjectResolver resolver) => implementationInstance;

        public Type ImplementationType => implementationInstance.GetType();

        public IReadOnlyList<Type> InterfaceTypes { get; }

        public Lifetime Lifetime { get; }
    }
}