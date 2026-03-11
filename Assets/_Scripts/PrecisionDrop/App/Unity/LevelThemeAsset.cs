using UnityEngine;

namespace PrecisionDrop.App.Unity {
    [CreateAssetMenu(fileName = "NewThemeConfig", menuName = "PrecisionDrop/Level/ThemeConfig")]
    internal sealed class LevelThemeAsset : ScriptableObject {
        [Header("Player")]
        [SerializeField] private Material playerMat;
        [SerializeField] private Material playerTrailMat;
        [SerializeField] private string playerBounceVfxId;

        [Header("Platform")]
        [SerializeField] private Material dangerMat;
        [SerializeField] private Material normalMat;

        // Player
        internal Material PlayerMat => playerMat;
        internal Material PlayerTrailMat => playerTrailMat;
        internal string PlayerBounceVfxId => playerBounceVfxId;

        // Platform
        internal Material DangerPieceMat => dangerMat;
        internal Material RegularPieceMat => normalMat;
    }
}