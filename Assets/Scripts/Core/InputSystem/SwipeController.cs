using System;
using UnityEngine;

using Core.EventSystem;
using Core.InputSystem.Events;

namespace Core.InputSystem {
    public class SwipeController : MonoBehaviour {
        [Header("Swipe Detection")]
        [SerializeField] private float swipeThreshold = 50f;
        [SerializeField] private float continuousSwipeDelay = 0.3f; // Delay between continuous swipes

        private InputSystem_Actions inputActions;
        private Vector2 startPos;
        private Vector2 lastSwipePos;

        private bool isTouching;
        private float lastSwipeTime;

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
        }

        private void OnDisable() {
            inputActions.Disable();
            inputActions.Player.TouchPress.started -= TouchPress_started;
            inputActions.Player.TouchPress.canceled -= TouchPress_canceled;
        }

        // ---------- Input Event Handlers ----------
        private void TouchPress_started(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
            isTouching = true;
            lastSwipePos = startPos;
            lastSwipeTime = 0f;
            startPos = inputActions.Player.TouchContact.ReadValue<Vector2>();
        }

        private void TouchPress_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
            isTouching = false;
        }

        // ---------- Swipe Detection ----------
        private void DetectSwipe() {
            Vector2 currentPos = inputActions.Player.TouchContact.ReadValue<Vector2>();
            Vector2 swipeDelta = currentPos - lastSwipePos;

            // Only consider a swipe if enough time has passed and threshold is met
            if (Time.time - lastSwipeTime >= continuousSwipeDelay && swipeDelta.magnitude >= swipeThreshold) {
                detectedDirection = GetSwipeDirection(swipeDelta);

                if (detectedDirection != SwipeDirection.None) {
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
                    }

                    lastSwipePos = currentPos;
                    lastSwipeTime = Time.time;
                }
            } else {
                detectedDirection = SwipeDirection.None;
            }
        }


        private SwipeDirection GetSwipeDirection(Vector2 swipeDelta) {
            // Determine if horizontal or vertical swipe is dominant
            if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y)) {
                // Horizontal swipe
                return swipeDelta.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
            }
            else {
                // Vertical swipe
                return swipeDelta.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
            }
        }

        // ---------- Helper Functions ----------
        public void RunSwipeController() {
            if (isTouching) {
                DetectSwipe();
            }
        }
    }
}