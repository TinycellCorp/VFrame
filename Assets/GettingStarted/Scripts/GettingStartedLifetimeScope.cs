using GettingStarted.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VFrame.Scene;
using VFrame.UI;

namespace GettingStarted
{
    public class GettingStartedLifetimeScope : SceneScope<GettingStartedScene>
    {
    }

    public class GettingStartedScene : SceneEntry, ITickable
    {
        public GettingStartedScene(IObjectResolver resolver) : base(resolver)
        {
        }

        public override void Initialize()
        {
            UISystem.Entry<FirstView>();
        }

        protected override void OnDisposed()
        {
        }

        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                UISystem.To<SecondView>();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UISystem.Back();
            }
        }
    }
}