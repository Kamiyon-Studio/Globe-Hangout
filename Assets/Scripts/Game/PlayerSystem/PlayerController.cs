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
        [SerializeField] private float moveSpeed = 10f;

        private Vector3 targetPos;
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

        private void FixedUpdate() {
            if (rb != null && Vector3.Distance(transform.position, targetPos) > 0.01f) {
                Vector3 newPos = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.fixedDeltaTime);
                rb.MovePosition(newPos);
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
    }
}