using UnityEngine;

namespace PrecisionDrop.Player.Unity {
    [CreateAssetMenu(fileName = "NewPlayerConfig", menuName = "PrecisionDrop/Player/Config")]
    public sealed class PlayerConfigAsset : ScriptableObject {
        [SerializeField] [Min(0f)] private float jumpStrength;
        [SerializeField] [Min(1)] private int smashThreshold;

        public float JumpStrength => jumpStrength;
        public int SmashThreshold => smashThreshold;
    }
}