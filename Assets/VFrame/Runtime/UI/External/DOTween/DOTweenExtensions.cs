using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using VFrame.UI.View;
using UnityEngine;

namespace VFrame.UI.External
{
    public static class DOTweenExtensions
    {
        public static TweenerCore<Vector3, Vector3, VectorOptions> DOAnchoredMoveX(
            this RectTransform target,
            float endValue,
            float duration,
            bool snapping = false)
        {
            
            TweenerCore<Vector3, Vector3, VectorOptions> t = DOTween.To(
                (DOGetter<Vector3>) (() => target.anchoredPosition),
                (DOSetter<Vector3>) (x => target.anchoredPosition = x), new Vector3(endValue, 0.0f, 0.0f), duration);
            t.SetOptions(AxisConstraint.X, snapping).SetTarget<Tweener>((object) target);
            return t;
        }

        public static TweenerCore<Vector3, Vector3, VectorOptions> DOAnchoredMoveY(
            this RectTransform target,
            float endValue,
            float duration,
            bool snapping = false)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> t = DOTween.To(
                (DOGetter<Vector3>) (() => target.anchoredPosition),
                (DOSetter<Vector3>) (y => target.anchoredPosition = y), new Vector3(0.0f, endValue, 0.0f), duration);
            t.SetOptions(AxisConstraint.Y, snapping).SetTarget<Tweener>((object) target);
            return t;
        }

        public static TweenerCore<Vector3, Vector3, VectorOptions> DOAnchoredMoveX(
            this IView target, float endValue, float duration, bool snapping = false
        )
        {
            return target.Rect.DOAnchoredMoveX(endValue, duration, snapping);
        }

        public static TweenerCore<Vector3, Vector3, VectorOptions> DOAnchoredMoveY(
            this IView target, float endValue, float duration, bool snapping = false
        )
        {
            return target.Rect.DOAnchoredMoveY(endValue, duration, snapping);
        }

        public static TweenerCore<float, float, FloatOptions> DOFade(
            this IView target,
            float endValue,
            float duration)
        {
            TweenerCore<float, float, FloatOptions> alpha = DOTween.To(() => target.Alpha, (x => target.Alpha = x),
                endValue, duration);
            alpha.SetTarget(target);
            return alpha;
        }

        public static TweenerCore<float, float, FloatOptions> DOFade(
            this CanvasGroup target,
            float endValue,
            float duration)
        {
            var alpha = DOTween.To(() => target.alpha, a => target.alpha = a, endValue, duration);
            alpha.SetTarget(target);
            return alpha;
        }
    }
}
