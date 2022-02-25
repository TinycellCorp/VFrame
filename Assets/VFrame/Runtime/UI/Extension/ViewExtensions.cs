using Cysharp.Threading.Tasks;
using UnityEngine;
using VFrame.UI.Animation;
using VFrame.UI.View;

namespace VFrame.UI.Extension
{
    public static class ViewExtensions
    {
        public static async UniTask In(this IView view, IAnimation animation)
        {
            view.IsActive = true;
            await view.Ready();
            await animation.In(view);
        }

        public static async UniTask Out(this IView view, IAnimation animation)
        {
            await animation.Out(view);
            view.IsActive = false;
        }

        public static async UniTask Show(this IView view)
        {
            await view.Ready();
            view.Alpha = 1;
            view.IsActive = true;
            view.OnEnter();
        }

        public static void Hide(this IView view)
        {
            view.IsActive = false;
            view.OnExit();
        }

        public static float GetWidth(this IView view)
        {
            return view.Rect.rect.width;
        }

        public static float GetHeight(this IView view)
        {
            return view.Rect.rect.height;
        }

        public static void SetPosition(this IView view, float x)
        {
            var pos = view.Rect.anchoredPosition;
            pos.x = x;
            view.Rect.anchoredPosition = pos;
        }

        public static void SetPosition(this IView view, Vector2 pos)
        {
            view.Rect.anchoredPosition = pos;
        }

        public static Vector2 GetPosition(this IView view)
        {
            return view.Rect.anchoredPosition;
        }
    }
}