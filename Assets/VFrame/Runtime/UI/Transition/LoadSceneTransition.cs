using System;
using Cysharp.Threading.Tasks;
using VFrame.UI.Context;
using VFrame.UI.Extension;
using VFrame.UI.View;
using UnityEngine;
using UnityEngine.SceneManagement;
using VFrame.UI.Animation;


namespace VFrame.UI.Transition
{
    public class LoadSceneTransition : ITransition
    {
        private readonly string _sceneName;
        public LoadSceneTransition(string sceneName) => _sceneName = sceneName;

        public async UniTask In(ISystemContext context, ITransitionExecutor executor)
        {
            var fade = context.ResolveAnimator<FadeView>();

            await fade.In();
            // await using (await fade.Scope()) TODO: Defeine 2021_2
            {
                // await UniTask.Delay(TimeSpan.FromSeconds(2));
                // Debug.Log("executor");
                await executor.Execute();

                // await UniTask.Delay(TimeSpan.FromSeconds(1));
                // Debug.Log("load scene");
                var operation = SceneManager.LoadSceneAsync(_sceneName);
                await operation.ToUniTask();

                // Debug.Log("active");
                operation.Active();
                // await UniTask.Delay(TimeSpan.FromSeconds(2));
            }
            FadeOut(fade).Forget();
        }

        private async UniTaskVoid FadeOut(ViewAnimator fade)
        {
            await UniTask.WaitUntil(() => UISystem.IsVisibleEntryView());
            fade.Out();
        }

        public UniTask Out(ISystemContext context, ITransitionExecutor executor) => UniTask.CompletedTask;
    }
}