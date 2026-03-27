using PrecisionDrop.Input.Contracts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PrecisionDrop.Input.Unity {
    public sealed class UnityInput : MonoBehaviour, IInputSource {
        [SerializeField] [Min(0f)] private float mouseSensitivity = 1f;
        [SerializeField] private bool invertRotation;

        public bool IsHolding { get; private set; }
        public bool WasPressedThisFrame { get; private set; }
        public bool WasReleasedThisFrame { get; private set; }
        public Vector2 MouseDragDelta { get; private set; }

        private PlayerInputActions playerActions;
        private int RotDir => invertRotation ? -1 : 1;

        private void Awake() {
            playerActions = new PlayerInputActions();
        }

        private void OnEnable() {
            playerActions.Player.Enable();

            playerActions.Player.ClickHold.performed += OnPress;
            playerActions.Player.ClickHold.canceled += OnRelease;

            playerActions.Player.LeftMouseDrag.performed += OnDrag;
        }

        private void LateUpdate() {
            WasPressedThisFrame = false;
            WasReleasedThisFrame = false;
            MouseDragDelta = Vector2.zero;
        }

        private void OnDisable() {
            playerActions.Player.ClickHold.performed -= OnPress;
            playerActions.Player.ClickHold.canceled -= OnRelease;

            playerActions.Player.LeftMouseDrag.performed -= OnDrag;

            playerActions.Player.Disable();

            IsHolding = false;
            WasPressedThisFrame = false;
            WasReleasedThisFrame = false;
            MouseDragDelta = Vector2.zero;
        }

        private void OnPress(InputAction.CallbackContext _) {
            IsHolding = true;
            WasPressedThisFrame = true;
        }

        private void OnRelease(InputAction.CallbackContext _) {
            IsHolding = false;
            WasReleasedThisFrame = true;
        }

        private void OnDrag(InputAction.CallbackContext context) {
            var delta = context.ReadValue<Vector2>();
            MouseDragDelta += delta * mouseSensitivity * RotDir;
        }
    }
}