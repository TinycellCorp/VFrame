using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VFrame.UI.View
{
    [DefaultExecutionOrder(-5001)]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class ComponentView : MonoBehaviour, IView
    {
        private RectTransform _rect;

        public RectTransform Rect
        {
            get
            {
                if (ReferenceEquals(_rect, null))
                {
                    _rect = GetComponent<RectTransform>();
                }

                return _rect;
            }
        }

        private CanvasGroup _canvasGroup;

        private CanvasGroup CanvasGroup
        {
            get
            {
                if (ReferenceEquals(_canvasGroup, null))
                {
                    _canvasGroup = GetComponent<CanvasGroup>();
                }

                return _canvasGroup;
            }
        }

        public float Alpha
        {
            get => CanvasGroup.alpha;
            set => CanvasGroup.alpha = value;
        }

        private bool _isActive = false;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
// #if UNITY_EDITOR
//                 gameObject.SetActive(value);
// #endif
                if (!_isActive)
                {
                    CanvasGroup.alpha = 0;
                    Rect.anchoredPosition = _initPosition;
                }

                if (_initInteractable) CanvasGroup.interactable = value;
                if (_initBlockRaycasts) CanvasGroup.blocksRaycasts = value;
            }
        }

        public bool IsInteractable
        {
            get => _canvasGroup.interactable;
            set => _canvasGroup.interactable = value;
        }

        private Vector2 _initPosition;
        private bool _initInteractable;
        private bool _initBlockRaycasts;

        protected virtual void Awake()
        {
            UISystem.Register(this);
            Init();
        }

        protected void Init()
        {
            _initPosition = Rect.anchoredPosition;
            _initInteractable = CanvasGroup.interactable;
            _initBlockRaycasts = CanvasGroup.blocksRaycasts;
            IsActive = _isActive;
        }

        public virtual UniTask Ready()
        {
            PositionZero();
            Alpha = 1;
            return UniTask.CompletedTask;
        }

        protected void PositionZero()
        {
            transform.localScale = Vector3.one;
            Rect.anchoredPosition = Vector2.zero;
        }

        public abstract void OnEnter();
        public abstract void OnExit();

        protected virtual void OnDestroy()
        {
            UISystem.Unregister(this);
        }
    }

    public abstract class ComponentView<TView> : ComponentView where TView : IView
    {
        protected override void Awake()
        {
            UISystem.Register<TView>(this);
            Init();
        }
    }
}