using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
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
            Debug.Log(nameof(SecondView) + $" {_person.Name}");
        }

        public override void OnExit()
        {
        }

        private Person _person;
        
        [Inject]
        public void Constructor(Person person)
        {
            _person = person;
        }
    }
}