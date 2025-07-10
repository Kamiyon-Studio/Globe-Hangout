using UnityEngine;

using Core.EventSystem;
using Demo.Train.Events;

namespace Demo.Train {
    public class TrainMove : MonoBehaviour {
        [SerializeField] private Transform targetPos;
        [SerializeField] private float maxSpeed = 10f;
        [SerializeField] private float slowDownDistance = 10f;

        private bool canMove = true;
        //private bool arriving = true;
        //private bool leaving = false;

        private void OnEnable() {
            EventBus.Subscribe<Evt_OnPlayerArrived>(OnPlayerArrived);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<Evt_OnPlayerArrived>(OnPlayerArrived);
        }

        private void OnPlayerArrived(Evt_OnPlayerArrived e) {
            canMove = true;
        }

        private void Update() {
            if (!canMove || targetPos == null) return;

            Vector3 direction = targetPos.position - transform.position;
            float distance = direction.magnitude;

            // Compute dynamic speed based on distance
            float speedFactor = Mathf.Clamp01(distance / slowDownDistance);
            float currentSpeed = maxSpeed * speedFactor;

            // Move without overshooting
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos.position,
                currentSpeed * Time.deltaTime
            );

            // Stop when reached
            if (distance < 0.01f) {
                canMove = false;
            }
        }
    }
}
