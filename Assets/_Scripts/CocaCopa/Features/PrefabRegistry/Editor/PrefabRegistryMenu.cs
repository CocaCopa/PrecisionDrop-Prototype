// Assets/PrefabRegistry/Editor/PrefabRegistryMenu.cs

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace CocaCopa.PrefabRegistry.EditorUtils {
    internal static class PrefabRegistryMenu {
        private const string DefaultResourcesPath = "Assets/Resources";
        private const string SettingsAssetPath = DefaultResourcesPath + "/PrefabRegistrySettings.asset";

        [MenuItem("Tools/Prefab Registry/Create Settings + Catalog")]
        private static void CreateSettingsAndCatalog() {
            if (!AssetDatabase.IsValidFolder("Assets/Resources")) { AssetDatabase.CreateFolder("Assets", "Resources"); }

            var settings = AssetDatabase.LoadAssetAtPath<PrefabRegistrySettings>(SettingsAssetPath);
            if (settings == null) {
                settings = ScriptableObject.CreateInstance<PrefabRegistrySettings>();
                AssetDatabase.CreateAsset(settings, SettingsAssetPath);
            }

            if (settings.Catalog == null) {
                var catalog = ScriptableObject.CreateInstance<PrefabCatalog>();
                const string catalogPath = "Assets/PrefabCatalog.asset";
                AssetDatabase.CreateAsset(catalog, catalogPath);
                settings.SetCatalog(catalog);
                EditorUtility.SetDirty(settings);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = settings;
            EditorGUIUtility.PingObject(settings);
        }
    }
}
#endif