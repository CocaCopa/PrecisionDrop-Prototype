using System;
using PrecisionDrop.LevelManagment.Runtime;
using PrecisionDrop.Platforms.Contracts;
using UnityEngine;

namespace PrecisionDrop.LevelManagment.Unity {
    public sealed class LevelGeneratorSystem : MonoBehaviour {
        private LevelGeneratorFlow flow;
        private bool installed;
        private bool initialized;

        public void Install(IPlatformBuilder builder) {
            if (installed) { throw new InvalidOperationException($"[{nameof(LevelGeneratorSystem)}] {nameof(Install)}() called twice."); }
            if (builder is null) { throw new ArgumentNullException(nameof(builder)); }

            flow = new LevelGeneratorFlow(builder);
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
