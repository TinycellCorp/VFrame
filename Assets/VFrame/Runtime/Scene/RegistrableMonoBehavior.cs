using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace VFrame.Scene
{
    [DefaultExecutionOrder(-5002)]
    public class RegistrableMonoBehavior<TComponent> : MonoBehaviour, SceneScope.IReserveRegistrable
        where TComponent : MonoBehaviour
    {
        public string SceneName => gameObject.scene.name;

        public void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent<TComponent>(this as TComponent);
        }

        protected virtual void Awake()
        {
            SceneScope.Reserve(this);
        }
    }
}