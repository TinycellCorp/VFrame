using VFrame.UI.View;
using UnityEngine;

namespace VFrame.UI.Layer
{
    
    [DefaultExecutionOrder(-5002)]
    [AddComponentMenu("UISystem/Layer/Canvas Layer")]
    [RequireComponent(typeof(Canvas))]
    public class CanvasLayer : MonoBehaviour, ILayer
    {
        private Canvas _canvas;
        private RectTransform _rect;

        protected Canvas Canvas
        {
            get
            {
                if (ReferenceEquals(_canvas, null))
                {
                    _canvas = GetComponent<Canvas>();
                }

                return _canvas;
            }
        }

        protected RectTransform Rect
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

        protected virtual void Awake()
        {
            Rect.anchoredPosition = Vector2.zero;
        }

        public void In(IView view)
        {
            if (!ReferenceEquals(Rect, view.Rect))
            {
                view.Rect.SetParent(Rect);
            }
        }
    }
}