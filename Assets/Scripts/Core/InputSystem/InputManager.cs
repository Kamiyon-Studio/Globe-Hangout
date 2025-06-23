using System;
using UnityEngine;

public class InputManager : MonoBehaviour {
    public static InputManager Instance { get; private set; }

    [Header("Swipe Detection")]
    [SerializeField] private float swipeThreshold = 50f;
    [SerializeField] private float continuousSwipeDelay = 0.3f; // Delay between continuous swipes

    private InputSystem_Actions inputActions;
    private Vector2 startPos;
    private Vector2 lastSwipePos;
    private bool isTouching;
    private float lastSwipeTime;

    public enum SwipeDirection {
        None,
        Left,
        Right,
        Up,
        Down
    }

    // Events for game systems to subscribe to
    public static event Action<SwipeDirection> OnSwipeDetected;
    public static event Action OnTouchStart;
    public static event Action OnTouchEnd;
    public static event Action<Vector2> OnTouchHold; // For continuous touch position

    // ---------- Lifecycle ----------
    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("InputManager: Instance already exists");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable() {
        inputActions.Enable();
        inputActions.Player.TouchPress.started += TouchPress_started;
        inputActions.Player.TouchPress.canceled += TouchPress_canceled;
    }

    private void Update() {
        if (isTouching) {
            DetectSwipe();
            HandleContinuousTouch();
        }
    }

    private void OnDisable() {
        inputActions.Disable();
        inputActions.Player.TouchPress.started -= TouchPress_started;
        inputActions.Player.TouchPress.canceled -= TouchPress_canceled;
    }

    // ---------- Input Event Handlers ----------
    private void TouchPress_started(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        isTouching = true;
        startPos = inputActions.Player.TouchContact.ReadValue<Vector2>();
        lastSwipePos = startPos;
        lastSwipeTime = 0f;

        OnTouchStart?.Invoke();
        Debug.Log("Touch Started");
    }

    private void TouchPress_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        isTouching = false;
        OnTouchEnd?.Invoke();
        Debug.Log("Touch Ended");
    }

    // ---------- Swipe Detection ----------
    private void DetectSwipe() {
        Vector2 currentPos = inputActions.Player.TouchContact.ReadValue<Vector2>();
        Vector2 swipeDelta = currentPos - lastSwipePos;

        // Check if enough time has passed for another swipe and if swipe threshold is met
        if (Time.time - lastSwipeTime >= continuousSwipeDelay && swipeDelta.magnitude >= swipeThreshold) {
            SwipeDirection detectedDirection = GetSwipeDirection(swipeDelta);

            if (detectedDirection != SwipeDirection.None) {
                OnSwipeDetected?.Invoke(detectedDirection);
                lastSwipePos = currentPos; // Update last swipe position
                lastSwipeTime = Time.time;

                Debug.Log($"Swipe Detected: {detectedDirection}");
            }
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

    private void HandleContinuousTouch() {
        Vector2 currentPos = inputActions.Player.TouchContact.ReadValue<Vector2>();
        OnTouchHold?.Invoke(currentPos);
    }

    // ---------- Public Methods ----------
    public bool IsTouching() {
        return isTouching;
    }

    public Vector2 GetCurrentTouchPosition() {
        return isTouching ? inputActions.Player.TouchContact.ReadValue<Vector2>() : Vector2.zero;
    }

    public Vector2 GetTouchDelta() {
        return isTouching ? inputActions.Player.TouchContact.ReadValue<Vector2>() - startPos : Vector2.zero;
    }

    // ---------- Settings ----------
    public void SetSwipeThreshold(float threshold) {
        swipeThreshold = threshold;
    }

    public void SetContinuousSwipeDelay(float delay) {
        continuousSwipeDelay = delay;
    }
}
