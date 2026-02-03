using UnityEngine;

namespace PrecisionDrop.Player.Unity {
    [CreateAssetMenu(menuName = "PrecisionDrop/Player/Config")]
    internal sealed class PlayerConfigAsset : ScriptableObject {
        [SerializeField] private float jumpStrength;
        [SerializeField] private int smashThreshold;

        public float JumpStrength => jumpStrength;
        public int SmashThreshold => smashThreshold;
    }
}
