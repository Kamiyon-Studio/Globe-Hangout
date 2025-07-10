using UnityEngine;
using Core.EventSystem;
using Core.InputSystem.Events;
using System.Collections;

namespace Demo.Train {
    public class PlayerMove : MonoBehaviour {
        [SerializeField] private float changeTargetTimer = 0.5f;
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private Transform targetPos;
        [SerializeField] private Transform rightTrainPos;
        [SerializeField] private Transform leftTrainPos;

        private bool reachedTarget = false;
        private bool isMoving = false;
        private bool shouldRotate = false; // New flag to control rotation
        private Transform currentTarget;

        private void Start() {
            currentTarget = targetPos;
            isMoving = true;
            shouldRotate = false; // Don't rotate for the initial target
        }

        private void OnEnable() {
            EventBus.Subscribe<Evt_OnSwipeLeft>(OnLeftInput);
            EventBus.Subscribe<Evt_OnSwipeRight>(OnRightInput);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<Evt_OnSwipeLeft>(OnLeftInput);
            EventBus.Unsubscribe<Evt_OnSwipeRight>(OnRightInput);
        }

        private void Update() {
            if (isMoving && !reachedTarget) {
                MovePlayerToTarget(currentTarget);
            }
        }

        private void OnLeftInput(Evt_OnSwipeLeft e) {
            // Only allow input after reaching the first target
            if (reachedTarget) {
                StartCoroutine(OnLeftInputCoroutine());
            }
        }

        private void OnRightInput(Evt_OnSwipeRight e) {
            // Only allow input after reaching the first target
            if (reachedTarget) {
                StartCoroutine(OnRightInputCoroutine());
                
            }
        }

        private IEnumerator OnLeftInputCoroutine() {
            yield return new WaitForSeconds(changeTargetTimer);

            currentTarget = leftTrainPos;
            isMoving = true;
            shouldRotate = true; // Enable rotation for direction changes
            reachedTarget = false;
        }

        private IEnumerator OnRightInputCoroutine() {
            yield return new WaitForSeconds(changeTargetTimer);

            currentTarget = rightTrainPos;
            isMoving = true;
            shouldRotate = true; // Enable rotation for direction changes
            reachedTarget = false;
        }

        private void MovePlayerToTarget(Transform target) {
            if (target == null) return;

            Vector3 targetFlat = new Vector3(target.position.x, transform.position.y, target.position.z);
            Vector3 direction = targetFlat - transform.position;

            // Only rotate if needed
            if (shouldRotate && direction != Vector3.zero && isMoving) {
                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 10f);
            }

            float distance = direction.magnitude;

            // Optional slowdown near target
            float slowDownDistance = 1.5f;
            float speedFactor = Mathf.Clamp01(distance / slowDownDistance);
            float currentSpeed = moveSpeed * speedFactor;

            // Move smoothly toward target
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetFlat,
                currentSpeed * Time.deltaTime
            );

            // Stop when close enough
            if (distance < 0.01f) {
                isMoving = false;

                if (target == targetPos) {
                    reachedTarget = true;
                    Debug.Log("Reached the target");
                }
            }
        }
    }
}