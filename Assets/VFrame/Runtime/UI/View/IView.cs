using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VFrame.UI.View
{
    public interface IView
    {
        RectTransform Rect { get; }
        
        float Alpha { get; set; }
        bool IsActive { get; set; }
        bool IsInteractable { get; set; }
        UniTask Ready();
        void OnEnter();
        void OnExit();
    }
}