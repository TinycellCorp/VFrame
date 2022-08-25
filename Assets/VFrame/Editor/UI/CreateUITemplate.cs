using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using VFrame.UI.Layer;
using VFrame.UI.Module.Popup;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
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
        static bool IsValidateView() => TryGetComponent(out CanvasLayer component);

        [MenuItem(MenuPath + "/View")]
        static void CreateView() => CreateInstance<CanvasLayer>("View");


        [MenuItem(MenuPath + "/ConfirmPopup", true)]
        [MenuItem(MenuPath + "/DialogPopup", true)]
        static bool IsValidatePopup() => TryGetComponent(out Canvas component);

        [MenuItem(MenuPath + "/ConfirmPopup")]
        static void CreateConfirmPopup() => CreateInstance<Canvas>("Confirm");

        [MenuItem(MenuPath + "/DialogPopup")]
        static void CreateDialogPopup() => CreateInstance<Canvas>("Dialog");


        private static readonly Dictionary<string, Action<Component>> RequireFieldFinder =
            new Dictionary<string, Action<Component>>
            {
                {"ConfirmPopup", FindConfirmFields},
                {"DialogPopup", FindDialogFields}
            };

        /// <summary>
        /// TODO:
        /// 여러개를 선택하고 실행하면 메소드가 3번 실행됨.
        /// 여러 대상의 처리를 위해 Selection.objects를 사용하면 3*3 번 실행됨.
        /// </summary>
        [MenuItem(MenuPath + "/AutoAttach")]
        static void AutoAttach()
        {
            var objects = Selection.objects;
            if (objects == null || objects.Length == 0) return;
            foreach (var selected in objects)
            {
                if (selected is not GameObject target) return;

                if (TryGetScriptType(target, out var type))
                {
                    if (target.TryGetComponent(type, out var attached)) continue;
                    var component = target.AddComponent(type);

                    if (type.BaseType != null)
                    {
                        var name = type.BaseType.Name;
                        foreach (var pair in RequireFieldFinder)
                        {
                            if (name.Contains(pair.Key))
                            {
                                pair.Value(component);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"not found script: {target.name}");
                }
            }
        }

        private static void FindConfirmFields(Component component)
        {
            var transform = component.transform;
            FindFields(component, new[]
            {
                (transform.Find("Content"), "contentText", typeof(TextMeshProUGUI)),

                (transform.Find("Button"), "confirmButton", typeof(Button)),
                (transform.Find("Button").GetChild(0), "confirmText", typeof(TextMeshProUGUI)),
            });
        }

        private static void FindDialogFields(Component component)
        {
            var transform = component.transform;
            FindFields(component, new[]
            {
                (transform.Find("Content"), "contentText", typeof(TextMeshProUGUI)),

                (transform.Find("Positive"), "positiveButton", typeof(Button)),
                (transform.Find("Positive").GetChild(0), "positiveText", typeof(TextMeshProUGUI)),

                (transform.Find("Negative"), "negativeButton", typeof(Button)),
                (transform.Find("Negative").GetChild(0), "negativeText", typeof(TextMeshProUGUI)),
            });
        }

        private static void FindFields(Component component,
            (Transform target, string fieldName, Type componentType)[] requireFields)
        {
            var type = component.GetType().BaseType;
            if (type == null) return;

            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            foreach (var requireField in requireFields)
            {
                var findComponent = requireField.target.GetComponent(requireField.componentType);
                type.GetField(requireField.fieldName, flags)?.SetValue(component, findComponent);
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

        static bool TryGetScriptType(GameObject target, out System.Type scriptType)
        {
            scriptType = null;
            if (target == null) return false;
            var name = target.name;

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