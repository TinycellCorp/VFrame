using UnityEditor.ShortcutManagement;
using UnityEngine.UI;

namespace VFrame.Editor.UI.Tool
{
    // Author: Chris Yarbrough
    using UnityEditor;
    using UnityEditor.EditorTools;
    using UnityEngine;

    [EditorTool("Raycast Padding Tool", typeof(Graphic))]
    public class RaycastPaddingTool : EditorTool
    {
        [SerializeField] private Texture2D icon;

        private GUIContent iconContent;

        public override GUIContent toolbarIcon => iconContent;

        private void OnEnable()
        {
            iconContent = new GUIContent
            {
                image = icon,
                text = "Raycast Padding Tool",
                tooltip = "Interactively edits the raycast padding " +
                          "property of any UI.Graphic component."
            };
        }

        public override void OnToolGUI(EditorWindow window)
        {
            var graphic = target as Graphic;

            // Seems like an issue with Unity, but target can sometimes be a GameObject.
            if (graphic == null)
                return;

            RectTransform rectTransform = (RectTransform) graphic.transform;
            using (new Handles.DrawingScope(rectTransform.localToWorldMatrix))
            {
                Rect rect = rectTransform.rect;
                Vector4 padding = graphic.raycastPadding;
                rect.xMin += padding[0]; // Left
                rect.yMin += padding[1]; // Bottom
                rect.xMax -= padding[2]; // Right
                rect.yMax -= padding[3]; // Top

                Handles.DrawSolidRectangleWithOutline(rect, Color.clear, Color.cyan);
            }
        }

        [Shortcut("Tools/Raycast Padding Tool", KeyCode.T, ShortcutModifiers.Action)]
        static void ToolShortcut()
        {
            if (Selection.GetFiltered<Graphic>(SelectionMode.TopLevel).Length > 0)
            {
                ToolManager.SetActiveTool<RaycastPaddingTool>();
            }
        }
    }
}
