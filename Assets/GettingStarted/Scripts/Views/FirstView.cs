using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VFrame.UI.View;

namespace GettingStarted.Views
{
    public class FirstView : ComponentView<FirstView>, IInitializable
    {
        public override void OnEnter()
        {
            Debug.Log("Hello World");
        }

        public override void OnExit()
        {
        }

        private Company _company;

        [Inject]
        public void Constructor(Company company)
        {
            _company = company;
        }

        public void Initialize()
        {
            Debug.Log(nameof(Initialize) + $" {_company.CeoName}");
        }
    }
}