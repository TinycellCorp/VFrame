using System;
using Cysharp.Threading.Tasks;
using VContainer;
using VFrame.Extension;
using VFrame.Pool;
using VFrame.Pool.ComponentPool.UniRx.Toolkit;
using VFrame.UI.Context;
using VFrame.UI.View;

namespace VFrame.UI.Pool
{
    public interface IViewPool<TView> where TView : IView
    {
        TView Rent(IView source);
        void Return(TView instance);
    }

    public class ComponentViewPool<TView> : ObjectPool<TView>, IViewPool<TView> where TView : ComponentView
    {
        private readonly IObjectResolver _resolver;

        public TView Source { set; private get; }

        public ComponentViewPool(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        protected override TView CreateInstance()
        {
            if (Source != null)
            {
                using (Source.InstantiateBefore(out TView view, Source.transform.parent))
                {
                    _resolver.Inject(view);
                    return view;
                }
            }

            throw new ArgumentNullException(nameof(Source));
        }

        public TView Rent(IView source)
        {
            if (Source == null)
            {
                Source = source as TView;
                return Source;
            }
            else
            {
                return base.Rent();
            }
        }
    }
}