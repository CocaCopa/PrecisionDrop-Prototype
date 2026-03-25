using System;
using PrecisionDrop.Input.Unity;
using PrecisionDrop.App.Unity.Themes;
using PrecisionDrop.GameFlow.Contracts;
using PrecisionDrop.GameFlow.Unity;
using PrecisionDrop.Input.Contracts;
using PrecisionDrop.LevelGeneration.Unity;
using PrecisionDrop.Platforms.Contracts;
using PrecisionDrop.Platforms.Unity;
using PrecisionDrop.Player.Contracts;
using PrecisionDrop.Player.Unity;
using PrecisionDrop.SessionTracker.Contracts;
using PrecisionDrop.SessionTracker.Unity;
using UnityEngine;
using Environment = PrecisionDrop.GameEnvironment.Unity.Environment;

namespace PrecisionDrop.App.Unity {
    public sealed class AppBootstrapper : MonoBehaviour {
        [Header("Theme")]
        [SerializeField] private ThemeSelectorAsset themeSelectorAsset;

        [Header("UI Bootstrapper")]
        [SerializeField] private UiBootstrapper uiBootstrapper;

        [Header("Environment")]
        [SerializeField] private Environment environment;

        [Header("Scene Systems")]
        [SerializeField] private UnityInput unityInput;
        [SerializeField] private PlayerSystem playerSystem;
        [SerializeField] private LevelGeneratorSystem levelGeneratorSystem;
        [SerializeField] private PlatformsSystem platformsSystem;

        private GameFlowInstaller gameFlowInstaller;
        private GameSessionInstaller gameSessionInstaller;

        internal PlayerAccess PlayerAccessRef => playerSystem.PlayerApi;
        internal PlayerConfigAsset PlayerConfigRef => playerSystem.PlayerConfig;
        internal IGameFlow GameFlowRef => gameFlowInstaller.Api;
        internal IPlatformBuilder PlatformBuilderRef => platformsSystem.Builder;
        internal IPlatformEventBus PlatformEventBusRef => platformsSystem.EventBus;
        internal IScore ScoreRef => gameSessionInstaller.ScoreApi;
        internal IInputSource InputSourceRef => (IInputSource)unityInput;

        private void Awake() {
            ValidateSceneWiring();
            Compose();
            uiBootstrapper.Install(this);
        }

        private void Start() {
            InitializeSystems();
            uiBootstrapper.Init();
        }

        private void Compose() {
            gameFlowInstaller = new GameFlowInstaller();
            gameSessionInstaller = new GameSessionInstaller();

            LevelThemeAsset levelTheme = themeSelectorAsset.Select();
            environment.ApplyTheme(levelTheme.EnvironmentTheme);

            platformsSystem.Install(levelTheme.PlatformTheme);
            gameFlowInstaller.Install(PlayerAccessRef, PlayerConfigRef.SmashThreshold, PlatformEventBusRef);
            levelGeneratorSystem.Install(GameFlowRef, PlatformBuilderRef);
            playerSystem.Install(InputSourceRef, GameFlowRef, levelTheme.PlayerTheme);
            gameSessionInstaller.Install(GameFlowRef);
        }

        private void InitializeSystems() {
            // Order matters.
            platformsSystem.Init();
            gameFlowInstaller.Init();
            levelGeneratorSystem.Init();
            playerSystem.Init();
            gameSessionInstaller.Init();
        }

        private void ValidateSceneWiring() {
            if (!themeSelectorAsset) { throw new NullReferenceException(Msg(nameof(themeSelectorAsset))); }
            if (!unityInput) { throw new NullReferenceException(Msg(nameof(unityInput))); }
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