using UnityEngine;

namespace CocaCopa.Cameras {
    [ExecuteAlways]
    public class FixedAspectGameplayCamera : MonoBehaviour {
        [SerializeField] private Camera targetCamera;
        [SerializeField] private Vector2 targetAspect = new Vector2(9f, 16f);

        private void Awake() {
            if (targetCamera == null) {
                throw new System.NullReferenceException($"{nameof(targetCamera)}");
            }
        }

        private void LateUpdate() {
            if (targetCamera == null) { return; }

            float windowAspect = (float)Screen.width / Screen.height;
            Rect rect = targetCamera.rect;

            float fixedAspect = targetAspect.x / targetAspect.y;

            if (windowAspect > fixedAspect) {
                float scale = fixedAspect / windowAspect;
                rect.width = scale;
                rect.height = 1f;
                rect.x = (1f - scale) / 2f;
                rect.y = 0f;
            }
            else {
                float scale = windowAspect / fixedAspect;
                rect.width = 1f;
                rect.height = scale;
                rect.x = 0f;
                rect.y = (1f - scale) / 2f;
            }

            targetCamera.rect = rect;
        }
    }
}
