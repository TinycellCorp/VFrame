using VFrame.UI.Component.Buttons;
using VFrame.UI.Component.Events;
using UnityEditor;
using UnityEngine.UI;
using VFrame.Audio;

namespace VFrame.Editor.UI
{
    internal static class ButtonComponent
    {
        [MenuItem("CONTEXT/Button/Pressed Event")]
        static void ButtonPressedEvent(MenuCommand command)
        {
            var target = command.context as Button;
            if (target == null) return;
            Undo.AddComponent<PressedEvent>(target.gameObject);
        }

        [MenuItem("CONTEXT/Button/Pressed Event - Blackboard")]
        static void ButtonPressedEventBlackboard(MenuCommand command)
        {
            var target = command.context as Button;
            if (target == null) return;
            var component = Undo.AddComponent<PressedEventBlackboard>(target.gameObject);

            component.FromSecondsKey = "from";
            component.IntervalSecondsKey = "interval";
        }

        [MenuItem("CONTEXT/Button/UISystem.To(View)")]
        static void ButtonUISystemTo(MenuCommand command)
        {
            var target = command.context as Button;
            if (target == null) return;
            Undo.AddComponent<ToButton>(target.gameObject);
        }

        [MenuItem("CONTEXT/Button/UISystem.Back")]
        static void ButtonUISystemBack(MenuCommand command)
        {
            var target = command.context as Button;
            if (target == null) return;
            Undo.AddComponent<BackButton>(target.gameObject);
        }


        [MenuItem("CONTEXT/Button/Play Audio - Key")]
        static void ButtonPlayAudioFromKey(MenuCommand command)
        {
            var target = command.context as Button;
            if (target == null) return;
            Undo.AddComponent<PlayAudioFromKey>(target.gameObject);
        }


        [MenuItem("CONTEXT/Button/Play Audio - Asset Reference")]
        static void ButtonPlayAudioFromAssetReference(MenuCommand command)
        {
            var target = command.context as Button;
            if (target == null) return;
            Undo.AddComponent<PlayAudioFromAssetReference>(target.gameObject);
        }
    }
}