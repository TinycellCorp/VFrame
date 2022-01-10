using System;
using System.IO;
using System.Linq;
using VFrame.UI.Layer;
using VFrame.UI.Module.Popup;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VFrame.Editor.UI
{
    public static class CreateUIPrefab
    {
        private const string MenuPath = "GameObject/VFrame";

        static bool TryGetComponent<T>(out T component)
        {
            component = default;
            return Selection.activeGameObject != null && Selection.activeGameObject.TryGetComponent(out component);
        }

        private static void CreateInstance<TParent>(string prefabName) where TParent : UnityEngine.Component
        {
            if (!TryGetComponent(out TParent layer)) return;
            CreateInstance(prefabName, layer.transform);
        }

        private static void CreateInstance(string prefabName, Transform parent = null)
        {
            if (parent == null && Selection.activeGameObject != null)
            {
                parent = Selection.activeGameObject.transform;
            }

            var assetName = $"VFrame.{prefabName}";
            var findPath = $"{assetName} t:prefab";
            var paths = AssetDatabase.FindAssets(findPath)
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToArray();

            if (!paths.Any())
            {
                throw new Exception("find path is zero");
            }

            var resultPath = string.Empty;
            foreach (var path in paths)
            {
                var name = Path.GetFileNameWithoutExtension(path);
                if (name.Equals(assetName))
                {
                    resultPath = path;
                    break;
                }
            }

            if (string.IsNullOrEmpty(resultPath))
            {
                throw new FileNotFoundException(prefabName);
            }

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(resultPath);
            var view = Object.Instantiate(prefab, parent);
            view.name = prefabName;
            Selection.activeGameObject = view;

            Undo.RegisterCreatedObjectUndo(view, view.name);
        }

        [MenuItem(MenuPath + "/CanvasLayer", true)]
        static bool IsValidateLayer()
        {
            if (TryGetComponent(out Canvas canvas) && canvas.transform.parent == null)
            {
                return true;
            }

            return false;
        }

        [MenuItem(MenuPath + "/CanvasLayer")]
        static void CreateLayer() => CreateInstance<Canvas>("CanvasLayer");


        [MenuItem(MenuPath + "/View", true)]
        [MenuItem(MenuPath + "/ConfirmPopup", true)]
        [MenuItem(MenuPath + "/DialogPopup", true)]
        static bool IsValidateView() => TryGetComponent(out CanvasLayer component);

        [MenuItem(MenuPath + "/View")]
        static void CreateView() => CreateInstance<CanvasLayer>("View");

        [MenuItem(MenuPath + "/ConfirmPopup")]
        static void CreateConfirmPopup() => CreateInstance<CanvasLayer>("Confirm");

        [MenuItem(MenuPath + "/DialogPopup")]
        static void CreateDialogPopup() => CreateInstance<CanvasLayer>("Dialog");


        [MenuItem(MenuPath + "/AutoAttach")]
        static void AutoAttach()
        {
            if (TryGetScriptType(out var type))
            {
                var target = Selection.activeGameObject;
                var component = target.AddComponent(type);

                if (component is IRequireFieldFinder finder)
                    finder.Find();
            }
        }


        [MenuItem(MenuPath + "/AutoAttach", true)]
        static bool IsValidateAutoAttach() => TryGetScriptType(out var type);

        static bool TryGetScriptType(out System.Type scriptType)
        {
            scriptType = null;
            if (Selection.activeGameObject == null) return false;
            var name = Selection.activeGameObject.name;

            var script = AssetDatabase
                .FindAssets($"t:Script {name}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<MonoScript>)
                .FirstOrDefault(script => script.name.Equals(name));

            if (script == null)
            {
                return false;
            }

            scriptType = script.GetClass();
            return true;
        }


        [MenuItem(MenuPath + "/Button")]
        static void CreateButton() => CreateInstance("Button");
    }
}