using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VFrame.UI.Context;

namespace VFrame.UI.View
{
    public abstract class VirtualView<TView> : IView where TView : class, IView
    {
        private readonly Func<TView> _viewResolver;
        protected TView View => _viewResolver();

        protected VirtualView(ISystemContext system)
        {
            _viewResolver = system.ResolveView<TView>;
        }

        public RectTransform Rect => View.Rect;

        public float Alpha
        {
            get => View.Alpha;
            set => View.Alpha = value;
        }

        public bool IsActive
        {
            get => View.IsActive;
            set => View.IsActive = value;
        }

        public bool IsInteractable
        {
            get => View.IsInteractable;
            set => View.IsInteractable = value;
        }

        public virtual UniTask Ready()
        {
            return View.Ready();
        }

        public virtual void OnEnter() => View.OnEnter();
        public virtual void OnExit() => View.OnExit();
    }
}