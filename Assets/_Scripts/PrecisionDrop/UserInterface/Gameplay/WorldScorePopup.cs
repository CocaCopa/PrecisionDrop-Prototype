using System;
using System.Collections;
using CocaCopa.ObjectPooling;
using CocaCopa.Unity.Numerics;
using PrecisionDrop.SessionTracker.Contracts;
using UnityEngine;

namespace PrecisionDrop.UserInterface.Gameplay {
    public class WorldScorePopup : MonoBehaviour {
        [Header("Pool Selection")]
        [SerializeField] private string groupId;
        [SerializeField] private string scoreId;

        private IScore score;

        private bool installed;
        private bool initialized;

        public void Install(IScore scoreRef) {
            if (installed) { throw new InvalidOperationException($"[{nameof(WorldScorePopup)}] {nameof(Install)}() called twice."); }
            score = scoreRef ?? throw new ArgumentException($"[{nameof(ScoreUI)}] {nameof(scoreRef)}");

            installed = true;
        }

        public void Init() {
            if (!installed) { throw new InvalidOperationException($"[{nameof(ScoreUI)}] {nameof(Init)}() called before {nameof(Install)}()."); }
            if (initialized) { throw new InvalidOperationException($"[{nameof(ScoreUI)}] {nameof(Init)}() called twice."); }

            initialized = true;
            score.OnScorePopupAvailable += Score_OnScorePopupAvailable;
        }

        private void Score_OnScorePopupAvailable(ScorePopupInfo popupInfo) {
            if (popupInfo.ScoreInfo.ScoreType != ScoreType.Smash) { return; }

            GameObject worldScore = PoolApi.Rent(groupId, scoreId, transform);
            var scoreWorldUI = worldScore.GetComponentInChildren<ScoreWorldUI>();
            if (!scoreWorldUI) { throw new NullReferenceException($"[{nameof(WorldScorePopup)}] Could not fetch '{nameof(ScoreWorldUI)}' from rented object."); }

            scoreWorldUI.Show(popupInfo.ScoreInfo.AddedAmount, popupInfo.ContactPoint.ToUnity());
            StartCoroutine(ReturnUIToPool(scoreWorldUI));
        }

        private static IEnumerator ReturnUIToPool(ScoreWorldUI element) {
            while (element.IsPlayingAnimation) { yield return null; }
            PoolApi.Return(element.gameObject);
        }
    }
}