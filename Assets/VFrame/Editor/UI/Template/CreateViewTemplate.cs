using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VFrame.Editor.UI.Template
{
    public static class CreateViewTemplate
    {
        private const string TemplatePath = "Assets/VFrame/Runtime/UI/Template";
        private const string ComponentViewTemplate = "ComponentViewTemplate.cs";
        private const string ConfirmPopupTemplate = "ConfirmPopupTemplate.cs";
        private const string DialogPopupTemplate = "DialogPopupTemplate.cs";

        static bool TryFindPath(string fileName, out string result)
        {
            var paths = AssetDatabase.FindAssets(fileName).Select(AssetDatabase.GUIDToAssetPath);
            result = string.Empty;
            foreach (var path in paths)
            {
                var name = Path.GetFileNameWithoutExtension(path);
                if (name.Equals(fileName))
                {
                    result = path;
                    break;
                }
            }

            return !string.IsNullOrEmpty(result);
        }

        [MenuItem("Assets/Create/VFrame/UI/ComponentView")]
        static void CreateComponentViewScript(MenuCommand command)
        {
            if (TryFindPath(ComponentViewTemplate, out var path))
            {
                ProjectWindowUtil.CreateScriptAssetFromTemplateFile(path, $"{nameof(ComponentViewTemplate)}.cs");
            }
        }

        [MenuItem("Assets/Create/VFrame/UI/ConfirmPopup")]
        static void CreateConfirmPopupScript(MenuCommand command)
        {
            if (TryFindPath(ConfirmPopupTemplate, out var path))
            {
                ProjectWindowUtil.CreateScriptAssetFromTemplateFile(path, $"{nameof(ConfirmPopupTemplate)}.cs");
            }
        }

        [MenuItem("Assets/Create/VFrame/UI/DialogPopup")]
        static void CreateDialogPopupScript(MenuCommand command)
        {
            if (TryFindPath(DialogPopupTemplate, out var path))
            {
                ProjectWindowUtil.CreateScriptAssetFromTemplateFile(path, $"{nameof(DialogPopupTemplate)}.cs");
            }
        }
    }
}