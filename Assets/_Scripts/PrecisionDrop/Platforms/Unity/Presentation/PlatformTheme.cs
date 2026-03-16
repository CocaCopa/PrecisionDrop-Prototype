using System;
using UnityEngine;

namespace PrecisionDrop.Platforms.Unity.Presentation {
    [Serializable]
    public struct PlatformTheme {
        [SerializeField] private Material regularMat;
        [SerializeField] private Material dangerMat;

        public Material RegularMat => regularMat;
        public Material DangerMat => dangerMat;

        public PlatformTheme(Material regularMat, Material dangerMat) {
            this.regularMat = regularMat;
            this.dangerMat = dangerMat;
        }
    }
}