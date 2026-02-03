using System;
using PrecisionDrop.Platforms.Contracts;
using UnityEngine;

namespace PrecisionDrop.Platforms.Unity {
    public sealed class PlatformsSystem : MonoBehaviour {
        [SerializeField] private PlatformBuilder builder;
        [SerializeField] private PlatformEventBus eventBus;

        public IPlatformBuilder Builder => (builder as IPlatformBuilder) ?? throw new NullReferenceException($"[{nameof(PlatformsSystem)}] {nameof(PlatformBuilder)}");
        public IPlatformEventBus EventBus => (eventBus as IPlatformEventBus) ?? throw new NullReferenceException($"[{nameof(PlatformsSystem)}] {nameof(IPlatformEventBus)}");
    }
}
