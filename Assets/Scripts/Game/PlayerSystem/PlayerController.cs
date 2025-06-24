using UnityEngine;
using System.Collections.Generic;

using Core.EventSystem;
using Core.InputSystem.Events;

namespace Game.PlayerSystem {
    public class PlayerController : MonoBehaviour {

        // Lanes Testing
        [Header("Lanes")]
        [SerializeField] private int laneIndex = 2;
        [SerializeField] private List<Transform> lanes = new List<Transform>();

        private Vector3 targetPos;



        [Header("Jumping")]
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private float checkDistance = 1f;


        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float playerHeight = 2f;

        private bool isGrounded = false;


        [Header("Smoothing")]
        [SerializeField] private float smoothTime = 0.3f;
        [SerializeField] private Vector2 velocity = Vector2.zero;



        private Rigidbody rb;


        // ---------- Lifecycle ----------

        private void Awake() {
            rb = GetComponent<Rigidbody>();
        }

        private void Start() {
            if (lanes.Count > 0 && laneIndex < lanes.Count) {
                targetPos = new Vector3(lanes[laneIndex].position.x, transform.position.y, transform.position.z);
            }
            else {
                Debug.LogError("Lanes not set up properly or invalid lane index!");
            }

            isGrounded = true;
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
                Vector3 currentPos = transform.position;
                Vector2 currentXZ = new Vector2(currentPos.x, currentPos.z);
                Vector2 targetXZ = new Vector2(targetPos.x, targetPos.z);

                if (Vector2.Distance(currentXZ, targetXZ) > 0.01f) {
                    Vector2 newPosXZ = Vector2.SmoothDamp(currentXZ, targetXZ, ref velocity, smoothTime);
                    Vector3 newPos = new Vector3(newPosXZ.x, currentPos.y, newPosXZ.y);
                    rb.MovePosition(newPos);
                }
            }
        }


        // ---------- Event Methods ----------
        private void OnSwipeLeft(Evt_OnSwipeLeft e) {
            Debug.Log("left");

            if (laneIndex > 0) {
                laneIndex--;
                MoveToLane();
            }
        }

        private void OnSwipeRight(Evt_OnSwipeRight e) {
            Debug.Log("right");

            if (laneIndex < lanes.Count - 1) {
                laneIndex++;
                MoveToLane();
            }
        }

        private void OnSwipeUp(Evt_OnSwipeUp e) {
            Debug.Log("up");

            HandleJump();
        }

        private void OnSwipeDown(Evt_OnSwipeDown e) {
            Debug.Log("down");
        }


        // ---------- Methods ----------

        private void MoveToLane() {
            if (lanes.Count > 0 && laneIndex < lanes.Count) {
                Vector3 newTargetPos = new Vector3(lanes[laneIndex].position.x, transform.position.y, transform.position.z);
                targetPos = newTargetPos;
            }
        }

        private void HandleJump() {
            if (isGrounded) {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }

        private void HandleGroundCheck() {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + checkDistance, groundLayer);
        }
    }
}