using UnityEditor;
using UnityEngine;
using VFrame.UI.Component.Buttons;

namespace VFrame.Editor.UI.Gizmo
{
    public static class ComponentViewGizmos
    {
        [DrawGizmo(GizmoType.Active)]
        static void ToButtonGizmo(ToButton button, GizmoType gizmoType)
        {
            if (button.Target == null) return;

            Gizmos.DrawLine(button.transform.position, button.Target.transform.position);
        }
    }
}