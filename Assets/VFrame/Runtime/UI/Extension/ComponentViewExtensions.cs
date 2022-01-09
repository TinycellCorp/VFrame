using System;
using VFrame.UI.View;

namespace VFrame.UI.Extension
{
    public static class ComponentViewExtensions
    {
        public static TView As<TView>(this ComponentView<TView> view) where TView : class, IView
        {
            if (view is TView asView)
            {
                return asView;
            }
            else
            {
                throw new InvalidCastException($"{view.GetType().Name} to {typeof(TView).Name}");
            }
        }
        
        
    }
}