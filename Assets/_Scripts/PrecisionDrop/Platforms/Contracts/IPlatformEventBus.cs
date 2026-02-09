using System;

namespace PrecisionDrop.Platforms.Contracts {
    public interface IPlatformEventBus {
        event Action<IPlatform> OnPlatformPassed;
        event Action<IPlatform> OnPlatformCollision;
    }
}
