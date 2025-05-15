using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;
using VFrame.UI.Animation;
using VFrame.UI.View;
using LMotion = LitMotion.LMotion;

namespace VFrame.External.LitMotion
{
    public class FadeAnimation<TView> : IAnimation<TView> where TView : IView
    {
        public async UniTask In(IView view)
        {
            MotionHandle handle = LMotion.Create(0f, 1f, 0.3f).Bind(view, static (value, view) => view.Alpha = value);
            await handle;
        }

        public async UniTask Out(IView view)
        {
            MotionHandle handle = LMotion.Create(1f, 0f, 0.3f).Bind(view, static (value, view) => view.Alpha = value);
            await handle;
        }
    }
}