using System;
using UnityEditor;
using UnityEngine;
using PrecisionDrop.App.Unity.Themes;
using PrecisionDrop.GameEnvironment.Unity;
using PrecisionDrop.Platforms.Unity;
using PrecisionDrop.Platforms.Unity.Presentation;
using PrecisionDrop.Player.Unity.Presentation;
using PrecisionDrop.Player.Unity;

namespace PrecisionDrop.App.EditorUtils {
    [CustomEditor(typeof(LevelThemeAsset))]
    internal sealed class LevelThemeAssetEditor : Editor {
        private static readonly int Tex = Shader.PropertyToID("_Tex");
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        private const string EditorOnlyTagName = "EditorOnly";

        private GameObject editorOnlyHolder;
        private PlatformTheme PlatformTheme => ((LevelThemeAsset)target).PlatformTheme;
        private PlayerTheme PlayerTheme => ((LevelThemeAsset)target).PlayerTheme;
        private EnvironmentTheme EnvironmentTheme => ((LevelThemeAsset)target).EnvironmentTheme;

        private void OnEnable() {
            editorOnlyHolder = GameObject.FindGameObjectWithTag(EditorOnlyTagName);
            if (!editorOnlyHolder) {
                Debug.LogWarning("Could not find a gameObject with the 'EditorOnly' tag. Created automatically");
                editorOnlyHolder = new GameObject("EditorOnly");
                editorOnlyHolder.tag = "EditorOnly";
            }
        }

        private void OnDisable() {
            ClearEditorHolderChildren();
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            EditorGUILayout.Space(20f);
            Button_VisualizeTheme();
        }

        private void Button_VisualizeTheme() {
            if (!GUILayout.Button("Visualize Theme")) { return; }
            if (!editorOnlyHolder) { Debug.LogWarning($"[{nameof(LevelThemeAssetEditor)}] No GameObject with tag '{EditorOnlyTagName}' was found in the scene."); }

            ClearEditorHolderChildren();

            for (int i = 0; i < 5; i++) {
                float heightOffset = -5f * i;
                float rotOffset = -120f * i;
                CreatePlatform(heightOffset, rotOffset);
            }

            PaintPlayerObj();
            PaintEnvironment();
        }

        private void CreatePlatform(float heightOffset, float rotOffset) {
            var platformHolder = new GameObject("Platform");
            GameObject piecePrefab = FindPrefabWithComponent<PlatformPiece>();

            platformHolder.transform.SetParent(editorOnlyHolder.transform);
            platformHolder.transform.localPosition = new Vector3(0f, heightOffset, 0f);

            float rotY = rotOffset;

            for (int i = 0; i < 32; i++) {
                GameObject pieceObj = Instantiate(piecePrefab, platformHolder.transform);
                pieceObj.transform.localPosition = Vector3.zero;
                pieceObj.transform.localEulerAngles = new Vector3(0f, rotY, 0f);
                rotY += 10f;

                var pieceRenderer = pieceObj.GetComponentInChildren<MeshRenderer>();
                if (!pieceRenderer) {
                    Debug.LogWarning($"[{nameof(LevelThemeAssetEditor)}] {nameof(MeshRenderer)} not found.");
                    continue;
                }

                Color pieceColor = i > 19 && i < 25
                    ? PlatformTheme.DangerColor
                    : PlatformTheme.RegularColor;

                SetRendererColor(pieceRenderer, pieceColor);
            }
        }

        private void PaintPlayerObj() {
            GameObject playerObj = FindAnyObjectByType<PlayerSphere>().gameObject;
            if (!playerObj.TryGetComponent(out MeshRenderer renderer)) {
                renderer = playerObj.GetComponentInChildren<MeshRenderer>();
                if (!renderer) {
                    throw new Exception(
                        "Could not find a 'MeshRenderer' component attached in the player prefab object."
                    );
                }
            }

            SetRendererColor(renderer, PlayerTheme.SphereColor);
        }

        private void PaintEnvironment() {
            GameObject towerObj = GameObject.Find("Tower");
            if (!towerObj) {
                throw new Exception(
                    "Could not find 'Tower' gameObject by name"
                );
            }

            if (!towerObj.TryGetComponent(out MeshRenderer towerRenderer)) {
                towerRenderer = towerObj.GetComponentInChildren<MeshRenderer>();
                if (!towerRenderer) {
                    throw new NullReferenceException(
                        "Could not fetch 'MeshRenderer' from 'Tower' object"
                    );
                }
            }

            SetRendererColor(towerRenderer, EnvironmentTheme.towerColor);
            SetSkyboxCubemap(EnvironmentTheme.skyboxMap);
        }

        private static void SetRendererColor(Renderer renderer, Color color) {
            if (!renderer) { return; }

            var block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);

            Material sharedMat = renderer.sharedMaterial;
            if (!sharedMat) {
                Debug.LogWarning($"[{nameof(LevelThemeAssetEditor)}] {nameof(Renderer)} has no {nameof(Renderer.sharedMaterial)}.");
                return;
            }

            if (sharedMat.HasProperty(BaseColorId)) { block.SetColor(BaseColorId, color); }
            else if (sharedMat.HasProperty(ColorId)) { block.SetColor(ColorId, color); }
            else {
                Debug.LogWarning(
                    $"[{nameof(LevelThemeAssetEditor)}] Material '{sharedMat.name}' has neither '_BaseColor' nor '_Color'.");
                return;
            }

            renderer.SetPropertyBlock(block);
        }

        private static void SetSkyboxCubemap(Cubemap map) {
            Material skyboxMat = RenderSettings.skybox;

            if (!skyboxMat) {
                Debug.LogError("No skybox material assigned in RenderSettings.");
                return;
            }

            skyboxMat.SetTexture(Tex, map);
            DynamicGI.UpdateEnvironment();
        }

        private static GameObject FindPrefabWithComponent<T>() where T : Component {
            string[] guids = AssetDatabase.FindAssets("t:Prefab");
            GameObject foundPrefab = null;

            foreach (string guid in guids) {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (!prefab) { continue; }

                if (!prefab.GetComponentInChildren<T>(true)) { continue; }

                if (foundPrefab) {
                    Debug.LogWarning(
                        $"[{nameof(LevelThemeAssetEditor)}] Multiple prefabs found with component '{typeof(T).Name}'. " +
                        $"Using '{foundPrefab.name}' and ignoring '{prefab.name}'."
                    );
                    continue;
                }

                foundPrefab = prefab;
            }

            if (!foundPrefab) { throw new Exception($"[{nameof(LevelThemeAssetEditor)}] No prefab found with component '{typeof(T).Name}'."); }

            return foundPrefab;
        }

        private void ClearEditorHolderChildren() {
            for (int i = editorOnlyHolder.transform.childCount - 1; i >= 0; i--) {
                GameObject child = editorOnlyHolder.transform.GetChild(i).gameObject;
                DestroyImmediate(child);
            }
        }
    }
}