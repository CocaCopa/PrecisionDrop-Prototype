using System;
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

        public Vector2 MouseDragDelta => IsHolding
            ? playerActions.Player.LeftMouseDrag.ReadValue<Vector2>() * mouseSensitivity * RotDir
            : Vector2.zero;

        private PlayerInputActions playerActions;
        private int RotDir => invertRotation ? -1 : 1;

        private void Awake() {
            playerActions = new PlayerInputActions();
        }

        private void OnEnable() {
            playerActions.Player.Enable();

            playerActions.Player.ClickHold.performed += OnPress;
            playerActions.Player.ClickHold.canceled += OnRelease;
        }

        private void Update() {
            ResetFrameFlags();
        }

        private void ResetFrameFlags() {
            WasPressedThisFrame = false;
            WasReleasedThisFrame = false;
        }

        private void OnDisable() {
            playerActions.Player.ClickHold.performed -= OnPress;
            playerActions.Player.ClickHold.canceled -= OnRelease;

            playerActions.Player.Disable();
        }

        private void OnPress(InputAction.CallbackContext _) {
            IsHolding = true;
            WasPressedThisFrame = true;
        }

        private void OnRelease(InputAction.CallbackContext _) {
            IsHolding = false;
            WasReleasedThisFrame = true;
        }
    }
}