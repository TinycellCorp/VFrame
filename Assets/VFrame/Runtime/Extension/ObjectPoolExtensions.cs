using UnityEngine;
using VFrame.Pool.ComponentPool.UniRx.Toolkit;

namespace VFrame.Extension
{
    public static class ObjectPoolExtensions
    {
        public static void ChangeParent<T>(this ObjectPool<T> pool, T instance, Transform parent)
            where T : Component
        {
            if (!ReferenceEquals(parent, null) && !ReferenceEquals(parent, instance.transform.parent))
            {
                instance.transform.SetParent(parent);
            }
        }
    }
}