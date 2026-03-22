using UnityEngine;

namespace PrecisionDrop.GameEnvironment.Unity {
    public class Environment : MonoBehaviour {
        [SerializeField] private MeshRenderer towerRenderer;

        private static readonly int Tex = Shader.PropertyToID("_Tex");

        public void ApplyTheme(EnvironmentTheme theme) {
            SetSkyboxCubemap(theme.skyboxMap);
            towerRenderer.material.color = theme.towerColor;
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
    }
}