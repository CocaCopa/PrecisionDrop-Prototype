using System;
using PrecisionDrop.GameFlow.Contracts;
using PrecisionDrop.LevelGeneration.Runtime;
using PrecisionDrop.Platforms.Contracts;
using UnityEngine;
using UnityEngine.Serialization;

namespace PrecisionDrop.LevelGeneration.Unity {
    public sealed class LevelGeneratorSystem : MonoBehaviour {
        [SerializeField] private GenerationSettingsAsset generationAsset;
        
        private LevelGeneratorFlow flow;
        private bool installed;
        private bool initialized;

        

        public void Install(IGameFlow gameFlow, IPlatformBuilder builder) {
            if (installed) { throw new InvalidOperationException($"[{nameof(LevelGeneratorSystem)}] {nameof(Install)}() called twice."); }
            if (builder is null) { throw new ArgumentNullException(nameof(builder)); }
            if (gameFlow is null) { throw new ArgumentNullException(nameof(gameFlow)); }

            flow = new LevelGeneratorFlow(generationAsset.GenSettings, gameFlow, builder);
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
