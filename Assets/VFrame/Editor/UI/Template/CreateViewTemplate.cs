using System.IO;
using UnityEditor;

namespace VFrame.Editor.UI.Template
{
    public static class CreateViewTemplate
    {
        private const string TemplatePath = "Assets/VFrame/Runtime/UI/Template";
        private const string ComponentViewTemplate = "ComponentViewTemplate.cs.txt";
        private const string ConfirmPopupTemplate = "ConfirmPopupTemplate.cs.txt";
        private const string DialogPopupTemplate = "DialogPopupTemplate.cs.txt";

        [MenuItem("Assets/Create/VFrame/UI/ComponentView")]
        static void CreateComponentViewScript(MenuCommand command)
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(
                Path.Combine(TemplatePath, ComponentViewTemplate),
                $"{nameof(ComponentViewTemplate)}.cs");
        }

        [MenuItem("Assets/Create/VFrame/UI/ConfirmPopup")]
        static void CreateConfirmPopupScript(MenuCommand command)
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(
                Path.Combine(TemplatePath, ConfirmPopupTemplate),
                $"{nameof(ConfirmPopupTemplate)}.cs");
        }

        [MenuItem("Assets/Create/VFrame/UI/DialogPopup")]
        static void CreateDialogPopupScript(MenuCommand command)
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(
                Path.Combine(TemplatePath, DialogPopupTemplate),
                $"{nameof(DialogPopupTemplate)}.cs");
        }
    }
}