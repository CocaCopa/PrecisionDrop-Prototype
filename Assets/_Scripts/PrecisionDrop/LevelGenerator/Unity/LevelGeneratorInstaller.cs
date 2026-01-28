using System;
using PrecisionDrop.LevelManagment.Runtime;
using PrecisionDrop.Platforms.Contracts;
using UnityEngine;

namespace PrecisionDrop.LevelManagment.Unity {
    internal sealed class LevelGeneratorInstaller : MonoBehaviour {
        [SerializeField] private GameObject platformBuilder;

        private IPlatformBuilder Builder {
            get {
                if (!platformBuilder.TryGetComponent<IPlatformBuilder>(out var builder)) {
                    throw new Exception($"GameObject '{platformBuilder.name}' does not contain a component implementing {nameof(IPlatformBuilder)}");
                }
                return builder;
            }
        }

        private void Awake() {
            var flow = new LevelGeneratorFlow(Builder);
            flow.Initialize();
        }
    }
}
