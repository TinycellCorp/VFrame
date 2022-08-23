using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace VFrame.Scene
{
    public class NamedSceneScope : LifetimeScope
    {
        public string nameTag;

        protected override void Configure(IContainerBuilder builder)
        {
            var targetName = string.IsNullOrEmpty(nameTag) ? gameObject.scene.name : nameTag;
            Debug.Log(targetName);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("Scenes/SubScene", LoadSceneMode.Additive);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.UnloadSceneAsync("Scenes/SubScene");
            }
        }
    }
}