using System;
using System.Linq;
using UnityEngine;
using VFrame.Audio;
using VFrame.UI;
using VFrame.UI.Blackboard;

namespace VFrame.Core
{
    // [CreateAssetMenu(fileName = "VFrameSettings", menuName = "VFrame/VFrameSettings")]
    public class VFrameSettings : ScriptableObject
    {
        public static VFrameSettings Instance { get; private set; }
        
        public RootCanvas RootCanvas;
        public BlackboardAsset BlackboardAsset;
        
        // [Obsolete]
        // public AudioGroup[] AudioGroups;

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/VFrame/VFrame Settings")]
        public static void CreateAsset()
        {
            var path = UnityEditor.EditorUtility.SaveFilePanelInProject(
                "Save VFrameSettings",
                "VFrameSettings",
                "asset",
                string.Empty);

            if (string.IsNullOrEmpty(path))
                return;

            var newSettings = CreateInstance<VFrameSettings>();
            UnityEditor.AssetDatabase.CreateAsset(newSettings, path);

            var preloadedAssets = UnityEditor.PlayerSettings.GetPreloadedAssets().ToList();
            preloadedAssets.RemoveAll(x => x is VFrameSettings);
            preloadedAssets.Add(newSettings);
            UnityEditor.PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
        }

        public static void LoadInstanceFromPreloadAssets()
        {
            var preloadAsset = UnityEditor.PlayerSettings.GetPreloadedAssets().FirstOrDefault(x => x is VFrameSettings);
            if (preloadAsset is VFrameSettings instance)
            {
                instance.OnEnable();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void RuntimeInitialize()
        {
            // For editor, we need to load the Preload asset manually.
            LoadInstanceFromPreloadAssets();
        }

        // [UnityEditor.InitializeOnLoadMethod]
        // static void EditorInitialize()
        // {
        //     // RootLifetimeScope must be disposed before it can be resumed.
        //     UnityEditor.EditorApplication.playModeStateChanged += state =>
        //     {
        //         switch (state)
        //         {
        //             case UnityEditor.PlayModeStateChange.ExitingPlayMode:
        //                 if (Instance != null)
        //                 {
        //                     if (Instance.RootLifetimeScope != null)
        //                     {
        //                         Instance.RootLifetimeScope.DisposeCore();
        //                     }
        //                     Instance = null;
        //                 }
        //                 break;
        //         }
        //     };
        // }
#endif

        void OnEnable()
        {
            // if (RootLifetimeScope != null)
            //     RootLifetimeScope.IsRoot = true;
            Instance = this;
        }
    }
}