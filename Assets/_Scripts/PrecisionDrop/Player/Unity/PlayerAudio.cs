using CocaCopa.Primitives;
using UnityEngine;

namespace PrecisionDrop.Player.Unity {
    [RequireComponent(typeof(AudioSource))]
    internal sealed class PlayerAudio : MonoBehaviour {
        [Header("Bounce")]
        [SerializeField] private AudioClip bounceFull;
        [SerializeField] private AudioClip bounceShort;
        [SerializeField] private RangeFloat pitchRange;

        private AudioSource source;

        private void Awake() {
            source = GetComponent<AudioSource>();
        }

        public void PlayBounceShort() {
            float pitchValue = Random.Range(pitchRange.min, pitchRange.max);
            source.pitch = pitchValue;
            source.PlayOneShot(bounceShort);
        }

        public void PlayBounceFull() {
            source.PlayOneShot(bounceFull);
        }
    }
}