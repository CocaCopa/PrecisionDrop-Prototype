using System;
using CocaCopa.Unity.Components;
using PrecisionDrop.GameFlow.Contracts;
using TMPro;
using UnityEngine;
using PrecisionDrop.SessionTracker.Contracts;

namespace PrecisionDrop.UserInterface.Gameplay {
    public sealed class ScoreUI : MonoBehaviour {
        [SerializeField] private PanelAnimator scoreAnimator;
        [SerializeField] private TextMeshProUGUI scoreTxt;

        private IGameFlow gameFlow;
        private IScore score;

        private bool installed;
        private bool initialized;

        private void Awake() {
            if (!scoreTxt) { throw new NullReferenceException($"[{nameof(ScoreUI)}] {nameof(scoreTxt)} is not assigned."); }
        }

        public void Install(IGameFlow gameFlowRef, IScore scoreRef) {
            if (installed) { throw new InvalidOperationException($"[{nameof(ScoreUI)}] {nameof(Install)}() called twice."); }
            score = scoreRef ?? throw new ArgumentNullException($"{nameof(ScoreUI)}] {nameof(scoreRef)}");
            gameFlow = gameFlowRef ?? throw new ArgumentException($"[{nameof(ScoreUI)}] {nameof(gameFlowRef)}");

            installed = true;
        }

        public void Init() {
            if (!installed) { throw new InvalidOperationException($"[{nameof(ScoreUI)}] {nameof(Init)}() called before {nameof(Install)}()."); }
            if (initialized) { throw new InvalidOperationException($"[{nameof(ScoreUI)}] {nameof(Init)}() called twice."); }

            initialized = true;
            gameFlow.OnPlayerHitDanger += GameFlow_OnPlayerHitDanger;
            score.OnScoreChanged += Score_OnScoreChanged;
            RefreshScore(0);
        }

        private void GameFlow_OnPlayerHitDanger() {
            scoreAnimator.Hide();
        }

        private void Score_OnScoreChanged(ScoreInfo scoreInfo) {
            int newTotal = scoreInfo.Total;
            RefreshScore(newTotal);
        }

        private void RefreshScore(int newTotal) {
            scoreTxt.SetText(newTotal.ToString("0"));
        }

        private void OnDestroy() {
            if (score != null) { score.OnScoreChanged -= Score_OnScoreChanged; }
        }
    }
}