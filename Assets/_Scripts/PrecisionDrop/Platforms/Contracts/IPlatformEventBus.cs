using System;

namespace PrecisionDrop.Platforms.Contracts {
    public interface IPlatformEventBus {
        event Action OnPlatformPassed;
        event Action OnPlatformCollision;
    }
}
