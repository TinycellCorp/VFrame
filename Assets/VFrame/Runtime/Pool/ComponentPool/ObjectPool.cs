//TODO: Version Defines - UniRx
namespace VFrame.Pool.ComponentPool
{
#if UNITY_5_3_OR_NEWER

    using System;
    using System.Collections.Generic;

    namespace UniRx.Toolkit
    {
        /// <summary>
        /// Bass class of ObjectPool.
        /// </summary>
        public abstract class ObjectPool<T> : IDisposable where T : UnityEngine.Component
        {
            bool _isDisposed = false;
            Queue<T> _q;

            /// <summary>
            /// Limit of instace count.
            /// </summary>
            protected int MaxPoolCount
            {
                get { return int.MaxValue; }
            }

            /// <summary>
            /// Create instance when needed.
            /// </summary>
            protected abstract T CreateInstance();

            /// <summary>
            /// Called before return to pool, useful for set active object(it is default behavior).
            /// </summary>
            protected virtual void OnBeforeRent(T instance)
            {
                instance.gameObject.SetActive(true);
            }

            /// <summary>
            /// Called before return to pool, useful for set inactive object(it is default behavior).
            /// </summary>
            protected virtual void OnBeforeReturn(T instance)
            {
                instance.gameObject.SetActive(false);
            }

            /// <summary>
            /// Called when clear or disposed, useful for destroy instance or other finalize method.
            /// </summary>
            protected virtual void OnClear(T instance)
            {
                if (instance == null) return;

                var go = instance.gameObject;
                if (go == null) return;
                UnityEngine.Object.Destroy(go);
            }

            /// <summary>
            /// Current pooled object count.
            /// </summary>
            public int Count
            {
                get
                {
                    if (_q == null) return 0;
                    return _q.Count;
                }
            }

            /// <summary>
            /// Get instance from pool.
            /// </summary>
            public T Rent()
            {
                if (_isDisposed) throw new ObjectDisposedException("ObjectPool was already disposed.");
                if (_q == null) _q = new Queue<T>();

                var instance = (_q.Count > 0)
                    ? _q.Dequeue()
                    : CreateInstance();

                OnBeforeRent(instance);
                return instance;
            }

            /// <summary>
            /// Return instance to pool.
            /// </summary>
            public void Return(T instance)
            {
                if (_isDisposed) throw new ObjectDisposedException("ObjectPool was already disposed.");
                if (instance == null) throw new ArgumentNullException("instance");

                if (_q == null) _q = new Queue<T>();

                if ((_q.Count + 1) == MaxPoolCount)
                {
                    throw new InvalidOperationException("Reached Max PoolSize");
                }

                OnBeforeReturn(instance);
                _q.Enqueue(instance);
            }

            /// <summary>
            /// Clear pool.
            /// </summary>
            public void Clear(bool callOnBeforeRent = false)
            {
                if (_q == null) return;
                while (_q.Count != 0)
                {
                    var instance = _q.Dequeue();
                    if (callOnBeforeRent)
                    {
                        OnBeforeRent(instance);
                    }

                    OnClear(instance);
                }
            }

            /// <summary>
            /// Trim pool instances. 
            /// </summary>
            /// <param name="instanceCountRatio">0.0f = clear all ~ 1.0f = live all.</param>
            /// <param name="minSize">Min pool count.</param>
            /// <param name="callOnBeforeRent">If true, call OnBeforeRent before OnClear.</param>
            public void Shrink(float instanceCountRatio, int minSize, bool callOnBeforeRent = false)
            {
                if (_q == null) return;

                if (instanceCountRatio <= 0) instanceCountRatio = 0;
                if (instanceCountRatio >= 1.0f) instanceCountRatio = 1.0f;

                var size = (int) (_q.Count * instanceCountRatio);
                size = Math.Max(minSize, size);

                while (_q.Count > size)
                {
                    var instance = _q.Dequeue();
                    if (callOnBeforeRent)
                    {
                        OnBeforeRent(instance);
                    }

                    OnClear(instance);
                }
            }

            #region IDisposable Support

            protected virtual void Dispose(bool disposing)
            {
                if (!_isDisposed)
                {
                    if (disposing)
                    {
                        Clear(false);
                    }

                    _isDisposed = true;
                }
            }

            public void Dispose()
            {
                Dispose(true);
            }

            #endregion
        }
       
    }

#endif
}