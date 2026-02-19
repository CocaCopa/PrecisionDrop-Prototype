using PrecisionDrop.LevelGeneration.Runtime;
using UnityEngine;

namespace PrecisionDrop.LevelGeneration.Unity {
    [CreateAssetMenu(fileName = "GenerationSettings", menuName = "PrecisionDrop/Level/GenerationSettings")]
    public sealed class GenerationSettingsAsset : ScriptableObject {
        [SerializeField] private GenerationSettings generationSettings;

        internal GenerationSettings GenSettings => generationSettings;
        
        private void OnValidate() {
            ValidateGenSettings();
        }

        private void ValidateGenSettings() {
            generationSettings.firstBatchCount = Mathf.Max(0, generationSettings.firstBatchCount);
        }
    }
}