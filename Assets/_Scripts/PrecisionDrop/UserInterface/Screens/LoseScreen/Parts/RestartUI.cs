using System.Collections;
using CocaCopa.Unity.Components;
using UnityEngine;

namespace PrecisionDrop.UserInterface.Screens {
    internal sealed class RestartUI : MonoBehaviour {
        [SerializeField] private PanelAnimator panelAnimator;
        [SerializeField] private PulseAnimation pulseAnimation;

        public bool SequenceCompleted { get; private set; }

        public void Show() {
            StartCoroutine(ShowRoutine());
        }

        public void Hide() {
            pulseAnimation.Stop();
            panelAnimator.Hide();
        }

        private IEnumerator ShowRoutine() {
            SequenceCompleted = false;
            panelAnimator.Show();
            while (panelAnimator.IsAnimating) { yield return null; }
            pulseAnimation.Play();
            SequenceCompleted = true;
        }
    }
}