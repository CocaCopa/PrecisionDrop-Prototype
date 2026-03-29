using System;
using PrecisionDrop.SessionTracker.Contracts;
using UnityEngine;

namespace PrecisionDrop.SessionTracker.Unity {
    [RequireComponent(typeof(AudioSource))]
    public sealed class ComboAudio : MonoBehaviour {
        private enum ScaleType {
            Major,
            Minor,
            HarmonicMajor,
            HarmonicMinor
        }

        [SerializeField] private AudioClip comboClip;
        [SerializeField] private ScaleType scaleType = ScaleType.Major;

        private static readonly int[] MajorScaleSemitones = {
            0, 2, 4, 5, 7, 9, 11, 12
        };

        private static readonly int[] MinorScaleSemitones = {
            0, 2, 3, 5, 7, 8, 10, 12
        };

        private static readonly int[] HarmonicMajorScaleSemitones = {
            0, 2, 4, 5, 7, 8, 11, 12
        };

        private static readonly int[] HarmonicMinorScaleSemitones = {
            0, 2, 3, 5, 7, 8, 11, 12
        };

        private AudioSource source;
        private ICombo combo;

        private void Awake() {
            source = GetComponent<AudioSource>();
        }

        public void Install(ICombo comboRef) {
            combo = comboRef;
        }

        public void Init() {
            combo.OnComboCounterUpdated += Combo_OnComboCounterUpdated;
            combo.OnComboCounterReset += Combo_OnComboCounterReset;
        }

        private void OnDestroy() {
            if (combo == null) { return; }

            combo.OnComboCounterUpdated -= Combo_OnComboCounterUpdated;
            combo.OnComboCounterReset -= Combo_OnComboCounterReset;
        }

        private void Combo_OnComboCounterReset() {
            source.pitch = 1f;
        }

        private void Combo_OnComboCounterUpdated(int counter) {
            if (comboClip == null) { return; }

            int comboStep = Mathf.Max(0, counter - 1);
            int semitone = GetPingPongSemitone(comboStep);
            float pitch = SemitoneToPitch(semitone);

            source.pitch = pitch;
            source.PlayOneShot(comboClip);
        }

        private int GetPingPongSemitone(int step) {
            int[] scale = GetSelectedScale();

            int maxIndex = scale.Length - 1;
            int pingPongLength = maxIndex * 2;
            int wrappedStep = step % pingPongLength;

            int scaleIndex = wrappedStep <= maxIndex
                ? wrappedStep
                : pingPongLength - wrappedStep;

            return scale[scaleIndex];
        }

        private int[] GetSelectedScale() {
            switch (scaleType) {
                case ScaleType.Minor:
                    return MinorScaleSemitones;

                case ScaleType.HarmonicMajor:
                    return HarmonicMajorScaleSemitones;

                case ScaleType.HarmonicMinor:
                    return HarmonicMinorScaleSemitones;

                case ScaleType.Major:
                default:
                    return MajorScaleSemitones;
            }
        }

        private static float SemitoneToPitch(int semitone) {
            return Mathf.Pow(2f, semitone / 12f);
        }
    }
}