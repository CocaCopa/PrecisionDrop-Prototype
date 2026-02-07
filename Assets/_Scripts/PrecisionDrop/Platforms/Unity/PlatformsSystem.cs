using System;
using PrecisionDrop.Platforms.Contracts;
using PrecisionDrop.Platforms.Unity.Presentation;
using UnityEngine;

namespace PrecisionDrop.Platforms.Unity {
    public sealed class PlatformsSystem : MonoBehaviour {
        [SerializeField] private PlatformBuilder builder;
        [SerializeField] private PlatformEventBus eventBus;

        public IPlatformBuilder Builder => (builder as IPlatformBuilder) ?? throw new NullReferenceException($"[{nameof(PlatformsSystem)}] {nameof(PlatformBuilder)}");
        public IPlatformEventBus EventBus => (eventBus as IPlatformEventBus) ?? throw new NullReferenceException($"[{nameof(PlatformsSystem)}] {nameof(IPlatformEventBus)}");

        public void Install(PlatformTheme theme) {
            if (!theme.IsValid) { throw new ArgumentNullException($"[{nameof(PlatformsSystem)}] {nameof(theme)}"); }
            
            builder.Install(theme);
        }

        public void Init() {
            builder.Init();
            eventBus.Init();
        }
    }
}
