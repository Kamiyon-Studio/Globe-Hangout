using System;
using UnityEngine;

using Core.EventSystem;
using Core.InputSystem.Events;

namespace Core.InputSystem {
    public class SwipeController : MonoBehaviour {
        [Header("Swipe Detection")]
        [SerializeField] private float swipeThreshold = 50f;

        private InputSystem_Actions inputActions;
        private Vector2 startTouchPos;
        private Vector2 endTouchPos;

        private bool isTouching;

        // Development vars
        private Vector3 moveDir;

        private SwipeDirection detectedDirection;
        public enum SwipeDirection {
            None,
            Left,
            Right,
            Up,
            Down
        }

        // ---------- Lifecycle ----------
        private void Awake() {
            inputActions = new InputSystem_Actions();
        }

        private void OnEnable() {
            inputActions.Enable();
            inputActions.Player.TouchPress.started += TouchPress_started;
            inputActions.Player.TouchPress.canceled += TouchPress_canceled;

            // Development Test
            inputActions.Player.W.performed += On_W_Button;
            inputActions.Player.A.performed += On_A_Button;
            inputActions.Player.S.performed += On_S_Button;
            inputActions.Player.D.performed += On_D_Button;

        }

        private void OnDisable() {
            inputActions.Disable();
            inputActions.Player.TouchPress.started -= TouchPress_started;
            inputActions.Player.TouchPress.canceled -= TouchPress_canceled;

            // Development Test
            inputActions.Player.W.performed -= On_W_Button;
            inputActions.Player.A.performed -= On_A_Button;
            inputActions.Player.S.performed -= On_S_Button;
            inputActions.Player.D.performed -= On_D_Button;
        }

        // ---------- Input Event Handlers ----------
        private void TouchPress_started(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
            isTouching = true;
            startTouchPos = inputActions.Player.TouchContact.ReadValue<Vector2>();
        }

        private void TouchPress_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
            if (!isTouching) return;

            endTouchPos = inputActions.Player.TouchContact.ReadValue<Vector2>();
            Vector2 swipe = endTouchPos - startTouchPos;

            isTouching = false;

            if (swipe.magnitude > swipeThreshold) {
                Vector2 direction = swipe.normalized;

                if (Math.Abs(direction.x) > Math.Abs(direction.y)) {
                    detectedDirection = direction.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
                }
                else {
                    detectedDirection = direction.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
                }
            }
            else {
                detectedDirection = SwipeDirection.None;
            }


            switch (detectedDirection) {
                case SwipeDirection.Left:
                    EventBus.Publish(new Evt_OnSwipeLeft());
                    break;
                case SwipeDirection.Right:
                    EventBus.Publish(new Evt_OnSwipeRight());
                    break;
                case SwipeDirection.Up:
                    EventBus.Publish(new Evt_OnSwipeUp());
                    break;
                case SwipeDirection.Down:
                    EventBus.Publish(new Evt_OnSwipeDown());
                    break;
                default:
                    break;
            }
        }

        // ---------- Development Test Methods ----------
        private void On_W_Button(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
            EventBus.Publish(new Evt_OnSwipeUp());
        }

        private void On_S_Button(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
            EventBus.Publish(new Evt_OnSwipeDown());
        }

        private void On_A_Button(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
            EventBus.Publish(new Evt_OnSwipeLeft());
        }

        private void On_D_Button(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
            EventBus.Publish(new Evt_OnSwipeRight());
        }
    }
}