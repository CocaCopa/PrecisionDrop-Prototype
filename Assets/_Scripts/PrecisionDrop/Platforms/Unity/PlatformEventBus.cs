using System;
using PrecisionDrop.Platforms.Contracts;
using UnityEngine;

namespace PrecisionDrop.Platforms.Unity {
    internal sealed class PlatformEventBus : MonoBehaviour, IPlatformEventBus {
        [SerializeField] private PlatformBuilder builder;

        public event Action<IPlatform> OnPlatformPassed;
        public event Action<IPlatform, CollisionData> OnPlatformCollision;

        public void Init() {
            builder.OnPlatformGenerated += Builder_OnPlatformGenerated;
        }

        private void OnDestroy() {
            builder.OnPlatformGenerated -= Builder_OnPlatformGenerated;
        }

        private void Builder_OnPlatformGenerated(PlatformRoot platform) {
            platform.OnPassedPlatform += Platform_OnPassedPlatform;
            platform.OnCollidedPlatform += Platform_OnCollidedPlatform;
        }

        private void Platform_OnPassedPlatform(PlatformRoot platform) {
            UnsubscribeFromPlatform(platform);
            OnPlatformPassed?.Invoke(platform);
        }

        private void Platform_OnCollidedPlatform(PlatformRoot platform, CollisionData colData) {
            OnPlatformCollision?.Invoke(platform, colData);
        }

        private void UnsubscribeFromPlatform(PlatformRoot platform) {
            platform.OnPassedPlatform -= Platform_OnPassedPlatform;
            platform.OnCollidedPlatform -= Platform_OnCollidedPlatform;
        }
    }
}