using Core.EventSystem;
using Game.TileSystem.Events;
using Game.PlayerSystem;


using UnityEngine;

namespace Game.TileSystem {
    public class TileTrigger : MonoBehaviour {
        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.GetComponent<DespawnPoint>() != null) {
                EventBus.Publish(new Evt_TileDespawnTrigger());
                Debug.Log("Despawn Triggered");
            }

            if (other.gameObject.GetComponent<PlayerController>() != null) {
                EventBus.Publish(new Evt_TileSpawnTrigger());
                Debug.Log("Spawn Triggered");
            }
        }
    }

}