using System;

namespace PrecisionDrop.SessionTracker.Contracts {
    public interface ICombo {
        event Action<int> OnComboCounterUpdated;
        event Action OnComboCounterReset;
    }
}