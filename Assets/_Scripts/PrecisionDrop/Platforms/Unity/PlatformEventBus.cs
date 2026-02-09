using System;
using PrecisionDrop.Platforms.Contracts;
using UnityEngine;

namespace PrecisionDrop.Platforms.Unity {
    internal sealed class PlatformEventBus : MonoBehaviour, IPlatformEventBus {
        [SerializeField] private PlatformBuilder builder;

        public event Action<IPlatform> OnPlatformPassed;
        public event Action<IPlatform> OnPlatformCollision;

        public void Init() {
            builder.OnPlatformGenerated += Builder_OnPlatformGenerated;
        }

        private void Builder_OnPlatformGenerated(Platform platform) {
            platform.OnPassedPlatform += Platform_OnPassedPlatform;
            platform.OnCollidedPlatform += Platform_OnCollidedPlatform;
        }

        private void Platform_OnPassedPlatform(Platform platform) {
            platform.OnPassedPlatform -= Platform_OnPassedPlatform;
            OnPlatformPassed?.Invoke(platform);
        }

        private void Platform_OnCollidedPlatform(Platform platform) {
            OnPlatformCollision?.Invoke(platform);
        }
    }
}
