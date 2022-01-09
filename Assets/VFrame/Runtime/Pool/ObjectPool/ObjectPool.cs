using System;
using System.Collections.Concurrent;

namespace VFrame.Pool.ObjectPool
{
    public abstract class ObjectPoolBase<T> : IDisposable where T : IDisposable
    {
        private readonly ConcurrentBag<T> _bag = new ConcurrentBag<T>();
        private readonly Func<T> _factory;

        protected ObjectPoolBase(Func<T> factory)
        {
            _factory = factory;
        }

        public T Rent()
        {
            if (!_bag.TryTake(out var result))
            {
                result = _factory.Invoke();
            }

            OnBeforeRent(result);
            return result;
        }

        protected abstract void OnBeforeRent(T instance);
        protected abstract void OnBeforeReturn(T instance);

        public void Return(T instance)
        {
            OnBeforeReturn(instance);
            _bag.Add(instance);
        }

        public void Dispose()
        {
            while (_bag.TryTake(out var result))
            {
                result.Dispose();
            }
        }
    }

    public class ObjectPool<T> : ObjectPoolBase<T>, IObjectPool<T> where T : IPoolable
    {
        public ObjectPool(Func<T> factory) : base(factory)
        {
        }

        protected override void OnBeforeRent(T instance) => instance.Rented();

        protected override void OnBeforeReturn(T instance) => instance.Return();
    }

    public class ObjectPool<TParam, T> : ObjectPoolBase<T>, IObjectPool<TParam, T> where T : IPoolable<TParam>
    {
        public ObjectPool(Func<T> factory) : base(factory)
        {
        }

        public T Rent(TParam param)
        {
            var instance = Rent();
            instance.Rented(param);
            return instance;
        }

        protected override void OnBeforeRent(T instance)
        {
        }

        protected override void OnBeforeReturn(T instance) => instance.Return();
    }
}