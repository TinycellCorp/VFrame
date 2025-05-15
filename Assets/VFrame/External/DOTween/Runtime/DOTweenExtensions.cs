#if VFRAME_DOTWEEN
using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using VFrame.UI.View;
using UnityEngine;
using UnityEngine.UI;

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
                (DOGetter<Vector3>)(() => target.anchoredPosition),
                (DOSetter<Vector3>)(x => target.anchoredPosition = x), new Vector3(endValue, 0.0f, 0.0f), duration);
            t.SetOptions(AxisConstraint.X, snapping).SetTarget<Tweener>((object)target);
            return t;
        }

        public static TweenerCore<Vector3, Vector3, VectorOptions> DOAnchoredMoveY(
            this RectTransform target,
            float endValue,
            float duration,
            bool snapping = false)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> t = DOTween.To(
                (DOGetter<Vector3>)(() => target.anchoredPosition),
                (DOSetter<Vector3>)(y => target.anchoredPosition = y), new Vector3(0.0f, endValue, 0.0f), duration);
            t.SetOptions(AxisConstraint.Y, snapping).SetTarget<Tweener>((object)target);
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

        public static TweenerCore<Vector2, Vector2, VectorOptions> DOAnchoredMove(
            this IView target, Vector2 endValue, float duration, bool snapping = false
        )
        {
            return target.Rect.DOAnchoredMove(endValue, duration, snapping);
        }

        public static TweenerCore<Vector2, Vector2, VectorOptions> DOAnchoredMove(
            this RectTransform target, Vector2 endValue, float duration, bool snapping = false
        )
        {
            TweenerCore<Vector2, Vector2, VectorOptions> t = DOTween.To(
                (DOGetter<Vector2>)(() => target.anchoredPosition),
                (DOSetter<Vector2>)(p => target.anchoredPosition = p), endValue, duration);
            t.SetOptions(snapping).SetTarget<Tweener>((object)target);
            return t;
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

        public static TweenerCore<float, float, FloatOptions> DOFade(
            this Image target,
            float endValue,
            float duration)
        {
            var alpha = DOTween.To(() => target.color.a, a =>
            {
                var c = target.color;
                c.a = a;
                target.color = c;
            }, endValue, duration);
            alpha.SetTarget(target);
            return alpha;
        }

        public static TweenerCore<Vector3, Vector3, VectorOptions> DOScale(
            this RectTransform target,
            Vector3 endValue,
            float duration)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> t = DOTween.To(
                () => target.localScale,
                x => target.localScale = x,
                endValue,
                duration
            );
            t.SetTarget(target);
            return t;
        }

        public static void DOScaleChildren(
            this RectTransform parent,
            Vector3 endValue,
            float duration)
        {
            foreach (RectTransform child in parent)
            {
                child.DOScale(endValue, duration);
            }
        }

        public static void DOScaleChildren(
            this RectTransform parent,
            Vector3 endValue,
            float duration,
            Func<RectTransform, bool> filter)
        {
            foreach (RectTransform child in parent)
            {
                if (filter(child))
                {
                    child.DOScale(endValue, duration);
                }
            }
        }

        public static TweenerCore<Vector3, Vector3, VectorOptions> DOScale(
            this IView target,
            Vector3 endValue,
            float duration)
        {
            return target.Rect.DOScale(endValue, duration);
        }

        public static void DOScaleChildren(
            this IView target,
            Vector3 endValue,
            float duration)
        {
            target.Rect.DOScaleChildren(endValue, duration);
        }

        public static void DOScaleChildren(
            this RectTransform parent,
            Vector3 endValue,
            float duration,
            Sequence sequence)
        {
            foreach (RectTransform child in parent)
            {
                var tweener = child.DOScale(endValue, duration).SetEase(Ease.OutExpo).SetUpdate(true);
                sequence.Insert(0, tweener);
            }
        }

        public static void DOScaleChildren(
            this IView target,
            Vector3 endValue,
            float duration,
            Func<RectTransform, bool> filter)
        {
            target.Rect.DOScaleChildren(endValue, duration, filter);
        }

        public static void DOScaleChildren(
            this RectTransform parent,
            Vector3 startValue,
            Vector3 endValue,
            float duration,
            Sequence sequence,
            Func<RectTransform, bool> filter)
        {
            foreach (RectTransform child in parent)
            {
                if (filter(child))
                {
                    child.localScale = startValue;
                    var tweener = child.DOScale(endValue, duration).SetEase(Ease.OutExpo).SetUpdate(true);
                    sequence.Insert(0, tweener);
                }
                else
                {
                    foreach (RectTransform cChild in child)
                    {
                        if (filter(cChild))
                        {
                            cChild.localScale = startValue;
                            var tweener = cChild.DOScale(endValue, duration).SetEase(Ease.OutExpo).SetUpdate(true);
                            sequence.Insert(0, tweener);
                        }
                    }
                }
            }
        }

        public static void DOPunchSacleChildren(
            this RectTransform parent,
            Vector3 startValue,
            Vector3 endValue,
            float duration,
            Sequence sequence,
            Func<RectTransform, bool> filter)
        {
            foreach (RectTransform child in parent)
            {
                if (filter(child))
                {
                    child.localScale = startValue;
                    var tweener = child.DOPunchScale(endValue, duration).SetEase(Ease.OutExpo).SetUpdate(true);
                    sequence.Insert(0, tweener);
                }
            }
        }

    }
}
#endif