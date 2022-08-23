using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using VFrame.UI.Context;
using VFrame.UI.Extension;
using VFrame.UI.Transition;
using VFrame.UI.View;

namespace VFrame.UI.SubScene
{
    public abstract class SubSceneView<TView> : VirtualView<TView>, ISubSceneView where TView : class, IView
    {
        public abstract string SceneName { get; }

        protected SubSceneView(ISystemContext system) : base(system)
        {
        }

        public override async UniTask Ready()
        {
            await UISystem.Ready;
            await base.Ready();
        }

        public class TransitionBase<TSubSceneView> : ITransition<TSubSceneView>
            where TSubSceneView : class, ISubSceneView
        {
            public async UniTask In(ISystemContext context, ITransitionJob job)
            {
                var animator = context.ResolveAnimator<FadeView>();
                await animator.In();

                var view = context.Resolve<TSubSceneView>();
                var operation = SceneManager.LoadSceneAsync(view.SceneName, LoadSceneMode.Additive);
                await operation;
                operation.Active();
                
                await job.Execute();
                await animator.Out();
            }

            public async UniTask Out(ISystemContext context, ITransitionJob job)
            {
                var animator = context.ResolveAnimator<FadeView>();
                await animator.In();
                await job.Execute();

                var view = context.Resolve<TSubSceneView>();
                var operation = SceneManager.UnloadSceneAsync(view.SceneName);
                await operation;

                await animator.Out();
            }
        }
    }
}