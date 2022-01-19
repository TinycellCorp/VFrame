using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using VFrame.UI.Context;
using VFrame.UI.Extension;
using VFrame.UI.Transition;

namespace VFrame.UI.External
{
    public class LoadSubSceneTransition : ITransition
    {
        private readonly string _sceneName;
        public LoadSubSceneTransition(string sceneName) => _sceneName = sceneName;

        public async UniTask In(ISystemContext context, ITransitionJob job)
        {
            var fade = context.ResolveAnimator<FadeView>();

            await fade.In();
            await job.Execute();

            var operation = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
            await operation.ToUniTask();
            operation.Active();

            fade.Out();
        }

        public UniTask Out(ISystemContext context, ITransitionJob job) => UniTask.CompletedTask;
    }
}