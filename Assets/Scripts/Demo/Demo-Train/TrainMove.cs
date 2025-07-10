using UnityEngine;

namespace Demo.Train {
    public class TrainMove : MonoBehaviour {
        [SerializeField] private Transform targetPos;
        [SerializeField] private float maxSpeed = 10f;
        [SerializeField] private float slowDownDistance = 10f;

        private bool canMove = true;

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
