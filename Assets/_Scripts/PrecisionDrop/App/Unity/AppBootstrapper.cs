using System;
using PrecisionDrop.GameFlow.Unity;
using PrecisionDrop.LevelGeneration.Unity;
using PrecisionDrop.Platforms.Unity;
using PrecisionDrop.Platforms.Unity.Presentation;
using PrecisionDrop.Player.Unity;
using PrecisionDrop.Player.Unity.Presentation;
using UnityEngine;

namespace PrecisionDrop.App.Unity {
    public sealed class AppBootstrapper : MonoBehaviour {
        [SerializeField] private ThemeSelectorAsset themeSelectorAsset;
        [Space(10f)]
        [SerializeField] private PlayerSystem playerSystem;
        [SerializeField] private GameFlowSystem gameFlowSystem;
        [SerializeField] private LevelGeneratorSystem levelGeneratorSystem;
        [SerializeField] private PlatformsSystem platformsSystem;

        private void Awake() {
            ValidateSceneWiring();
            var levelTheme = themeSelectorAsset.Select();
            
            gameFlowSystem.Install(playerSystem.PlayerApi, platformsSystem.EventBus);
            levelGeneratorSystem.Install(gameFlowSystem.Api, platformsSystem.Builder);
            playerSystem.Install(gameFlowSystem.Api, CreatePlayerTheme(levelTheme));
            platformsSystem.Install(CreatePlatformTheme(levelTheme));
        }

        private void Start() {
            platformsSystem.Init();
            gameFlowSystem.Init();
            levelGeneratorSystem.Init();
            playerSystem.Init();
        }

        private static PlayerTheme CreatePlayerTheme(LevelThemeAsset levelTheme) {
            return new PlayerTheme(
                levelTheme.PlayerMat
            );
        }

        private static PlatformTheme CreatePlatformTheme(LevelThemeAsset levelTheme) {
            return new PlatformTheme(
                levelTheme.RegularPieceMat,
                levelTheme.DangerPieceMat
            );
        }

        private void ValidateSceneWiring() {
            if (!themeSelectorAsset) throw new NullReferenceException(Msg(nameof(themeSelectorAsset)));
            if (!playerSystem) throw new NullReferenceException(Msg(nameof(playerSystem)));
            if (!platformsSystem) throw new NullReferenceException(Msg(nameof(platformsSystem)));
            if (!levelGeneratorSystem) throw new NullReferenceException(Msg(nameof(levelGeneratorSystem)));
            if (!gameFlowSystem) throw new NullReferenceException(Msg(nameof(gameFlowSystem)));
        }

        private string Msg(string fieldName) => $"[{nameof(AppBootstrapper)}] Missing reference: {fieldName} on '{name}'.";
    }
}
