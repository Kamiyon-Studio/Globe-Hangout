using UnityEngine;

namespace Game.TileSystem {
    public class TileMovement : MonoBehaviour {
        [Header("Tile Speed")]
        [SerializeField] private float tileSpeed = 5f;

        private void Update() {
            transform.position += new Vector3(0, 0, -tileSpeed) * Time.deltaTime;
        }
    }
}