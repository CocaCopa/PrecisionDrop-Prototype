using System;
using CocaCopa.Unity.Components;
using PrecisionDrop.SessionTracker.Contracts;
using TMPro;
using UnityEngine;

namespace PrecisionDrop.UserInterface.Gameplay {
    public class ComboUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI comboValueTxt;
        [SerializeField] private PopAnimation popAnim;

        private ICombo combo;

        private bool installed;
        private bool initialized;

        public void Install(ICombo comboRef) {
            if (installed) { throw new InvalidOperationException($"[{nameof(ComboUI)}] {nameof(Install)}() called twice."); }
            combo = comboRef ?? throw new NullReferenceException($"[{nameof(ComboUI)}] {nameof(comboRef)}");

            installed = true;
        }

        public void Init() {
            if (!installed) { throw new InvalidOperationException($"[{nameof(ComboUI)}] {nameof(Init)}() called before {nameof(Install)}()."); }
            if (initialized) { throw new InvalidOperationException($"[{nameof(ComboUI)}] {nameof(Init)}() called twice."); }

            initialized = true;

            combo.OnComboCounterUpdated += Combo_OnComboCounterUpdated;
        }

        private void Combo_OnComboCounterUpdated(int counter) {
            comboValueTxt.SetText($"{counter.ToString()}x");
            popAnim.Play();
        }
    }
}