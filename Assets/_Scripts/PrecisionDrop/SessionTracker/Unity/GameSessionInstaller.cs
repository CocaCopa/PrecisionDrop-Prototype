using System;
using PrecisionDrop.GameFlow.Contracts;
using PrecisionDrop.SessionTracker.Contracts;

namespace PrecisionDrop.SessionTracker.Unity {
    public sealed class GameSessionInstaller {
        private Score score;
        private Combo combo;

        private bool installed;

        public IScore ScoreApi => score as IScore ?? throw new NullReferenceException($"[{nameof(GameSessionInstaller)}] {nameof(score)}");
        public ICombo ComboApi => combo as ICombo ?? throw new NullReferenceException($"[{nameof(GameSessionInstaller)}] {nameof(combo)}");

        public void Install(IGameFlow gameFlow) {
            if (installed) { throw new InvalidOperationException($"[{nameof(GameSessionInstaller)}] {nameof(Install)}() called twice."); }
            if (gameFlow is null) { throw new ArgumentNullException($"[{nameof(GameSessionInstaller)}] {nameof(gameFlow)}"); }

            score = new Score(gameFlow);
            combo = new Combo(gameFlow);
        }

        public void Init() {
            score.Init();
            combo.Init();
        }
    }
}