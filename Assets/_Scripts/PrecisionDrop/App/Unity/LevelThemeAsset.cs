using UnityEngine;

namespace PrecisionDrop.App.Unity {
    [CreateAssetMenu(fileName = "NewThemeConfig", menuName = "PrecisionDrop/Level/ThemeConfig")]
    internal sealed class LevelThemeAsset : ScriptableObject {
        [Header("Player")]
        [SerializeField] private Material playerMat;
        [SerializeField] private Material playerTrailMat;

        [Header("Platform")]
        [SerializeField] private Material dangerMat;
        [SerializeField] private Material normalMat;

        internal Material PlayerMat => playerMat;
        internal Material PlayerTrailMat => playerTrailMat;

        internal Material DangerPieceMat => dangerMat;
        internal Material RegularPieceMat => normalMat;
    }
}