using System;
using UnityEngine;

namespace PrecisionDrop.Platforms.Unity.Presentation {
    [Serializable]
    public struct PlatformTheme {
        [SerializeField] private Color regularColor;
        [SerializeField] private Color dangerColor;

        public Color RegularColor => regularColor;
        public Color DangerColor => dangerColor;

        public PlatformTheme(Color regularColor, Color dangerColor) {
            this.regularColor = regularColor;
            this.dangerColor = dangerColor;
        }
    }
}