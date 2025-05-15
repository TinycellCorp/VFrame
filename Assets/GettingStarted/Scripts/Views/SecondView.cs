using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VFrame.UI.View;

namespace GettingStarted.Views
{
    public class SecondView : ComponentView<SecondView>
    {
        public override UniTask Ready()
        {
            PositionZero();
            return UniTask.CompletedTask;
        }

        public override void OnEnter()
        {
            Debug.Log(nameof(SecondView));
        }

        public override void OnExit()
        {
        }
    }
}