using PrecisionDrop.Platforms.Unity.Presentation;
using PrecisionDrop.Player.Unity.Presentation;
using UnityEngine;

namespace PrecisionDrop.App.Unity {
    [CreateAssetMenu(fileName = "NewThemeConfig", menuName = "PrecisionDrop/Level/ThemeConfig")]
    internal sealed class LevelThemeAsset : ScriptableObject {
        [SerializeField] private PlayerTheme playerTheme;
        [Space(10f)]
        [SerializeField] private PlatformTheme platformTheme;

        internal PlayerTheme PlayerTheme => playerTheme;
        internal PlatformTheme PlatformTheme => platformTheme;
    }
}