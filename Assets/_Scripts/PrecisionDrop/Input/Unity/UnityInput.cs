using PrecisionDrop.Input.Contracts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PrecisionDrop.Input.Unity {
    public sealed class UnityInput : MonoBehaviour, IInputSource {
        public bool IsHolding { get; private set; }
        public Vector2 MouseDragDelta { get; private set; }

        private PlayerInputActions playerActions;

        private void Awake() {
            playerActions = new PlayerInputActions();
        }

        private void OnEnable() {
            playerActions.Player.Enable();

            playerActions.Player.ClickHold.performed += OnPress;
            playerActions.Player.ClickHold.canceled += OnRelease;

            playerActions.Player.LeftMouseDrag.performed += OnDelta;
        }

        private void OnDisable() {
            playerActions.Player.LeftMouseDrag.performed -= OnDelta;

            playerActions.Player.ClickHold.performed -= OnPress;
            playerActions.Player.ClickHold.canceled -= OnRelease;

            playerActions.Player.Disable();
        }

        public Vector2 ConsumeDragDelta() {
            var d = MouseDragDelta;
            MouseDragDelta = Vector2.zero;
            return d;
        }

        private void OnPress(InputAction.CallbackContext _) {
            IsHolding = true;
            MouseDragDelta = Vector2.zero;
        }

        private void OnRelease(InputAction.CallbackContext _) {
            IsHolding = false;
            MouseDragDelta = Vector2.zero;
        }

        private void OnDelta(InputAction.CallbackContext ctx) {
            if (!IsHolding) return;

            var d = ctx.ReadValue<Vector2>();
            if (d.sqrMagnitude < 0.0001f) return;

            MouseDragDelta += d;
        }
    }
}
