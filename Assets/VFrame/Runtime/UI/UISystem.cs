using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VFrame.UI.Animation;
using VFrame.UI.Command;
using VFrame.UI.Command.Route;
using VFrame.UI.Context;
using VFrame.UI.Extension;
using VFrame.UI.Group;
using VFrame.UI.Layer;
using VFrame.UI.Transition;
using VFrame.UI.View;
using UnityEngine.EventSystems;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace VFrame.UI
{
    public partial class UISystem : IDisposable, IInitializable, ISystemContext
    {
        private static UISystem _sharedInstance;

        // private static LifetimeScope _uiScope;
        private static IObjectResolver _container;
        private static RootCanvas _rootCanvas;

#if UNITY_2021_2_OR_NEWER
        private static readonly Dictionary<UnityEngine.SceneManagement.Scene, Queue<ComponentView>> RegisteredViews =
            new();

        private readonly LifetimeScope _rootScope;
        private readonly Dictionary<Type, ILayer> _layers = new();
        private readonly Dictionary<IView, IGroup> _groups = new();
        private readonly Dictionary<IView, IAnimation> _animations = new();
        private readonly Dictionary<IView, ITransition> _transitions = new();
#else
        private static readonly Dictionary<UnityEngine.SceneManagement.Scene, Queue<ComponentView>> RegisteredViews =
            new Dictionary<UnityEngine.SceneManagement.Scene, Queue<ComponentView>>();

        private readonly LifetimeScope _rootScope;
        private readonly Dictionary<Type, ILayer> _layers = new Dictionary<Type, ILayer>();
        private readonly Dictionary<IView, IGroup> _groups = new Dictionary<IView, IGroup>();
        private readonly Dictionary<IView, IAnimation> _animations = new Dictionary<IView, IAnimation>();
        private readonly Dictionary<IView, ITransition> _transitions = new Dictionary<IView, ITransition>();
#endif

        UISystem ISystemContext.System => this;

        public UISystem(LifetimeScope rootScope, RootCanvas rootCanvas)
        {
            _sharedInstance = this;
            _rootScope = rootScope;
            _rootCanvas = rootCanvas;
            SystemReadySource.TrySetResult();
        }


        public void Initialize()
        {
        }

    


        private static IAnimation GetDefaultAnimation(IObjectResolver resolver)
        {
            return resolver.Resolve<IAnimation<IView>>();
        }

        public void Dispose()
        {
            _container = null;
            _sharedInstance = null;
        }

        #region Static Methods

        public static void RootConfigure(UnityEngine.SceneManagement.Scene rootScene, IContainerBuilder builder)
        {
            if (!RegisteredViews.TryGetValue(rootScene, out var globalViews)) return;

            while (globalViews.Any())
            {
                var view = globalViews.Dequeue();
                builder.RegisterScopedInstanceWithInterfaces(view);
            }

            RegisteredViews.Remove(rootScene);
        }

        public static void Configure(LifetimeScope sceneScope, IContainerBuilder builder)
        {
            // _uiScope = sceneScope;
            var scene = sceneScope.gameObject.scene;
            if (!RegisteredViews.TryGetValue(scene, out var views)) return;
            while (views.Any())
            {
                var view = views.Dequeue();
                builder.RegisterScopedInstanceWithInterfaces(view);
            }

            RegisteredViews.Remove(scene);

            ConfigureDefaultAnimation(sceneScope);

            PlayCommands();
            // PlayCommandsAsync().Forget();

            void ConfigureDefaultAnimation(LifetimeScope scope)
            {
                try
                {
                    var defaultAnimation = GetDefaultAnimation(scope.Parent.Container);
                }
                catch (VContainerException)
                {
                    builder.Register<IAnimation<IView>, ToggleAnimation>(Lifetime.Singleton);
                }
            }
        }

        public static void Initialize(IObjectResolver resolver, bool isRoot = false)
        {
            InitializeMatcher(resolver);
            if (!isRoot)
            {
                _container = resolver;
                _containerReadyTask.TrySetResult();
            }
            PlayCommands();
        }
        public static void Clear(LifetimeScope scope)
        {
            var scene = scope.gameObject.scene;
            
            _containerReadyTask = new UniTaskCompletionSource();
            _container = null;

            _entryView = null;
        }
        
        private static bool _isBlockingRegisterView = false;

        private readonly struct BlockingRegisterViewHandler : IDisposable
        {
            public void Dispose()
            {
                _isBlockingRegisterView = false;
            }
        }

        private static BlockingRegisterViewHandler EnableBlockingRegisterView()
        {
            _isBlockingRegisterView = true;
            return new BlockingRegisterViewHandler();
        }

        private static void RegisterView(ComponentView view)
        {
            if (_isBlockingRegisterView) return;

            var scene = view.gameObject.scene;
            if (!RegisteredViews.TryGetValue(scene, out var queue))
            {
                queue = new Queue<ComponentView>();
                RegisteredViews.Add(scene, queue);
            }

            queue.Enqueue(view);
        }


        public static void Register(ComponentView view)
        {
            RegisterView(view);
        }

        public static void Register<TView>(ComponentView<TView> view) where TView : IView
        {
            RegisterView(view);
            ExecuteCacheCommand(new MergeCommand(
                new CacheAnimationCommand<TView>(view),
                new CacheTransitionCommand<TView>(view),
                new CacheGroupCommand<TView>(view)
            ));
        }


        public static void Unregister(ComponentView view)
        {
            _sharedInstance?._animations.SafeRemove(view);
            _sharedInstance?._transitions.SafeRemove(view);
            _sharedInstance?._groups.SafeRemove(view);
        }


        private static void Register<TView, TGroup>(ComponentView<TView> view)
            where TView : IView
            where TGroup : class, IGroup
        {
            RegisterView(view);
            ExecuteCacheCommand(new MergeCommand(
                new CacheAnimationCommand<TView>(view),
                new CacheTransitionCommand<TView>(view),
                new CacheGroupCommand<TView, TGroup>(view)
            ));
        }

        public static void RegisterGroupWithAnimation<TView, TGroup, TAnimation>(ComponentView<TView> view)
            where TView : IView
            where TGroup : class, IGroup
            where TAnimation : class, IAnimation
        {
            RegisterView(view);
            ExecuteCacheCommand(new MergeCommand(
                new CacheAnimationCommand<TView, TAnimation>(view),
                new CacheTransitionCommand<TView>(view),
                new CacheGroupCommand<TView, TGroup>(view)
            ));
        }

        public static void Register<TView, TLayer, TGroup>(ComponentView<TView> view)
            where TView : IView
            where TLayer : class, ILayer
            where TGroup : class, IGroup
        {
            Register<TView, TGroup>(view);
            ExecuteCacheCommand(new CacheLayerCommand<TLayer>(view));
        }

        public static void Register<TView, TLayer, TGroup, TAnimation>(ComponentView<TView> view)
            where TView : IView
            where TLayer : class, ILayer
            where TGroup : class, IGroup
            where TAnimation : class, IAnimation
        {
            RegisterView(view);
            ExecuteCacheCommand(new MergeCommand(
                new CacheAnimationCommand<TView, TAnimation>(view),
                new CacheTransitionCommand<TView>(view),
                new CacheGroupCommand<TView, TGroup>(view),
                new CacheLayerCommand<TLayer>(view)
            ));
        }


        public static void Register<TLayer>(TLayer layer) where TLayer : class, ILayer
        {
            EnqueueCommand(new RegisterLayerCommand<TLayer>(layer));
        }

        public static void Unregister<TLayer>(TLayer layer) where TLayer : class, ILayer
        {
            if (_sharedInstance == null) return;

            var layerType = typeof(TLayer);
            var layers = _sharedInstance._layers;
            if (layers.ContainsKey(layerType))
            {
                layers.Remove(layerType);
            }
        }

        #endregion

        T ISystemContext.Resolve<T>() => _container.Resolve<T>();


        T ISystemContext.ResolveView<T>()
        {
            var view = _container.Resolve<T>();
            _container.Inject(view);
            return view;
        }

        IAnimation ISystemContext.ResolveAnimation<T>()
        {
            try
            {
                var animation = _container.Resolve<IAnimation<T>>();
                var view = _container.Resolve<T>();

                if (!_animations.ContainsKey(view))
                {
                    _animations.Add(view, animation);
                }

                return animation;
            }
            catch (VContainerException e)
            {
                return GetDefaultAnimation(_container);
            }
        }

        IAnimation ISystemContext.ResolveAnimation(IView view)
        {
            if (_animations.TryGetValue(view, out var animation))
            {
                return animation;
            }
            else
            {
                return GetDefaultAnimation(_container);
            }
        }

        ITransition ISystemContext.ResolveTransition<T>()
        {
            try
            {
                var transition = _container.Resolve<ITransition<T>>();
                var view = _container.Resolve<T>();

                if (!_transitions.ContainsKey(view))
                {
                    _transitions.Add(view, transition);
                }

                return transition;
            }
            catch (VContainerException e)
            {
                throw new Exception($"not found transition: {typeof(T).Name}");
            }
        }

        ITransition ISystemContext.ResolveTransition(IView view)
        {
            if (_transitions.TryGetValue(view, out var transition))
            {
                return transition;
            }

            throw new Exception($"not found transition: {view.GetType().Name}");
        }

        bool ISystemContext.HasTransition(IView view)
        {
            return _transitions.ContainsKey(view);
        }

        IGroup ISystemContext.ResolveGroup<TGroup>()
        {
            return _container.Resolve<TGroup>();
        }

        bool ISystemContext.TryResolveGroup(IView view, out IGroup @group)
        {
            if (!_groups.TryGetValue(view, out group))
            {
                ConsumeMatcher(view, _groups, out group);
            }

            return group != null;
        }

        private IReadOnlyList<IRouteFilter> _filters;

        IReadOnlyList<IRouteFilter> ISystemContext.ResolveRouteFilters()
        {
            return _filters ??= _rootScope.Container.Resolve<IReadOnlyList<IRouteFilter>>();
        }

        ViewAnimator ISystemContext.ResolveAnimator<TView>()
        {
            var context = this as ISystemContext;
            return new ViewAnimator(
                context.ResolveView<TView>(),
                context.ResolveAnimation<TView>()
            );
        }

        ViewAnimator ISystemContext.ResolveAnimator(IView view)
        {
            var context = this as ISystemContext;
            return new ViewAnimator(
                view,
                context.ResolveAnimation(view)
            );
        }


        #region Util

        public readonly struct InteractableDisableHandler : IDisposable
        {
            private readonly IView _view;

            public InteractableDisableHandler(IView view)
            {
                _view = view;
                _view.IsInteractable = false;
            }

            public void Dispose()
            {
                _view.IsInteractable = true;
            }
        }


        private static EventSystem _eventSystem;

        public static EventSystemDisableHandler DisableEventSystem()
        {
            if (ReferenceEquals(_eventSystem, null))
            {
                _eventSystem = Object.FindObjectOfType<EventSystem>();
            }

            _eventSystem.enabled = false;
            return new EventSystemDisableHandler();
        }

        public static InteractableDisableHandler DisableInteractable(IView view)
        {
            return new InteractableDisableHandler(view);
        }

        public readonly struct EventSystemDisableHandler : IDisposable
        {
            public void Dispose()
            {
                _eventSystem.enabled = true;
            }
        }

        private class DisableBlockingCommand : ICommand
        {
            public UniTask Execute(ISystemContext context)
            {
                _isBlocking = false;
                return UniTask.CompletedTask;
            }
        }

        #endregion

        #region System Commands

        private abstract class CacheCommandBase : ICommand
        {
            protected IView View { get; private set; }
            protected CacheCommandBase(IView view) => (View) = (view);

            protected IObjectResolver Container => _container;
            public abstract UniTask Execute(ISystemContext context);
        }

        private abstract class CacheGroupCommandBase : CacheCommandBase
        {
            protected Dictionary<IView, IGroup> Groups(ISystemContext context) => context.System._groups;

            protected CacheGroupCommandBase(IView view) : base(view)
            {
            }
        }

        private class CacheGroupCommand<TView> : CacheGroupCommandBase where TView : IView
        {
            public CacheGroupCommand(IView view) : base(view)
            {
            }

            public override UniTask Execute(ISystemContext context)
            {
                try
                {
                    var group = Container.Resolve<IGroup<TView>>();
                    var groups = Groups(context);
                    if (!groups.ContainsKey(View))
                    {
                        groups.Add(View, group);
                    }
                }
                catch
                {
                    // ignored
                }

                return UniTask.CompletedTask;
            }
        }

        private class CacheGroupCommand<TView, TGroup> : CacheGroupCommandBase
            where TView : IView where TGroup : class, IGroup
        {
            public CacheGroupCommand(IView view) : base(view)
            {
            }

            public override UniTask Execute(ISystemContext context)
            {
                try
                {
                    var group = Container.Resolve<TGroup>();
                    var groups = Groups(context);
                    if (!groups.ContainsKey(View))
                    {
                        groups.Add(View, group);
                    }
                }
                catch
                {
                    // ignored
                }

                return UniTask.CompletedTask;
            }
        }


        private abstract class CacheAnimationCommandBase : CacheCommandBase
        {
            protected Dictionary<IView, IAnimation> Animations(ISystemContext context) => context.System._animations;

            protected CacheAnimationCommandBase(IView view) : base(view)
            {
            }
        }

        private class CacheAnimationCommand<TView> : CacheAnimationCommandBase where TView : IView
        {
            public CacheAnimationCommand(IView view) : base(view)
            {
            }

            public override UniTask Execute(ISystemContext context)
            {
                try
                {
                    var animation = Container.Resolve<IAnimation<TView>>();
                    var animations = Animations(context);
                    if (!animations.ContainsKey(View))
                    {
                        animations.Add(View, animation);
                    }
                }
                catch
                {
                    // ignored
                }

                return UniTask.CompletedTask;
            }
        }

        private class CacheAnimationCommand<TView, TAnimation> : CacheAnimationCommandBase
            where TView : IView
            where TAnimation : class, IAnimation
        {
            public CacheAnimationCommand(IView view) : base(view)
            {
            }

            public override UniTask Execute(ISystemContext context)
            {
                IAnimation animation = null;
                try
                {
                    animation = Container.Resolve<IAnimation<TView>>();
                }
                catch
                {
                    try
                    {
                        animation = Container.Resolve<TAnimation>();
                    }
                    catch
                    {
                        // ignored
                    }
                }

                var animations = Animations(context);
                if (!animations.ContainsKey(View) && animation != null)
                {
                    animations.Add(View, animation);
                }

                return UniTask.CompletedTask;
            }
        }

        private class CacheTransitionCommand<TView> : CacheCommandBase where TView : IView
        {
            private Dictionary<IView, ITransition> Transitions(ISystemContext context) => context.System._transitions;

            public CacheTransitionCommand(IView view) : base(view)
            {
            }

            public override UniTask Execute(ISystemContext context)
            {
                try
                {
                    var transition = Container.Resolve<ITransition<TView>>();
                    var transitions = Transitions(context);
                    if (!transitions.ContainsKey(View))
                    {
                        transitions.Add(View, transition);
                    }
                }
                catch (Exception e)
                {
                    // ignored
                }

                return UniTask.CompletedTask;
            }
        }

        private class CacheLayerCommand<TLayer> : CacheCommandBase where TLayer : class, ILayer
        {
            private Dictionary<Type, ILayer> Layers(ISystemContext context) => context.System._layers;

            public CacheLayerCommand(IView view) : base(view)
            {
            }

            public override UniTask Execute(ISystemContext context)
            {
                var layerType = typeof(TLayer);
                var layers = Layers(context);
                if (layers.TryGetValue(layerType, out var layer))
                {
                    layer.In(View);
                }

                return UniTask.CompletedTask;
            }
        }

        private class RegisterLayerCommand<TLayer> : ICommand where TLayer : ILayer
        {
            private readonly ILayer _layer;
            public RegisterLayerCommand(ILayer layer) => _layer = layer;

            public UniTask Execute(ISystemContext context)
            {
                var type = typeof(TLayer);
                var layers = _sharedInstance._layers;
                if (!layers.ContainsKey(type))
                {
                    layers.Add(type, _layer);
                }

                return UniTask.CompletedTask;
            }
        }

        #endregion

        public class MergeCommand : ICommand
        {
            private readonly ICommand[] _commands;
            public MergeCommand(params ICommand[] commands) => _commands = commands;

            public async UniTask Execute(ISystemContext context)
            {
                foreach (var command in _commands)
                {
                    await command.Execute(context);
                }
            }
        }

        public static async UniTask Show<TView>() where TView : class, IView
        {
            var system = _sharedInstance as ISystemContext;
            var view = system.ResolveView<TView>();
            await view.Show();
        }

        public static void Hide<TView>() where TView : class, IView
        {
            var system = _sharedInstance as ISystemContext;
            var view = system.ResolveView<TView>();
            view.Hide();
        }

        public static ViewAnimator Animator<TView>() where TView : class, IView
        {
            var system = _sharedInstance as ISystemContext;
            var view = system.ResolveView<TView>();
            return system.ResolveAnimator(view);
        }
    }
}