using System;
using PrecisionDrop.GameFlow.Unity;
using PrecisionDrop.LevelGeneration.Unity;
using PrecisionDrop.Platforms.Unity;
using PrecisionDrop.Platforms.Unity.Presentation;
using PrecisionDrop.Player.Unity;
using PrecisionDrop.Player.Unity.Presentation;
using PrecisionDrop.SessionTracker.Unity;
using UnityEngine;

namespace PrecisionDrop.App.Unity {
    public sealed class AppBootstrapper : MonoBehaviour {
        [Header("Theme")]
        [SerializeField] private ThemeSelectorAsset themeSelectorAsset;

        [Header("UI Bootstrapper")]
        [SerializeField] private UiBootstrapper uiBootstrapper;

        [Header("Scene Systems")]
        [SerializeField] private PlayerSystem playerSystem;
        [SerializeField] private LevelGeneratorSystem levelGeneratorSystem;
        [SerializeField] private PlatformsSystem platformsSystem;

        private GameFlowInstaller gameFlowInstaller;
        private GameSessionInstaller gameSessionInstaller;

        private void Awake() {
            ValidateSceneWiring();
            Compose();
            uiBootstrapper.Install(gameSessionInstaller.ScoreApi);
        }

        private void Start() {
            InitializeSystems();
            uiBootstrapper.Init();
        }

        private void Compose() {
            gameFlowInstaller = new GameFlowInstaller();
            gameSessionInstaller = new GameSessionInstaller();

            LevelThemeAsset levelTheme = themeSelectorAsset.Select();
            PlayerTheme playerTheme = CreatePlayerTheme(levelTheme);
            PlatformTheme platformTheme = CreatePlatformTheme(levelTheme);

            platformsSystem.Install(platformTheme);
            gameFlowInstaller.Install(playerSystem.PlayerApi, platformsSystem.EventBus);
            levelGeneratorSystem.Install(gameFlowInstaller.Api, platformsSystem.Builder);
            playerSystem.Install(gameFlowInstaller.Api, playerTheme);
            gameSessionInstaller.Install(gameFlowInstaller.Api);
        }

        private void InitializeSystems() {
            // Order matters.
            platformsSystem.Init();
            gameFlowInstaller.Init();
            levelGeneratorSystem.Init();
            playerSystem.Init();
            gameSessionInstaller.Init();
        }

        private static PlayerTheme CreatePlayerTheme(LevelThemeAsset levelTheme) {
            return new PlayerTheme(
                levelTheme.PlayerMat,
                levelTheme.PlayerTrailMat,
                levelTheme.PlayerBounceVfxId
            );
        }

        private static PlatformTheme CreatePlatformTheme(LevelThemeAsset levelTheme) {
            return new PlatformTheme(
                levelTheme.RegularPieceMat,
                levelTheme.DangerPieceMat
            );
        }

        private void ValidateSceneWiring() {
            if (!themeSelectorAsset) { throw new NullReferenceException(Msg(nameof(themeSelectorAsset))); }
            if (!playerSystem) { throw new NullReferenceException(Msg(nameof(playerSystem))); }
            if (!platformsSystem) { throw new NullReferenceException(Msg(nameof(platformsSystem))); }
            if (!levelGeneratorSystem) { throw new NullReferenceException(Msg(nameof(levelGeneratorSystem))); }
            if (!uiBootstrapper) { throw new NullReferenceException(Msg(nameof(uiBootstrapper))); }
        }

        private string Msg(string fieldName) {
            return $"[{nameof(AppBootstrapper)}] Missing reference: {fieldName} on '{name}'.";
        }
    }
}