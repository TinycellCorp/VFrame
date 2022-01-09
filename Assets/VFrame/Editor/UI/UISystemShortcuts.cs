using System;
using VFrame.UI.Layer;
using UnityEditor;
using UnityEngine;

namespace VFrame.Editor.UI
{
    // [CustomEditor(typeof(CanvasLayer))]
    public class CanvasShortcuts : UnityEditor.Editor
    {
        private CanvasLayer _target;
        private string _viewName;

        private void OnSceneGUI()
        {
            _target = target as CanvasLayer;
            Handles.BeginGUI();
            {
                var width = 180;
                var sceneWidth = SceneView.currentDrawingSceneView.camera.pixelWidth;
                var boxStyle = new GUIStyle("box");
                GUILayout.BeginArea(new Rect(sceneWidth - width - 10, 10, width, 70), boxStyle);
                {
                    _viewName = GUILayout.TextField(_viewName);
                    GUI.enabled = !string.IsNullOrEmpty(_viewName);
                    if (GUILayout.Button("view"))
                    {
                        Debug.Log("do");
                        var code = @$"
                            public class {_viewName} : ComponentView<{_viewName}>{{ }}
                        ";
                        var folder = "Assets";
                        folder = EditorUtility.SaveFolderPanel("script", folder, $"{_viewName}.cs");

                        Debug.Log(folder);
                    }

                    GUI.enabled = true;
                }
                GUILayout.EndArea();
            }
            Handles.EndGUI();
        }
    }
}