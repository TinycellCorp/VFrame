using Cysharp.Threading.Tasks;
using VFrame.UI.Context;
using VFrame.UI.Extension;
using UnityEngine.SceneManagement;
using VFrame.UI.Animation;
using VFrame.UI.Transition;


namespace VFrame.UI.External
{
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

                var operation = SceneManager.LoadSceneAsync(_sceneName);
                await operation.ToUniTask();

                operation.Active();
            }

            await UISystem.Ready;
            fade.Out();
            // UISystem.WaitUntilNextEntry().ContinueWith(() => { fade.Out(); });
        }

        public UniTask Out(ISystemContext context, ITransitionJob job) => UniTask.CompletedTask;
    }
}