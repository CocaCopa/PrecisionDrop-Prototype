using CocaCopa.Primitives;
using UnityEngine;

namespace PrecisionDrop.Platforms.Unity {
    [RequireComponent(typeof(AudioSource))]
    internal sealed class PlatformAudio : MonoBehaviour {
        [Header("Bounce")]
        [SerializeField] private AudioClip breakEffect;
        [SerializeField] private RangeFloat pitchRange;

        private AudioSource source;

        private void Awake() {
            source = GetComponent<AudioSource>();
        }

        public void PlayBreak() {
            float pitchValue = Random.Range(pitchRange.min, pitchRange.max);
            source.pitch = pitchValue;
            source.PlayOneShot(breakEffect);
        }
    }
}