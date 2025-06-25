using UnityEngine;

namespace Game.TileSystem {
    public class TileMovement : MonoBehaviour {
        public float moveSpeed = 10f;

        void Update() {
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);

            // Optional: destroy if far behind
            if (transform.position.z < -20f) {
                Destroy(gameObject);
            }
        }
    }

}
