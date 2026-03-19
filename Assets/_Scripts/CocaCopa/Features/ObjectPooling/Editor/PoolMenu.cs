#if UNITY_EDITOR
using CocaCopa.ObjectPooling.Unity.Config;
using UnityEditor;
using UnityEngine;

namespace CocaCopa.ObjectPooling.EditorUtils {
    internal static class PrefabRegistryMenu {
        private const string DefaultResourcesPath = "Assets/Resources";
        private const string SettingsAssetPath = DefaultResourcesPath + "/PoolSettings.asset";

        [MenuItem("Tools/CocaCopa/Object Pool/Create Settings + Catalog")]
        private static void CreateSettingsAndCatalog() {
            if (!AssetDatabase.IsValidFolder("Assets/Resources")) { AssetDatabase.CreateFolder("Assets", "Resources"); }

            var settings = AssetDatabase.LoadAssetAtPath<PoolSettings>(SettingsAssetPath);
            if (settings == null) {
                settings = ScriptableObject.CreateInstance<PoolSettings>();
                AssetDatabase.CreateAsset(settings, SettingsAssetPath);
            }

            if (settings.Catalog == null) {
                var catalog = ScriptableObject.CreateInstance<PoolCatalog>();
                const string catalogPath = "Assets/PoolCatalog.asset";
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