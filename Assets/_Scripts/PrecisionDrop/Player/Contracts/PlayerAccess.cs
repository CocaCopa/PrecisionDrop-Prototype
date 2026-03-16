using System;

namespace PrecisionDrop.Player.Contracts {
    public readonly struct PlayerAccess {
        public IPlayerSphere Sphere { get; }
        public IPlayerStateRead StateRead { get; }
        public IPlayerStateWrite StateWrite { get; }

        public PlayerAccess(IPlayerSphere sphere, IPlayerStateRead stateRead, IPlayerStateWrite stateWrite) {
            Sphere = sphere ?? throw new ArgumentNullException($"[{nameof(PlayerAccess)}] {nameof(sphere)}");
            StateRead = stateRead ?? throw new ArgumentNullException($"[{nameof(PlayerAccess)}] {nameof(stateRead)}");
            StateWrite = stateWrite ?? throw new ArgumentNullException($"[{nameof(PlayerAccess)}] {nameof(stateWrite)}");
        }
    }
}