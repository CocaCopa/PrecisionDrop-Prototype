using System;
using UnityEngine;

namespace CocaCopa.Unity.Components.BetterGravity {
    [Serializable]
    internal struct ScaleSettings {
        [Tooltip("Curve controlling how gravity strength scales over time (0-1) during ramp-up.")]
        public AnimationCurve curve;
        [Tooltip("Percentage (0-100) of the curve progress to start from when gravity switches direction.\n\n• 0 → start of the curve\n• 100 → end of the curve.")]
        [Range(0f, 100f)] public float curveOffset;
        [Tooltip("Time in seconds for gravity strength to scale from 0 to full strength.")]
        [Min(0f)] public float duration;
        public ScaleSettings(AnimationCurve curve, float curveOffset, float duration) {
            this.curve = curve;
            this.curveOffset = curveOffset;
            this.duration = duration;
        }
    }
}
