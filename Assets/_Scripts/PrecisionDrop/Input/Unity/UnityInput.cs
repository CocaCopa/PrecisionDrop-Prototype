using PrecisionDrop.Input.Contracts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PrecisionDrop.Input.Unity {
    internal sealed class UnityInput : MonoBehaviour, IInputSource {
        [SerializeField, Min(0f)] private float mouseSensitivity;

        public bool IsHolding { get; private set; }
        public Vector2 MouseDragDelta => IsHolding ? playerActions.Player.LeftMouseDrag.ReadValue<Vector2>() : Vector2.zero;

        private PlayerInputActions playerActions;

        private void Awake() {
            playerActions = new PlayerInputActions();
        }

        private void OnEnable() {
            playerActions.Player.Enable();

            playerActions.Player.ClickHold.performed += OnPress;
            playerActions.Player.ClickHold.canceled += OnRelease;
        }

        private void OnDisable() {
            playerActions.Player.ClickHold.performed -= OnPress;
            playerActions.Player.ClickHold.canceled -= OnRelease;

            playerActions.Player.Disable();
        }

        public Vector2 ConsumeDragDelta() {
            var d = MouseDragDelta;
            return d;
        }

        private void OnPress(InputAction.CallbackContext _) {
            IsHolding = true;
        }

        private void OnRelease(InputAction.CallbackContext _) {
            IsHolding = false;
        }
    }
}
