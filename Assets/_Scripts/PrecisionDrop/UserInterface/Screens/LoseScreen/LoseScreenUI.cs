using System;
using System.Collections;
using PrecisionDrop.GameFlow.Contracts;
using PrecisionDrop.SessionTracker.Contracts;
using PrecisionDrop.Input.Contracts;
using PrecisionDrop.UserInterface.Screens.Contracts;
using UnityEngine;

namespace PrecisionDrop.UserInterface.Screens {
    public sealed class LoseScreenUI : MonoBehaviour, ILoseScreen {
        [Header("References")]
        [SerializeField] private LoseScreenBackgroundUI backgroundUI;
        [SerializeField] private FinalScoreUI finalScoreUI;
        [SerializeField] private RestartUI restartUI;

        [Header("Sequence Delays")]
        [SerializeField] private float sequenceStartDelay;
        [SerializeField] private float backgroundDelay;
        [SerializeField] private float finalScoreDelay;
        [SerializeField] private float restartDelay;

        public event Action OnPlayerRequestedRestart;

        private IInputSource inputSource;
        private IGameFlow gameFlow;
        private IScore score;

        private bool canTapForRestart;
        private bool allowRestart;
        private int latestScore;

        public void Install(IInputSource inputSourceRef, IGameFlow gameFlowRef, IScore scoreRef) {
            inputSource = inputSourceRef;
            gameFlow = gameFlowRef;
            score = scoreRef;
        }

        public void Init() {
            canTapForRestart = false;
            allowRestart = false;
            gameFlow.OnPlayerHitDanger += GameFlow_OnPlayerHitDanger;
            score.OnScoreChanged += Score_OnScoreChanged;
        }

        private void OnDestroy() {
            gameFlow.OnPlayerHitDanger -= GameFlow_OnPlayerHitDanger;
            score.OnScoreChanged -= Score_OnScoreChanged;
        }

        private void GameFlow_OnPlayerHitDanger() {
            UpdateScoreValue(latestScore);
            Show();
        }

        private void Score_OnScoreChanged() {
            latestScore = score.CurrentScore;
        }

        private void Update() {
            CheckForRestartRequest();
        }

        private void CheckForRestartRequest() {
            if (!canTapForRestart) { return; }
            if (inputSource.WasPressedThisFrame) { allowRestart = true; }
            if (allowRestart && inputSource.WasReleasedThisFrame) { OnPlayerRequestedRestart?.Invoke(); }
        }

        private void UpdateScoreValue(int value) {
            finalScoreUI.UpdateScoreValue(value);
        }

        private void Show() {
            StartCoroutine(StartAnimationSequence());
        }

        private IEnumerator StartAnimationSequence() {
            canTapForRestart = false;
            yield return new WaitForSeconds(sequenceStartDelay);

            EnableHolderObject();

            StartCoroutine(BackgroundTrigger());
            StartCoroutine(FinalScoreTrigger());
            StartCoroutine(RestartTrigger());

            while (!restartUI.SequenceCompleted) { yield return null; }

            canTapForRestart = true;
        }

        private void EnableHolderObject() {
            GameObject holderObj = transform.GetChild(0).gameObject;
            holderObj.SetActive(true);
        }

        private IEnumerator BackgroundTrigger() {
            yield return new WaitForSeconds(backgroundDelay);
            backgroundUI.FadeIn();
        }

        private IEnumerator FinalScoreTrigger() {
            yield return new WaitForSeconds(finalScoreDelay);
            finalScoreUI.Show();
        }

        private IEnumerator RestartTrigger() {
            yield return new WaitForSeconds(restartDelay);
            restartUI.Show();
        }
    }
}