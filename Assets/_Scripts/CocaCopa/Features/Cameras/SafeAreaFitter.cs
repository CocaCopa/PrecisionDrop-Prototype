using UnityEngine;

namespace CocaCopa.Cameras {
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaFitter : MonoBehaviour {
        private RectTransform rect;

        private void Awake() {
            rect = GetComponent<RectTransform>();
            Apply();
        }

        private void Apply() {
            Rect safe = Screen.safeArea;

            Vector2 min = safe.position;
            Vector2 max = safe.position + safe.size;

            min.x /= Screen.width;
            min.y /= Screen.height;
            max.x /= Screen.width;
            max.y /= Screen.height;

            rect.anchorMin = min;
            rect.anchorMax = max;
        }
    }
}
