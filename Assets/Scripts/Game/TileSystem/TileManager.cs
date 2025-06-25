using System.Collections.Generic;
using UnityEngine;

namespace Game.TileSystem {
    public class TileManager : MonoBehaviour {
        public GameObject[] tilePrefabs;
        public Transform player; // reference point for spawn logic
        public int tilesOnScreen = 5;
        public float tileLength = 10f;

        private float zSpawn = 0f;
        private List<GameObject> activeTiles = new List<GameObject>();

        void Start() {
            for (int i = 0; i < tilesOnScreen; i++) {
                SpawnTile(i == 0 ? 0 : Random.Range(0, tilePrefabs.Length));
            }
        }

        void Update() {
            // Check the last tile's Z position. Spawn new tile if needed.
            if (activeTiles.Count == 0 || activeTiles[activeTiles.Count - 1].transform.position.z <= tileLength * (tilesOnScreen - 1)) {
                SpawnTile(Random.Range(0, tilePrefabs.Length));
            }

            // Remove tiles behind the player
            if (activeTiles.Count > 0 && activeTiles[0].transform.position.z < -tileLength) {
                Destroy(activeTiles[0]);
                activeTiles.RemoveAt(0);
            }
        }

        void SpawnTile(int index) {
            GameObject tile = Instantiate(tilePrefabs[index], new Vector3(0, 0, zSpawn), Quaternion.identity);
            activeTiles.Add(tile);
            zSpawn += tileLength;
        }
    }
}