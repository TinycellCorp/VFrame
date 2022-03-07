using Cysharp.Threading.Tasks;
using UnityEngine;
using VFrame.UI.Context;
using VFrame.UI.Extension;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using VFrame.Scene;
using VFrame.UI.Animation;
using VFrame.UI.Transition;


namespace VFrame.UI.External
{
    public class FadeOutAfterPreload : IPostProcessPreload
    {
        private readonly ViewAnimator _animator;

        public FadeOutAfterPreload(ViewAnimator animator)
        {
            _animator = animator;
        }

        public UniTask OnPostProcessPreload() => _animator.Out();
    }

    public class LoadSceneTransition : ITransition
    {
        private readonly string _sceneName;
        public LoadSceneTransition(string sceneName) => _sceneName = sceneName;

        public async UniTask In(ISystemContext context, ITransitionJob job)
        {
            var fade = context.ResolveAnimator<FadeView>();

            await fade.In();
            // await using (await fade.Scope()) TODO: UNITY_2021_2_OR_NEWER System.IAsyncDisposable
            {
                await job.Execute();

                using (LifetimeScope.Enqueue(
                           builder => builder.RegisterInstance<IPostProcessPreload>(new FadeOutAfterPreload(fade))
                       ))
                {
                    var operation = SceneManager.LoadSceneAsync(_sceneName);
                    await operation.ToUniTask();
                    operation.Active();
                }
            }
            await UISystem.Ready;
            // fade.Out();
        }

        public UniTask Out(ISystemContext context, ITransitionJob job) => UniTask.CompletedTask;
    }
}