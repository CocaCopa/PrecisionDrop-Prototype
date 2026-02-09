using System;
using PrecisionDrop.GameFlow.Contracts;
using PrecisionDrop.LevelGeneration.Runtime;
using PrecisionDrop.Platforms.Contracts;
using UnityEngine;

namespace PrecisionDrop.LevelGeneration.Unity {
    public sealed class LevelGeneratorSystem : MonoBehaviour {
        [SerializeField] private GenerationSettings generationSettings;
        
        private LevelGeneratorFlow flow;
        private bool installed;
        private bool initialized;

        private void OnValidate() {
            ValidateGenSettings();
        }

        private void ValidateGenSettings() {
            generationSettings.firstBatchCount = Mathf.Max(0, generationSettings.firstBatchCount);
            generationSettings.gapRange = new CocaCopa.Primitives.RangeInt(
                Mathf.Max(0, generationSettings.gapRange.min),
                Mathf.Max(generationSettings.gapRange.min, generationSettings.gapRange.max)
            );
        }

        public void Install(IGameFlow gameFlow, IPlatformBuilder builder) {
            if (installed) { throw new InvalidOperationException($"[{nameof(LevelGeneratorSystem)}] {nameof(Install)}() called twice."); }
            if (builder is null) { throw new ArgumentNullException(nameof(builder)); }
            if (gameFlow is null) { throw new ArgumentNullException(nameof(gameFlow)); }

            flow = new LevelGeneratorFlow(generationSettings, gameFlow, builder);
            installed = true;
        }

        public void Init() {
            if (!installed) { throw new InvalidOperationException($"[{nameof(LevelGeneratorSystem)}] {nameof(Init)}() called before Install()."); }
            if (initialized) { Debug.LogWarning($"[{nameof(LevelGeneratorSystem)}] Already initialized."); return; }

            initialized = true;
            flow.Initialize();
        }
    }
}
