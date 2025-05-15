using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VFrame.UI.View;

namespace GettingStarted.Views
{
    public class FirstView : ComponentView<FirstView>
    {
        public override void OnEnter()
        {
            Debug.Log("Hello World");
        }

        public override void OnExit()
        {
        }
    }
}