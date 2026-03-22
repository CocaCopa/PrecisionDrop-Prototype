using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace PrecisionDrop.GameEnvironment.Unity {
    [Serializable]
    public struct EnvironmentTheme {
        public Cubemap skyboxMap;
        public Color towerColor;

        public EnvironmentTheme(Cubemap skyboxMap, Color towerColor) {
            this.skyboxMap = skyboxMap;
            this.towerColor = towerColor;
        }
    }
}