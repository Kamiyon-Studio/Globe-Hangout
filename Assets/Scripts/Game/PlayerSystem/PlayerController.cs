using UnityEngine;

using Core.EventSystem;
using Core.InputSystem;
using Core.InputSystem.Events;

namespace Game.PlayerSystem {
    public class PlayerController : MonoBehaviour {
        private Rigidbody rb;


        // ---------- Lifecycle ----------

        private void Awake() {
            rb = GetComponent<Rigidbody>();
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


        // ---------- Event Methods ----------
        private void OnSwipeLeft(Evt_OnSwipeLeft e) {
            Debug.Log("Swipe left");
        }

        private void OnSwipeRight(Evt_OnSwipeRight e) {
            Debug.Log("Swipe right");
        }

        private void OnSwipeUp(Evt_OnSwipeUp e) {
            Debug.Log("Swipe up");
        }

        private void OnSwipeDown(Evt_OnSwipeDown e) {
            Debug.Log("Swipe down");
        }
    }
}