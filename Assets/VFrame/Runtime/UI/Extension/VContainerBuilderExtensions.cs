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
using VFrame.Core;
using VFrame.UI.External;
using Object = UnityEngine.Object;

namespace VFrame.UI.Extension
{
    public static class VContainerBuilderExtensions
    {
        public static void RegisterScopeInstance(this IContainerBuilder builder, object instance)
        {
            EntryPointsBuilder.EnsureDispatcherRegistered(builder);
            builder.Register(new ScopeInstanceBuilder(instance));
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

        public static void UseUISystem(this IContainerBuilder builder)
        {
            if (VFrameSettings.Instance == null)
                throw new ArgumentNullException("require VFrameSettings asset to preload assets");

            var rootCanvasPrefab = VFrameSettings.Instance.RootCanvas;

            RootCanvas instance;
            using (rootCanvasPrefab.InstantiateBefore(out instance))
            {
                Object.DontDestroyOnLoad(instance.gameObject);
            }

            instance.Configure(builder);
            builder.RegisterEntryPoint<UISystem>().AsSelf().WithParameter(instance);
            builder.RegisterGroup<TransitionGroup>();
            builder.Register<IRouteFilter, TransitionRouteFilter>(Lifetime.Scoped);
            builder.Register<IRouteFilter, GroupRouteFilter>(Lifetime.Scoped);
            builder.RegisterEntryPoint<UISystemRootInitializer>();
            builder.RegisterViewAnimation<FadeView, FadeAnimation<FadeView>>();
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
    /// RegisterInstance(Lifetime.Scoped) 
    /// </summary>
    public class ScopeInstanceBuilder : RegistrationBuilder, IRegistration
    {
        readonly object implementationInstance;

        public ScopeInstanceBuilder(object implementationInstance)
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
}