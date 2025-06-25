using Core.EventSystem;
using Game.TileSystem.Events;

using System.Collections.Generic;
using UnityEngine;

namespace Game.TileSystem {
    public class TileManager : MonoBehaviour {
        public static TileManager Instance { get; private set; }

        [Header("Tile Settings")]
        [SerializeField] private GameObject[] tilePrefab;
        [SerializeField] private Transform tileParent;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private int maxTileCount;
        [SerializeField] private GameObject[] startingTiles;

        [Header("Tile Speed")]
        [SerializeField] private float tileSpeed = 5f;


        private Queue<GameObject> tilePool = new Queue<GameObject>();
        private List<GameObject> activeTiles = new List<GameObject>();


        // ---------- Lifecycle ----------

        private void Awake() {
            if (Instance != null) {
                Debug.LogWarning("TileManager: Instance already exists");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }


        private void Start() {
            foreach (GameObject tile in startingTiles) {
                activeTiles.Add(tile);
            }

            for (int i = 0; i < maxTileCount; i++) {
                GameObject tile = Instantiate(tilePrefab[Random.Range(0, tilePrefab.Length)], tileParent);
                tile.transform.position = spawnPoint.position;
                tile.SetActive(false);
                tilePool.Enqueue(tile);
            }
                Debug.Log("Pool Initialized");
        }


        private void OnEnable() {
            EventBus.Subscribe<Evt_TileSpawnTrigger>(OnTileTrigger);
            EventBus.Subscribe<Evt_TileDespawnTrigger>(OnDespawnTile);
        }

        private void OnDisable() {
            EventBus.Unsubscribe<Evt_TileSpawnTrigger>(OnTileTrigger);
            EventBus.Unsubscribe<Evt_TileDespawnTrigger>(OnDespawnTile);
        }


        // ---------- Event Methods ----------

        private void OnTileTrigger(Evt_TileSpawnTrigger e) {
            SpawnTile();
        }

        private void OnDespawnTile(Evt_TileDespawnTrigger e) {
            DespawnTile();
        }


        // ---------- Methods ----------

        private void SpawnTile() {
            if (tilePool.Count > 0) {
                GameObject tile = tilePool.Dequeue();
                tile.SetActive(true);
                activeTiles.Add(tile);
            }
        }

        private void DespawnTile() {
            if (activeTiles.Count > 0) {
                GameObject tile = activeTiles[0];
                activeTiles.RemoveAt(0);
                tile.SetActive(false);
                tile.transform.position = spawnPoint.position;
                tilePool.Enqueue(tile);
            }
        }
    }
}