using System;
using System.Linq;
using UnityEngine;

namespace PrecisionDrop.App.Unity {
    [CreateAssetMenu(fileName = "NewThemeSelector", menuName = "PrecisionDrop/Level/ThemeSelector")]
    public sealed class ThemeSelectorAsset : ScriptableObject {
        [SerializeField] private LevelThemeAsset[] themes;

        private bool nullEntriesWarning = false;

        internal LevelThemeAsset Select() {
            if (themes == null || themes.Length == 0) {
                throw new InvalidOperationException($"[{nameof(ThemeSelectorAsset)}] No themes assigned in '{name}'.");
            }

            if (themes.Any(theme => theme == null)) {
                Debug.LogWarning($"[{nameof(ThemeSelectorAsset)}] Null entries detected.");
            }

            int start = UnityEngine.Random.Range(0, themes.Length);

            for (int i = 0; i < themes.Length; i++) {
                int idx = (start + i) % themes.Length;
                var theme = themes[idx];
                if (theme != null) {
                    return theme;
                }
                else if (!nullEntriesWarning) {
                    nullEntriesWarning = true;
                }
            }

            throw new InvalidOperationException($"[{nameof(ThemeSelectorAsset)}] All assigned themes are null in '{name}'.");
        }
    }
}