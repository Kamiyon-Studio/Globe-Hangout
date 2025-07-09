using Core.EventSystem;
using Core.InputSystem.Events;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game.PlayerSystem {
    public class PlayerController : MonoBehaviour {

        // Lanes Testing
        [Header("Lanes")]
        [SerializeField] private int laneIndex = 2;
        [SerializeField] private List<Transform> lanes = new List<Transform>();
        [SerializeField] private float laneSwitchSpeed = 10f;

        private Vector3 targetPos;


        [Header("Jumping")]
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private float checkDistance = 1f;
        [SerializeField] private float fallMultiplier = 2.5f;


        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float playerHeight = 2f;

        private bool isGrounded = false;


        [Header("Sliding")]
        [SerializeField] private float slideForce = 10f;
        [SerializeField] private float slideTime = 1f;

        private float originalHeight;
        private Vector3 originalCenter;
        private float originalRadius;
        private Coroutine slideRoutine;

        private bool isSliding = false;


        //[Header("Smoothing")]
        //[SerializeField] private float smoothTime = 0.3f;
        //[SerializeField] private Vector2 velocity = Vector2.zero;


        private Rigidbody rb;
        private CapsuleCollider col;


        // ---------- Lifecycle ----------

        private void Awake() {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<CapsuleCollider>();
        }

        private void Start() {
            if (lanes.Count > 0 && laneIndex < lanes.Count) {
                targetPos = new Vector3(lanes[laneIndex].position.x, transform.position.y, transform.position.z);
            }
            else {
                Debug.LogError("Lanes not set up properly or invalid lane index!");
            }

            isGrounded = true;

            originalHeight = col.height;
            originalCenter = col.center;
            originalRadius = col.radius;
        }

        private void OnEnable() {
            EventBus.Subscribe<Evt_OnSwipeLeft>(OnSwipeLeft);
            EventBus.Subscribe<Evt_OnSwipeRight>(OnSwipeRight);
            EventBus.Subscribe<Evt_OnSwipeUp>(OnSwipeUp);
            EventBus.Subscribe<Evt_OnSwipeDown>(OnSwipeDown);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<Evt_OnSwipeLeft>(OnSwipeLeft);
            EventBus.Unsubscribe<Evt_OnSwipeRight>(OnSwipeRight);
            EventBus.Unsubscribe<Evt_OnSwipeUp>(OnSwipeUp);
            EventBus.Unsubscribe<Evt_OnSwipeDown>(OnSwipeDown);
        }

        private void Update() {
            HandleGroundCheck();
        }

        private void FixedUpdate() {
            if (rb != null) {
                HandleLaneSwitching();
                HandleFalling();
            }
        }


        // ---------- Event Methods ----------


        /// <summary>
        /// Handles the player jumping
        /// </summary>
        /// <param name="e"></param>
        private void OnSwipeLeft(Evt_OnSwipeLeft e) {
            Debug.Log("left");

            if (laneIndex > 0) {
                laneIndex--;
                MoveToTargetLane();
            }
        }


        /// <summary>
        /// Handles the player jumping
        /// </summary>
        /// <param name="e"></param>
        private void OnSwipeRight(Evt_OnSwipeRight e) {
            Debug.Log("right");

            if (laneIndex < lanes.Count - 1) {
                laneIndex++;
                MoveToTargetLane();
            }
        }


        /// <summary>
        /// Handles the player jumping
        /// </summary>
        /// <param name="e"></param>
        private void OnSwipeUp(Evt_OnSwipeUp e) {
            Debug.Log("up");

            HandleJump();
        }


        /// <summary>
        /// Handles the player sliding
        /// </summary>
        /// <param name="e"></param>
        private void OnSwipeDown(Evt_OnSwipeDown e) {
            Debug.Log("down");

            HandleSlide();
        }


        // ---------- Methods ----------


        /// <summary>
        /// Handles the player lane switching smoothly
        /// </summary>
        private void HandleLaneSwitching() {
            if (lanes == null || laneIndex >= lanes.Count) return;

            Vector3 currentPos = rb.position;
            Vector3 target = new Vector3(lanes[laneIndex].position.x, currentPos.y, lanes[laneIndex].position.z);

            if (Vector3.Distance(currentPos, target) > 0.01f) {
                Vector3 newPos = Vector3.MoveTowards(currentPos, target, laneSwitchSpeed * Time.fixedDeltaTime);
                rb.MovePosition(newPos);
            }
        }


        /// <summary>
        /// Moves the player to the current lane
        /// </summary>
        private void MoveToTargetLane() {
            if (lanes.Count > 0 && laneIndex < lanes.Count) {
                Vector3 laneWorldPos = lanes[laneIndex].position;
                targetPos = new Vector3(laneWorldPos.x, rb.position.y, laneWorldPos.z);
            }
        }


        /// <summary>
        /// Handles the player jumping
        /// </summary>
        private void HandleJump() {
            if (isGrounded) {
                if (isSliding) { 
                    isSliding = false;
                    if (slideRoutine != null) {
                        StopCoroutine("SlideCoroutine");
                    }
                    ResetCollider();
                }
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            }
        }


        /// <summary>
        /// Handles the player falling
        /// </summary>
        private void HandleFalling() {
            if (rb.linearVelocity.y < 0) {
                // Falling - stronger gravity
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb.linearVelocity.y > 0 && rb.linearVelocity.y < 2f) {
                // Near peak - slight extra gravity for less floatiness
                rb.linearVelocity += Vector3.up * Physics.gravity.y * 0.5f * Time.deltaTime;
            }
        }


        /// <summary>
        /// Checks if the player is on the ground
        /// </summary>
        private void HandleGroundCheck() {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + checkDistance, groundLayer);
        }


        private void HandleSlide() {
            if (!isGrounded) {
                rb.AddForce(Vector3.down * (slideForce * 1.5f), ForceMode.Impulse);
                StartCoroutine(SlideCoroutine());
            }
            else if (!isSliding && isGrounded) {
                if (isSliding) {
                    isSliding = false;
                    if (slideRoutine != null) {
                        StopCoroutine("SlideCoroutine");
                    }
                    ResetCollider();
                }

                StartCoroutine(SlideCoroutine());
            }


        }


        private IEnumerator SlideCoroutine() {
            isSliding = true;

            // Lower collider to "crouch" height
            col.height = originalHeight / 2f;
            col.center = originalCenter - new Vector3(0, originalHeight / 4f, 0);
            col.radius = originalRadius / 2f;


            // Wait for duration
            yield return new WaitForSeconds(slideTime);

            ResetCollider();
            isSliding = false;
        }


        private void ResetCollider() {
            col.height = originalHeight;
            col.center = originalCenter;
            col.radius = originalRadius;
        }
    }
}