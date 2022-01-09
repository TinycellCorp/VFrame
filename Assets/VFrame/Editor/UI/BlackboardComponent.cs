using VFrame.UI.Blackboard;
using VFrame.UI.Blackboard.Event;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace VFrame.Editor.UI
{
    internal static class BlackboardComponent
    {
        [MenuItem("CONTEXT/Button/Blackboard - Interactable")]
        private static void ButtonInteractableEvent(MenuCommand command)
        {
            var target = command.context as Button;
            var component = SetupForBoolMethod(target, "interactable");
            if (component != null) component.Mode = RegisterMode.EnableAndDisable;
        }

        [MenuItem("CONTEXT/Button/Blackboard - Active")]
        private static void ButtonActiveEvent(MenuCommand command)
        {
            var target = command.context as Button;
            var component = SetupForBoolMethod(target.gameObject, "SetActive");
            if (component != null) component.Mode = RegisterMode.AwakeAndDestroy;
        }

        private static BlackboardBoolEvent SetupForBoolMethod(Button target, string methodName)
        {
            if (!CheckContainsMethod(target.gameObject, methodName, out var component))
            {
                return null;
            }

            var method = target.GetType().GetProperty(methodName).GetSetMethod();
            var methodDelegate =
                System.Delegate.CreateDelegate(typeof(UnityAction<bool>), target, method) as UnityAction<bool>;
            UnityEventTools.AddPersistentListener(component.OnUpdateValue, methodDelegate);
            component.OnUpdateValue.SetPersistentListenerState(0, UnityEventCallState.RuntimeOnly);

            return component;
        }

        private static BlackboardBoolEvent SetupForBoolMethod(GameObject target, string methodName)
        {
            if (!CheckContainsMethod(target, methodName, out var component))
            {
                return null;
            }

            var method = target.GetType().GetMethod(methodName);
            var methodDelegate =
                System.Delegate.CreateDelegate(typeof(UnityAction<bool>), target, method) as UnityAction<bool>;
            UnityEventTools.AddPersistentListener(component.OnUpdateValue, methodDelegate);
            component.OnUpdateValue.SetPersistentListenerState(0, UnityEventCallState.RuntimeOnly);

            return component;
        }

        private static bool CheckContainsMethod(GameObject target, string methodName, out BlackboardBoolEvent component)
        {
            if (!target.TryGetComponent(out component))
            {
                component = Undo.AddComponent(target, typeof(BlackboardBoolEvent)) as BlackboardBoolEvent;
            }

            for (int i = 0; i < component.OnUpdateValue.GetPersistentEventCount(); i++)
            {
                var addedName = component.OnUpdateValue.GetPersistentMethodName(i);
                if (addedName.Contains(methodName))
                {
                    Debug.LogWarning($"contains event method: {addedName}");
                    return false;
                }
            }

            return true;
        }
    }
}