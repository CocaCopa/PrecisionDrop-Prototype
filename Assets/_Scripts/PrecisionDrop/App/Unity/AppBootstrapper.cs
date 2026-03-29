using System;
using PrecisionDrop.App.Unity.Themes;
using PrecisionDrop.GameFlow.Contracts;
using PrecisionDrop.GameFlow.Unity;
using PrecisionDrop.Input.Contracts;
using PrecisionDrop.Input.Unity;
using PrecisionDrop.LevelGeneration.Unity;
using PrecisionDrop.Platforms.Unity;
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

        [Header("Audio")]
        [SerializeField] private ComboAudio comboAudio;

        [Header("Scene Systems")]
        [SerializeField] private UnityInput unityInput;
        [SerializeField] private PlayerSystem playerSystem;
        [SerializeField] private LevelGeneratorSystem levelGeneratorSystem;
        [SerializeField] private PlatformsSystem platformsSystem;

        private GameFlowInstaller gameFlowInstaller;
        private GameSessionInstaller gameSessionInstaller;

        private void Awake() {
            ValidateSceneWiring();
            Compose();
            uiBootstrapper.Install(new UiAccess(
                gameFlowInstaller.Api,
                gameSessionInstaller.ScoreApi,
                gameSessionInstaller.ComboApi,
                unityInput
            ));
        }

        private void Start() {
            InitializeSystems();
            uiBootstrapper.Init();
        }

        private void Compose() {
            gameFlowInstaller = new GameFlowInstaller();
            gameSessionInstaller = new GameSessionInstaller();

            LevelThemeAsset levelTheme = themeSelectorAsset.Select() ?? throw new NullReferenceException($"[{nameof(AppBootstrapper)}] {nameof(themeSelectorAsset)} returned null {nameof(LevelThemeAsset)}.");

            environment.ApplyTheme(levelTheme.EnvironmentTheme);

            platformsSystem.Install(levelTheme.PlatformTheme);
            gameFlowInstaller.Install(
                playerSystem.PlayerApi,
                playerSystem.PlayerConfig.SmashThreshold,
                platformsSystem.EventBus
            );
            levelGeneratorSystem.Install(
                gameFlowInstaller.Api,
                platformsSystem.Builder
            );
            playerSystem.Install(
                unityInput,
                gameFlowInstaller.Api,
                levelTheme.PlayerTheme
            );
            gameSessionInstaller.Install(gameFlowInstaller.Api);
            comboAudio.Install(gameSessionInstaller.ComboApi);
        }

        private void InitializeSystems() {
            // Order matters.
            platformsSystem.Init();
            gameFlowInstaller.Init();
            levelGeneratorSystem.Init();
            playerSystem.Init();
            gameSessionInstaller.Init();

            comboAudio.Init();
        }

        private void ValidateSceneWiring() {
            if (!themeSelectorAsset) { throw new NullReferenceException(Msg(nameof(themeSelectorAsset))); }
            if (!uiBootstrapper) { throw new NullReferenceException(Msg(nameof(uiBootstrapper))); }
            if (!environment) { throw new NullReferenceException(Msg(nameof(environment))); }
            if (!unityInput) { throw new NullReferenceException(Msg(nameof(unityInput))); }
            if (!playerSystem) { throw new NullReferenceException(Msg(nameof(playerSystem))); }
            if (!platformsSystem) { throw new NullReferenceException(Msg(nameof(platformsSystem))); }
            if (!levelGeneratorSystem) { throw new NullReferenceException(Msg(nameof(levelGeneratorSystem))); }
            if (!comboAudio) { throw new NullReferenceException(Msg(nameof(comboAudio))); }
        }

        private string Msg(string fieldName) {
            return $"[{nameof(AppBootstrapper)}] Missing reference: {fieldName} on '{name}'.";
        }
    }

    public readonly struct UiAccess {
        public readonly IGameFlow GameFlow;
        public readonly IScore Score;
        public readonly ICombo Combo;
        public readonly IInputSource Input;

        public UiAccess(IGameFlow gameFlow, IScore score, ICombo combo, IInputSource input) {
            GameFlow = gameFlow ?? throw new ArgumentNullException(nameof(gameFlow));
            Score = score ?? throw new ArgumentNullException(nameof(score));
            Combo = combo ?? throw new ArgumentNullException(nameof(combo));
            Input = input ?? throw new ArgumentNullException(nameof(input));
        }
    }
}