using System.Collections;
using CocaCopa.Unity.Components;
using TMPro;
using UnityEngine;

namespace PrecisionDrop.UserInterface.Screens {
    internal sealed class FinalScoreUI : MonoBehaviour {
        [SerializeField] private PanelAnimator titleAnimator;
        [SerializeField] private PanelAnimator valueAnimator;
        [Space(10f)]
        [SerializeField] private TextMeshProUGUI scoreValueTxt;

        [Header("Settings")]
        [SerializeField] private float showSequenceDelay;
        [SerializeField] private float hideSequenceDelay;

        public void UpdateScoreValue(int value) {
            scoreValueTxt.SetText(value.ToString());
        }

        public void Show() {
            StartCoroutine(ShowRoutine());
        }

        private IEnumerator ShowRoutine() {
            titleAnimator.Show();
            yield return new WaitForSeconds(showSequenceDelay);
            valueAnimator.Show();
        }

        public void Hide() {
            StartCoroutine(HideRoutine());
        }

        private IEnumerator HideRoutine() {
            titleAnimator.Hide();
            yield return new WaitForSeconds(hideSequenceDelay);
            valueAnimator.Hide();
        }
    }
}