using System;
using TMPro;
using UnityEngine;
using PrecisionDrop.SessionTracker.Contracts;

namespace PrecisionDrop.UserInterface.Gameplay {
    public sealed class ScoreUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI scoreTxt;

        private IScore score;
        private bool installed;
        private bool initialized;

        private void Awake() {
            if (!scoreTxt) { throw new NullReferenceException($"[{nameof(ScoreUI)}] {nameof(scoreTxt)} is not assigned."); }
        }

        public void Install(IScore scoreRef) {
            if (installed) { throw new InvalidOperationException($"[{nameof(ScoreUI)}] {nameof(Install)}() called twice."); }
            score = scoreRef ?? throw new ArgumentNullException(nameof(scoreRef));

            installed = true;
        }

        public void Init() {
            if (!installed) { throw new InvalidOperationException($"[{nameof(ScoreUI)}] {nameof(Init)}() called before {nameof(Install)}()."); }
            if (initialized) { throw new InvalidOperationException($"[{nameof(ScoreUI)}] {nameof(Init)}() called twice."); }

            initialized = true;
            score.OnScoreChanged += Score_OnScoreChanged;
            RefreshScore();
        }

        private void Score_OnScoreChanged() {
            RefreshScore();
        }

        private void RefreshScore() {
            scoreTxt.SetText(score.CurrentScore.ToString("0"));
        }

        private void OnDestroy() {
            if (score != null) { score.OnScoreChanged -= Score_OnScoreChanged; }
        }
    }
}