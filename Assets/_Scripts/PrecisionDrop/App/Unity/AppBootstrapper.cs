using System;
using PrecisionDrop.GameFlow.Unity;
using PrecisionDrop.LevelManagment.Unity;
using PrecisionDrop.Platforms.Unity;
using PrecisionDrop.Player.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace PrecisionDrop.App.Unity {
    public sealed class AppBootstrapper : MonoBehaviour {
        [SerializeField] private PlayerSystem playerSystem;
        [SerializeField] private GameFlowSystem gameFlowSystem;
        [SerializeField] private LevelGeneratorSystem levelGeneratorSystem;
        [SerializeField] private PlatformsSystem platformsSystem;

        private void OnValidate() {
            if (!platformsSystem) Debug.LogError(Msg(nameof(platformsSystem)), this);
            if (!levelGeneratorSystem) Debug.LogError(Msg(nameof(levelGeneratorSystem)), this);
            if (!gameFlowSystem) Debug.LogError(Msg(nameof(gameFlowSystem)), this);
            if (!platformsSystem) Debug.LogError(Msg(nameof(platformsSystem)), this);
        }

        private void Awake() {
            ValidateSceneWiring();

            gameFlowSystem.Install(platformsSystem.EventBus);
            levelGeneratorSystem.Install(platformsSystem.Builder);
            playerSystem.Install(gameFlowSystem.Api);
        }

        private void Start() {
            gameFlowSystem.Init();
            levelGeneratorSystem.Init();
            playerSystem.Init();
        }

        private void ValidateSceneWiring() {
            if (!platformsSystem) throw new NullReferenceException(Msg(nameof(platformsSystem)));
            if (!levelGeneratorSystem) throw new NullReferenceException(Msg(nameof(levelGeneratorSystem)));
            if (!gameFlowSystem) throw new NullReferenceException(Msg(nameof(gameFlowSystem)));
        }

        private string Msg(string fieldName) => $"[{nameof(AppBootstrapper)}] Missing reference: {fieldName} on '{name}'.";
    }
}
